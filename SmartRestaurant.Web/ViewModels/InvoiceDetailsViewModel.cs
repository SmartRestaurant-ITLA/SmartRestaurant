using SmartRestaurant.Application.DTOs.Billing;

namespace SmartRestaurant.Web.ViewModels;

public class InvoiceDetailsViewModel
{
    public InvoiceDto Invoice { get; set; } = new();

    public RegisterPaymentDto Payment { get; set; } = new();
}
