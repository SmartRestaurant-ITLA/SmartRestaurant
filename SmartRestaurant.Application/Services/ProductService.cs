using SmartRestaurant.Application.Interfaces.Repositories;
using SmartRestaurant.Application.Interfaces.Services;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Exceptions;

namespace SmartRestaurant.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public Task<List<Product>> GetAllAsync() => _productRepository.GetAllAsync();

    public Task<Product?> GetByIdAsync(int id) => _productRepository.GetByIdAsync(id);

    // Contrato con Pedidos
    public Task<List<Product>> GetActiveAsync() => _productRepository.GetActiveAsync();

    public async Task<int> CreateAsync(string name, string? description, decimal price, int categoryId)
    {
        await EnsureCategoryIsValidAsync(categoryId);

        var product = new Product(name, description, price, categoryId);   // la entidad valida nombre y precio
        await _productRepository.AddAsync(product);
        return product.Id;
    }

    public async Task<bool> UpdateAsync(int id, string name, string? description, decimal price, int categoryId)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product is null) return false;

        await EnsureCategoryIsValidAsync(categoryId);

        product.UpdateDetails(name, description, price, categoryId);
        await _productRepository.UpdateAsync(product);
        return true;
    }

    public async Task<bool> SetActiveAsync(int id, bool isActive)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product is null) return false;

        if (isActive) product.Activate();
        else product.Deactivate();

        await _productRepository.UpdateAsync(product);
        return true;
    }

    // Regla 8.2: categoría válida (que exista y esté activa)
    private async Task EnsureCategoryIsValidAsync(int categoryId)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId);
        if (category is null || !category.IsActive)
            throw new DomainException("La categoría seleccionada no existe o está inactiva.");
    }
}