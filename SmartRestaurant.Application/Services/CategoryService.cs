using SmartRestaurant.Application.Interfaces.Repositories;
using SmartRestaurant.Application.Interfaces.Services;
using SmartRestaurant.Domain.Entities;

namespace SmartRestaurant.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Category>> GetAllAsync() => _repository.GetAllAsync();

    public Task<Category?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

    public async Task<int> CreateAsync(string name)
    {
        var category = new Category(name);   // la entidad valida el nombre
        await _repository.AddAsync(category);
        return category.Id;
    }

    public async Task<bool> UpdateAsync(int id, string name)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category is null) return false;

        category.Rename(name);
        await _repository.UpdateAsync(category);
        return true;
    }

    public async Task<bool> SetActiveAsync(int id, bool isActive)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category is null) return false;

        if (isActive) category.Activate();
        else category.Deactivate();

        await _repository.UpdateAsync(category);
        return true;
    }
}