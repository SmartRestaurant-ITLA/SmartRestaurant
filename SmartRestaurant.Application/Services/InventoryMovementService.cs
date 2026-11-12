using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using SmartRestaurant.Application.DTOs.Inventory;
using SmartRestaurant.Application.Interfaces.Repositories;
using SmartRestaurant.Application.Interfaces.Services;
using SmartRestaurant.Domain.Enums;

namespace SmartRestaurant.Application.Services;

public class InventoryMovementService : IInventoryMovementService
{
    private readonly IInventoryMovementRepository _movementRepository;
    private readonly IInventoryItemRepository _itemRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<RegisterInventoryMovementDto> _registerValidator;

    public InventoryMovementService(
        IInventoryMovementRepository movementRepository,
        IInventoryItemRepository itemRepository,
        IMapper mapper,
        IValidator<RegisterInventoryMovementDto> registerValidator)
    {
        _movementRepository = movementRepository;
        _itemRepository = itemRepository;
        _mapper = mapper;
        _registerValidator = registerValidator;
    }

    // REGISTRAR ENTRADA O SALIDA
    // Si no hay stock suficiente o el insumo está inactivo, el dominio lanza DomainException.
    public async Task<int> RegisterAsync(RegisterInventoryMovementDto dto, CancellationToken ct = default)
    {
        // 1. Validar los datos del formulario
        await _registerValidator.ValidateAndThrowAsync(dto, ct);

        // 2. Cargar el insumo (con su stock actual guardado en la base)
        var item = await _itemRepository.GetByIdAsync(dto.InventoryItemId, ct);
        if (item is null)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(RegisterInventoryMovementDto.InventoryItemId), "El insumo seleccionado no existe.")
            });
        }

        // 3. El dominio cambia el stock y crea el movimiento al mismo tiempo
        var movement = dto.Type == InventoryMovementType.Entry
            ? item.RegisterEntry(dto.Quantity, dto.MovementDate, dto.Reference, dto.Observation)
            : item.RegisterExit(dto.Quantity, dto.MovementDate, dto.Reference, dto.Observation);

        // 4. Guardar las dos cosas juntas
        await _movementRepository.RegisterAsync(item, movement, ct);
        return movement.Id;
    }

    // CONSULTAR HISTORIAL
    public async Task<IReadOnlyList<InventoryMovementDto>> GetHistoryAsync(
        int? inventoryItemId = null,
        InventoryMovementType? type = null,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken ct = default)
    {
        var movements = await _movementRepository.GetHistoryAsync(inventoryItemId, type, from, to, ct);
        return _mapper.Map<List<InventoryMovementDto>>(movements);
    }
}
