using SmartRestaurant.Domain.Enums;

namespace SmartRestaurant.Domain.Entities;

public class Payment
{
    private Payment()
    {
    }

    public Payment(int invoiceId, decimal amount, DateTime paymentDate, PaymentMethod method, string? reference = null)
    {
        if (invoiceId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(invoiceId), "La factura debe ser válida.");
        }

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "El importe debe ser mayor que cero.");
        }

        if (decimal.Round(amount, 2) != amount)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "El importe no puede tener más de dos decimales.");
        }

        if (paymentDate == default)
        {
            throw new ArgumentOutOfRangeException(nameof(paymentDate), "La fecha de pago debe ser válida.");
        }

        if (!Enum.IsDefined(method))
        {
            throw new ArgumentOutOfRangeException(nameof(method), "El método de pago no es válido.");
        }

        if (reference?.Length > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(reference), "La referencia no puede superar 100 caracteres.");
        }

        InvoiceId = invoiceId;
        Amount = amount;
        PaymentDate = paymentDate;
        Method = method;
        Reference = string.IsNullOrWhiteSpace(reference) ? null : reference.Trim();
    }

    public int Id { get; private set; }

    public int InvoiceId { get; private set; }

    public decimal Amount { get; private set; }

    public DateTime PaymentDate { get; private set; }

    public PaymentMethod Method { get; private set; }

    public string? Reference { get; private set; }
}
