using SmartRestaurant.Domain.Common;
using SmartRestaurant.Domain.Enums;

namespace SmartRestaurant.Domain.Entities;

public class Invoice : BaseEntity
{
    private List<InvoiceDetail> _details = [];
    private List<Payment> _payments = [];

    private Invoice()
    {
    }

    public Invoice(int orderId, string invoiceNumber, DateTime issuedAt, IEnumerable<InvoiceDetail> details)
    {
        if (orderId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(orderId), "El pedido debe ser válido.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(invoiceNumber);
        var normalizedInvoiceNumber = invoiceNumber.Trim();
        if (normalizedInvoiceNumber.Length > 50)
        {
            throw new ArgumentOutOfRangeException(nameof(invoiceNumber), "El número de factura no puede superar 50 caracteres.");
        }

        if (issuedAt == default)
        {
            throw new ArgumentOutOfRangeException(nameof(issuedAt), "La fecha de emisión debe ser válida.");
        }

        ArgumentNullException.ThrowIfNull(details);
        var invoiceDetails = details.ToList();
        if (invoiceDetails.Count == 0)
        {
            throw new ArgumentException("La factura debe contener al menos un detalle.", nameof(details));
        }

        if (invoiceDetails.Any(detail => detail is null))
        {
            throw new ArgumentException("Los detalles de la factura no pueden ser nulos.", nameof(details));
        }

        OrderId = orderId;
        InvoiceNumber = normalizedInvoiceNumber;
        IssuedAt = issuedAt;
        Status = InvoiceStatus.Issued;
        _details.AddRange(invoiceDetails);
    }

    public int OrderId { get; private set; }

    public string InvoiceNumber { get; private set; } = string.Empty;

    public DateTime IssuedAt { get; private set; }

    public InvoiceStatus Status { get; private set; }

    public IReadOnlyCollection<InvoiceDetail> Details => _details.AsReadOnly();

    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

    public decimal Total => _details.Sum(detail => detail.Subtotal);

    public decimal AmountPaid => _payments.Sum(payment => payment.Amount);

    public decimal Balance => Total - AmountPaid;

    public void RegisterPayment(Payment payment)
    {
        ArgumentNullException.ThrowIfNull(payment);

        if (Status is InvoiceStatus.Paid or InvoiceStatus.Cancelled)
        {
            throw new InvalidOperationException("No se pueden registrar pagos en una factura pagada o cancelada.");
        }

        if (payment.InvoiceId != Id)
        {
            throw new ArgumentException("El pago debe pertenecer a esta factura.", nameof(payment));
        }

        if (AmountPaid + payment.Amount > Total)
        {
            throw new InvalidOperationException("El importe del pago supera el saldo pendiente de la factura.");
        }

        _payments.Add(payment);
        Status = Balance == 0 ? InvoiceStatus.Paid : InvoiceStatus.PartiallyPaid;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == InvoiceStatus.Cancelled)
        {
            return;
        }

        if (_payments.Count > 0)
        {
            throw new InvalidOperationException("No se puede cancelar una factura que ya tiene pagos registrados.");
        }

        Status = InvoiceStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
}
