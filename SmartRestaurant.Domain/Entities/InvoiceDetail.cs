namespace SmartRestaurant.Domain.Entities;

public class InvoiceDetail
{
    private InvoiceDetail()
    {
    }

    public InvoiceDetail(string productName, decimal unitPrice, int quantity)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(productName);
        var normalizedName = productName.Trim();
        if (normalizedName.Length > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(productName), "El nombre no puede superar 100 caracteres.");
        }

        if (unitPrice <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(unitPrice), "El precio debe ser mayor que cero.");
        }

        if (decimal.Round(unitPrice, 2) != unitPrice)
        {
            throw new ArgumentOutOfRangeException(nameof(unitPrice), "El precio no puede tener más de dos decimales.");
        }

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "La cantidad debe ser mayor que cero.");
        }

        ProductName = normalizedName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public int Id { get; private set; }

    public int InvoiceId { get; private set; }

    public string ProductName { get; private set; } = string.Empty;

    public decimal UnitPrice { get; private set; }

    public int Quantity { get; private set; }

    public decimal Subtotal => UnitPrice * Quantity;
}
