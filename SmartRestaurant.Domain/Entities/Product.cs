using SmartRestaurant.Domain.Common;

namespace SmartRestaurant.Domain.Entities;

public class Product : BaseEntity
{
    private Product()
    {
    }

    public Product(string name, decimal price)
    {
        SetName(name);
        SetPrice(price);
        IsActive = true;
    }

    public string Name { get; private set; } = string.Empty;

    public decimal Price { get; private set; }

    public bool IsActive { get; private set; }

    // Catálogo (César): descripción opcional y categoría
    public string? Description { get; private set; }

    public int? CategoryId { get; private set; }

    public void Update(string name, decimal price)
    {
        SetName(name);
        SetPrice(price);
        UpdatedAt = DateTime.UtcNow;
    }

    // Catálogo (César): asigna descripción y categoría
    public void SetCatalogInfo(string? description, int? categoryId)
    {
        var normalizedDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

        if (normalizedDescription is not null && normalizedDescription.Length > 500)
        {
            throw new ArgumentOutOfRangeException(nameof(description), "La descripción no puede superar 500 caracteres.");
        }

        if (categoryId is <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(categoryId), "La categoría seleccionada no es válida.");
        }

        Description = normalizedDescription;
        CategoryId = categoryId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    private void SetName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var normalizedName = name.Trim();
        if (normalizedName.Length > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(name), "El nombre no puede superar 100 caracteres.");
        }

        Name = normalizedName;
    }

    private void SetPrice(decimal price)
    {
        if (price <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price), "El precio debe ser mayor que cero.");
        }

        if (decimal.Round(price, 2) != price)
        {
            throw new ArgumentOutOfRangeException(nameof(price), "El precio no puede tener más de dos decimales.");
        }

        Price = price;
    }
}