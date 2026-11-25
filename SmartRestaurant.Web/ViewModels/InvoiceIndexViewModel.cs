using SmartRestaurant.Application.DTOs.Billing;

namespace SmartRestaurant.Web.ViewModels;

public class InvoiceIndexViewModel
{
    public IReadOnlyList<InvoiceDto> Invoices { get; set; } = [];
}
