using SmartRestaurant.Application.DTOs.Products;

namespace SmartRestaurant.Application.Interfaces.Services;

public interface IProductService
{
    Task<IReadOnlyList<ProductDto>> GetAllAsync(bool includeInactive = false, CancellationToken ct = default);

    Task<ProductDto?> GetByIdAsync(int id, CancellationToken ct = default);

    Task<UpdateProductDto?> GetForEditAsync(int id, CancellationToken ct = default);

    Task<int> CreateAsync(CreateProductDto dto, CancellationToken ct = default);

    Task<bool> UpdateAsync(UpdateProductDto dto, CancellationToken ct = default);

    Task<bool> SetActiveAsync(int id, bool isActive, CancellationToken ct = default);
}
