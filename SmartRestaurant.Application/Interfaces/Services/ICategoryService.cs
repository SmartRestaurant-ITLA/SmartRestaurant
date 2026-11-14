using SmartRestaurant.Domain.Entities;

namespace SmartRestaurant.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<List<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<int> CreateAsync(string name);
    Task<bool> UpdateAsync(int id, string name);
    Task<bool> SetActiveAsync(int id, bool isActive);
}