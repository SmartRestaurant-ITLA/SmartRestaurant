using SmartRestaurant.Application.DTOs.Tables;
using SmartRestaurant.Domain.Entities;

namespace SmartRestaurant.Application.Interfaces.Services;

public interface ITableAvailabilityService
{
    Task<IReadOnlyList<TableOperationalStatusDto>> GetOperationalStatusesAsync(
        DateTime? asOf = null,
        CancellationToken ct = default);

    Task<IReadOnlyList<Table>> GetAvailableTablesForReservationAsync(
        DateTime startAt,
        int durationMinutes,
        int partySize,
        int? excludedReservationId = null,
        CancellationToken ct = default);

    Task<bool> CanAssignReservationAsync(
        int tableId,
        DateTime startAt,
        int durationMinutes,
        int partySize,
        int? excludedReservationId = null,
        CancellationToken ct = default);

    Task<bool> CanReceiveOrderAsync(
        int tableId,
        DateTime? at = null,
        CancellationToken ct = default);

    Task<bool> CanDeactivateTableAsync(
        int tableId,
        CancellationToken ct = default);
}
