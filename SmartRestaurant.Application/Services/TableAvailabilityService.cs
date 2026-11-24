using SmartRestaurant.Application.DTOs.Tables;
using SmartRestaurant.Application.Interfaces.Repositories;
using SmartRestaurant.Application.Interfaces.Services;
using SmartRestaurant.Domain.Entities;

namespace SmartRestaurant.Application.Services;

public class TableAvailabilityService : ITableAvailabilityService
{
    private readonly ITableRepository _tableRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IOrderRepository _orderRepository;

    public TableAvailabilityService(
        ITableRepository tableRepository,
        IReservationRepository reservationRepository,
        IOrderRepository orderRepository)
    {
        _tableRepository = tableRepository;
        _reservationRepository = reservationRepository;
        _orderRepository = orderRepository;
    }

    public async Task<IReadOnlyList<TableOperationalStatusDto>> GetOperationalStatusesAsync(
        DateTime? asOf = null,
        CancellationToken ct = default)
    {
        var instant = asOf ?? DateTime.Now;
        var tables = await _tableRepository.GetAllAsync(ct);
        var openOrderTableIds = (await _orderRepository.GetOpenOrderTableIdsAsync(ct)).ToHashSet();
        var reservedTableIds = (await _reservationRepository.GetBlockingReservationTableIdsAtAsync(instant, ct)).ToHashSet();

        return tables
            .Select(table => new TableOperationalStatusDto
            {
                Id = table.Id,
                Number = table.Number,
                Capacity = table.Capacity,
                IsActive = table.IsActive,
                State = GetState(table, openOrderTableIds, reservedTableIds)
            })
            .ToArray();
    }

    public async Task<IReadOnlyList<Table>> GetAvailableTablesForReservationAsync(
        DateTime startAt,
        int durationMinutes,
        int partySize,
        int? excludedReservationId = null,
        CancellationToken ct = default)
    {
        var endAt = GetIntervalEnd(startAt, durationMinutes, partySize);
        var tables = await _tableRepository.GetAllAsync(ct);
        var openOrderTableIds = (await _orderRepository.GetOpenOrderTableIdsAsync(ct)).ToHashSet();
        var reservedTableIds = (await _reservationRepository.GetBlockingReservationTableIdsAsync(
            startAt,
            endAt,
            excludedReservationId,
            ct)).ToHashSet();

        return tables
            .Where(table => table.IsActive && table.Capacity >= partySize)
            .Where(table => !openOrderTableIds.Contains(table.Id))
            .Where(table => !reservedTableIds.Contains(table.Id))
            .ToArray();
    }

    public async Task<bool> CanAssignReservationAsync(
        int tableId,
        DateTime startAt,
        int durationMinutes,
        int partySize,
        int? excludedReservationId = null,
        CancellationToken ct = default)
    {
        if (tableId <= 0)
        {
            return false;
        }

        var availableTables = await GetAvailableTablesForReservationAsync(
            startAt,
            durationMinutes,
            partySize,
            excludedReservationId,
            ct);

        return availableTables.Any(table => table.Id == tableId);
    }

    public async Task<bool> CanReceiveOrderAsync(
        int tableId,
        DateTime? at = null,
        CancellationToken ct = default)
    {
        if (tableId <= 0)
        {
            return false;
        }

        var table = await _tableRepository.GetByIdAsync(tableId, ct);
        if (table is null || !table.IsActive)
        {
            return false;
        }

        if (await _orderRepository.HasOpenOrderForTableAsync(tableId, ct: ct))
        {
            return false;
        }

        var reservedTableIds = await _reservationRepository.GetBlockingReservationTableIdsAtAsync(
            at ?? DateTime.Now,
            ct);

        return !reservedTableIds.Contains(tableId);
    }

    public async Task<bool> CanDeactivateTableAsync(int tableId, CancellationToken ct = default)
    {
        if (tableId <= 0)
        {
            return false;
        }

        return !await _orderRepository.HasOpenOrderForTableAsync(tableId, ct: ct);
    }

    private static TableOperationalState GetState(
        Table table,
        IReadOnlySet<int> openOrderTableIds,
        IReadOnlySet<int> reservedTableIds)
    {
        if (!table.IsActive)
        {
            return TableOperationalState.Inactive;
        }

        if (openOrderTableIds.Contains(table.Id))
        {
            return TableOperationalState.Occupied;
        }

        if (reservedTableIds.Contains(table.Id))
        {
            return TableOperationalState.Reserved;
        }

        return TableOperationalState.Available;
    }

    private static DateTime GetIntervalEnd(DateTime startAt, int durationMinutes, int partySize)
    {
        if (durationMinutes <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(durationMinutes), "La duración debe ser positiva.");
        }

        if (partySize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(partySize), "El tamaño del grupo debe ser positivo.");
        }

        try
        {
            return startAt.AddMinutes(durationMinutes);
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new ArgumentOutOfRangeException(nameof(durationMinutes), "El intervalo de la reserva no es válido.");
        }
    }
}
