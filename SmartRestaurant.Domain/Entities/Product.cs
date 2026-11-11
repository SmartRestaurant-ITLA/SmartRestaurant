using SmartRestaurant.Domain.Common;
using SmartRestaurant.Domain.Exceptions;

namespace SmartRestaurant.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public int CategoryId { get; private set; }
    public bool IsActive { get; private set; }

    // Requerido por EF Core
    private Product() { }

    public Product(string name, string? description, decimal price, int categoryId)
    {
        SetDetails(name, description, price, categoryId);
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public void UpdateDetails(string name, string? description, decimal price, int categoryId)
    {
        SetDetails(name, description, price, categoryId);
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

    // Reglas: nombre obligatorio, precio no negativo, categoría indicada
    private void SetDetails(string name, string? description, decimal price, int categoryId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre del producto es obligatorio.");
        if (price < 0)
            throw new DomainException("El precio no puede ser negativo.");
        if (categoryId <= 0)
            throw new DomainException("Debe seleccionar una categoría.");

        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        Price = price;
        CategoryId = categoryId;
    }
}