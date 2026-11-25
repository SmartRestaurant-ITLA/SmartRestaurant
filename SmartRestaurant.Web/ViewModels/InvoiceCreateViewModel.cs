using System.ComponentModel.DataAnnotations;
using SmartRestaurant.Application.DTOs.Orders;

namespace SmartRestaurant.Web.ViewModels;

public class InvoiceCreateViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Selecciona un pedido.")]
    public int OrderId { get; set; }

    public IReadOnlyList<OrderListItemDto> EligibleOrders { get; set; } = [];
}
