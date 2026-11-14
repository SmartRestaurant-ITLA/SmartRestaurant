using SmartRestaurant.Domain.Common;
using SmartRestaurant.Domain.Exceptions;

namespace SmartRestaurant.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    // Requerido por EF Core
    private Category() { }

    public Category(string name)
    {
        SetName(name);
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public void Rename(string name)
    {
        SetName(name);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    // Regla: nombre obligatorio
    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre de la categoría es obligatorio.");

        Name = name.Trim();
    }
}