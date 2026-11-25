using SmartRestaurant.Domain.Enums;

namespace SmartRestaurant.Application.DTOs.Billing;

public class RegisterPaymentDto
{
    public int InvoiceId { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; } = DateTime.Now;

    public PaymentMethod Method { get; set; }

    public string? Reference { get; set; }
}
