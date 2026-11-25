using SmartRestaurant.Application.DTOs.Orders;
using SmartRestaurant.Application.DTOs.Products;
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

    public IReadOnlyList<ProductDto> AvailableProducts { get; set; } = [];

    public bool CanEditItems => Status == OrderStatus.Pending;
}

/*Descripción: agrega la lista de productos activos (para el selector de “agregar producto”) y una propiedad que indica si el pedido se puede editar (solo Pending).*/
