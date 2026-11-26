using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;

namespace SmartRestaurant.Application.Interfaces.Repositories;

public interface IInventoryMovementRepository
{
    // Guarda el movimiento nuevo y el stock actualizado del insumo en una sola operación:
    // o se guardan los dos, o no se guarda ninguno.
    Task RegisterAsync(InventoryItem item, InventoryMovement movement, CancellationToken ct = default);

    // Historial con filtros opcionales (null = sin filtro), del más reciente al más antiguo.
    // "from" y "to" se comparan por día y ambos se incluyen.
    Task<IReadOnlyList<InventoryMovement>> GetHistoryAsync(
        int? inventoryItemId,
        InventoryMovementType? type,
        DateTime? from,
        DateTime? to,
        CancellationToken ct = default);
}
