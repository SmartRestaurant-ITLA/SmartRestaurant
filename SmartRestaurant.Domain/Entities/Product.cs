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

    public void Update(string name, decimal price)
    {
        SetName(name);
        SetPrice(price);
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
