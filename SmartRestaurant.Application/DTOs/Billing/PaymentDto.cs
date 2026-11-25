using SmartRestaurant.Domain.Enums;

namespace SmartRestaurant.Application.DTOs.Billing;

public class PaymentDto
{
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public PaymentMethod Method { get; set; }

    public string? Reference { get; set; }
}
