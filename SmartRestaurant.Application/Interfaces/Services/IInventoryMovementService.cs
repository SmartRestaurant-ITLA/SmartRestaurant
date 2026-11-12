using SmartRestaurant.Application.DTOs.Inventory;
using SmartRestaurant.Domain.Enums;

namespace SmartRestaurant.Application.Interfaces.Services;

public interface IInventoryMovementService
{
    Task<int> RegisterAsync(RegisterInventoryMovementDto dto, CancellationToken ct = default);          // registrar entrada o salida

    Task<IReadOnlyList<InventoryMovementDto>> GetHistoryAsync(                                          // consultar historial
        int? inventoryItemId = null,
        InventoryMovementType? type = null,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken ct = default);
}
