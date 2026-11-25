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
    private readonly ICategoryRepository _categoryRepository;   // Catálogo (César)
    private readonly IValidator<CreateProductDto> _createValidator;
    private readonly IValidator<UpdateProductDto> _updateValidator;

    public ProductService(
        IProductRepository repository,
        ICategoryRepository categoryRepository,
        IValidator<CreateProductDto> createValidator,
        IValidator<UpdateProductDto> updateValidator)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(bool includeInactive = false, CancellationToken ct = default)
    {
        var products = await _repository.GetAllAsync(includeInactive, ct);
        var categoryNames = await GetCategoryNamesAsync();
        return products.Select(product => ToDto(product, categoryNames)).ToArray();
    }

    public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var product = await _repository.GetByIdAsync(id, ct);
        if (product is null)
        {
            return null;
        }

        var categoryNames = await GetCategoryNamesAsync();
        return ToDto(product, categoryNames);
    }

    public async Task<UpdateProductDto?> GetForEditAsync(int id, CancellationToken ct = default)
    {
        var product = await _repository.GetByIdAsync(id, ct);
        return product is null
            ? null
            : new UpdateProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                CategoryId = product.CategoryId
            };
    }

    public async Task<int> CreateAsync(CreateProductDto dto, CancellationToken ct = default)
    {
        await _createValidator.ValidateAndThrowAsync(dto, ct);

        var name = dto.Name.Trim();
        await EnsureNameIsUniqueAsync(name, null, ct);
        await EnsureCategoryIsValidAsync(dto.CategoryId);

        var now = DateTime.UtcNow;
        var product = new Product(name, dto.Price)
        {
            CreatedAt = now,
            UpdatedAt = now
        };
        product.SetCatalogInfo(dto.Description, dto.CategoryId);

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
        await EnsureCategoryIsValidAsync(dto.CategoryId);

        product.Update(name, dto.Price);
        product.SetCatalogInfo(dto.Description, dto.CategoryId);
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

    // Catálogo (César): regla 8.2 "categoría válida" (que exista y esté activa)
    private async Task EnsureCategoryIsValidAsync(int? categoryId)
    {
        var category = categoryId is null ? null : await _categoryRepository.GetByIdAsync(categoryId.Value);
        if (category is null || !category.IsActive)
        {
            throw new ValidationException(
            [
                new ValidationFailure(nameof(CreateProductDto.CategoryId), "La categoría seleccionada no existe o está inactiva.")
            ]);
        }
    }

    // Catálogo (César): nombres de categorías para mostrar en listas y detalles
    private async Task<Dictionary<int, string>> GetCategoryNamesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.ToDictionary(category => category.Id, category => category.Name);
    }

    private static ProductDto ToDto(Product product, IReadOnlyDictionary<int, string> categoryNames) =>
        new()
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            IsActive = product.IsActive,
            Description = product.Description,
            CategoryId = product.CategoryId,
            CategoryName = product.CategoryId is int id && categoryNames.TryGetValue(id, out var name) ? name : null
        };
}