using FluentValidation;
using FluentValidation.Results;
using SmartRestaurant.Application.DTOs.Products;
using SmartRestaurant.Application.Interfaces.Repositories;
using SmartRestaurant.Application.Interfaces.Services;
using SmartRestaurant.Domain.Entities;

namespace SmartRestaurant.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly IValidator<CreateProductDto> _createValidator;
    private readonly IValidator<UpdateProductDto> _updateValidator;

    public ProductService(
        IProductRepository repository,
        IValidator<CreateProductDto> createValidator,
        IValidator<UpdateProductDto> updateValidator)
    {
        _repository = repository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(bool includeInactive = false, CancellationToken ct = default)
    {
        var products = await _repository.GetAllAsync(includeInactive, ct);
        return products.Select(ToDto).ToArray();
    }

    public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var product = await _repository.GetByIdAsync(id, ct);
        return product is null ? null : ToDto(product);
    }

    public async Task<UpdateProductDto?> GetForEditAsync(int id, CancellationToken ct = default)
    {
        var product = await _repository.GetByIdAsync(id, ct);
        return product is null
            ? null
            : new UpdateProductDto { Id = product.Id, Name = product.Name, Price = product.Price };
    }

    public async Task<int> CreateAsync(CreateProductDto dto, CancellationToken ct = default)
    {
        await _createValidator.ValidateAndThrowAsync(dto, ct);

        var name = dto.Name.Trim();
        await EnsureNameIsUniqueAsync(name, null, ct);

        var now = DateTime.UtcNow;
        var product = new Product(name, dto.Price)
        {
            CreatedAt = now,
            UpdatedAt = now
        };

        await _repository.AddAsync(product, ct);
        return product.Id;
    }

    public async Task<bool> UpdateAsync(UpdateProductDto dto, CancellationToken ct = default)
    {
        await _updateValidator.ValidateAndThrowAsync(dto, ct);

        var product = await _repository.GetByIdAsync(dto.Id, ct);
        if (product is null)
        {
            return false;
        }

        var name = dto.Name.Trim();
        await EnsureNameIsUniqueAsync(name, dto.Id, ct);
        product.Update(name, dto.Price);
        await _repository.UpdateAsync(product, ct);
        return true;
    }

    public async Task<bool> SetActiveAsync(int id, bool isActive, CancellationToken ct = default)
    {
        var product = await _repository.GetByIdAsync(id, ct);
        if (product is null)
        {
            return false;
        }

        if (isActive)
        {
            product.Activate();
        }
        else
        {
            product.Deactivate();
        }

        await _repository.UpdateAsync(product, ct);
        return true;
    }

    private async Task EnsureNameIsUniqueAsync(string name, int? excludeId, CancellationToken ct)
    {
        if (await _repository.ExistsByNameAsync(name, excludeId, ct))
        {
            throw new ValidationException(
            [
                new ValidationFailure(nameof(CreateProductDto.Name), "Ya existe un producto con ese nombre.")
            ]);
        }
    }

    private static ProductDto ToDto(Product product) =>
        new()
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            IsActive = product.IsActive
        };
}
