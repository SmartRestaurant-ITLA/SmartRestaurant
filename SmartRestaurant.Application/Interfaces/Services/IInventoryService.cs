using SmartRestaurant.Application.DTOs.Inventory;

namespace SmartRestaurant.Application.Interfaces.Services;

public interface IInventoryService
{
    Task<IReadOnlyList<InventoryItemDto>> GetAllAsync(string? search = null, bool includeInactive = false, CancellationToken ct = default);   // listar

    Task<InventoryItemDto?> GetByIdAsync(int id, CancellationToken ct = default);                                                          // consultar

    Task<UpdateInventoryItemDto?> GetForEditAsync(int id, CancellationToken ct = default);                                                 // llenar el formulario de editar

    Task<int> CreateAsync(CreateInventoryItemDto dto, CancellationToken ct = default);                                                     // crear

    Task<bool> UpdateAsync(UpdateInventoryItemDto dto, CancellationToken ct = default);                                                    // editar
}