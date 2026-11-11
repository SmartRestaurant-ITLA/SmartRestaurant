using SmartRestaurant.Domain.Entities;

namespace SmartRestaurant.Application.Interfaces.Services;

public interface IProductService
{
    // CRUD
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<int> CreateAsync(string name, string? description, decimal price, int categoryId);
    Task<bool> UpdateAsync(int id, string name, string? description, decimal price, int categoryId);
    Task<bool> SetActiveAsync(int id, bool isActive);

    // Contrato con Pedidos: solo productos activos
    Task<List<Product>> GetActiveAsync();
}