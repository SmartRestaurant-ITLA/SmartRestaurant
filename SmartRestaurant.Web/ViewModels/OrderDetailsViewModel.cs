using SmartRestaurant.Application.DTOs.Orders;
using SmartRestaurant.Domain.Enums;

namespace SmartRestaurant.Web.ViewModels;

public class OrderDetailsViewModel
{
    public int Id { get; set; }

    public int TableNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public OrderStatus Status { get; set; }

    public decimal Total { get; set; }

    public IReadOnlyList<OrderDetailDto> Details { get; set; } = [];

    public IReadOnlyList<OrderStatus> AvailableStatusTransitions { get; set; } = [];
}
