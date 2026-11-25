using SmartRestaurant.Domain.Enums;

namespace SmartRestaurant.Application.DTOs.Billing;

public class InvoiceDto
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;

    public DateTime IssuedAt { get; set; }

    public InvoiceStatus Status { get; set; }

    public decimal Total { get; set; }

    public decimal AmountPaid { get; set; }

    public decimal Balance { get; set; }

    public IReadOnlyList<InvoiceDetailDto> Details { get; set; } = [];

    public IReadOnlyList<PaymentDto> Payments { get; set; } = [];
}
