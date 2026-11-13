using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartRestaurant.Application.DTOs.Orders;
using SmartRestaurant.Application.DTOs.Products;
using SmartRestaurant.Application.Interfaces;
using SmartRestaurant.Application.Interfaces.Services;
using SmartRestaurant.Application.Services;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Web.ViewModels;
/*Cambio 2: using nuevo*/
using SmartRestaurant.Application.DTOs.Products;

namespace SmartRestaurant.Web.Controllers;

public class OrdersController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IProductService _productService;
    private readonly ReservationService _reservationService;

    public OrdersController(
        IOrderService orderService,
        IProductService productService,
        ReservationService reservationService)
    {
        _orderService = orderService;
        _productService = productService;
        _reservationService = reservationService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var orders = await _orderService.GetAllAsync(ct);
        return View(new OrderIndexViewModel { Orders = orders });
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var model = new OrderCreateViewModel();
        await PopulateCreateOptionsAsync(model, ct);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OrderCreateViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCreateOptionsAsync(model, ct);
            return View(model);
        }

        try
        {
            var id = await _orderService.CreateAsync(new CreateOrderDto
            {
                TableId = model.TableId,
                Details = model.Details.Select(detail => new CreateOrderDetailDto
                {
                    ProductId = detail.ProductId,
                    Quantity = detail.Quantity
                }).ToList()
            }, ct);

            TempData["Success"] = "Pedido registrado correctamente.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (ValidationException exception)
        {
            AddValidationErrors(exception);
            await PopulateCreateOptionsAsync(model, ct);
            return View(model);
        }
    }

    /*Cambio 1: acción Details
    Descripción: carga los productos activos solo cuando el pedido es editable.*/
    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var order = await _orderService.GetByIdAsync(id, ct);
        if (order is null)
        {
            return NotFound();
        }

        IReadOnlyList<ProductDto> availableProducts = [];
        if (order.Status == OrderStatus.Pending)
        {
            availableProducts = await _productService.GetAllAsync(ct: ct);
        }

        return View(new OrderDetailsViewModel
        {
            Id = order.Id,
            TableNumber = order.TableNumber,
            CreatedAt = order.CreatedAt,
            Status = order.Status,
            Total = order.Total,
            Details = order.Details,
            AvailableStatusTransitions = order.AvailableStatusTransitions,
            AvailableProducts = availableProducts
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(int id, OrderStatus status, CancellationToken ct)
    {
        try
        {
            if (!await _orderService.ChangeStatusAsync(id, status, ct))
            {
                return NotFound();
            }

            TempData["Success"] = $"El pedido cambió a {GetStatusLabel(status)}.";
        }
        catch (ValidationException exception)
        {
            TempData["Error"] = string.Join(" ", exception.Errors.Select(error => error.ErrorMessage));
        }
        catch (InvalidOperationException exception)
        {
            TempData["Error"] = exception.Message;
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    /*Cambio 3: tres acciones nuevas y un método auxiliar
    Descripción: AddItem, UpdateItem y RemoveItem llaman al servicio y vuelven a Details. 
    Capturan ValidationException e InvalidOperationException, igual que ChangeStatus, y devuelven NotFound si el pedido no existe. 
    El auxiliar evita repetir el try/catch.*/

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> AddItem(int id, int productId, int quantity, CancellationToken ct) =>
    ExecuteItemActionAsync(
        id,
        () => _orderService.AddItemAsync(
            new AddOrderItemDto { OrderId = id, ProductId = productId, Quantity = quantity }, ct),
        "Producto agregado al pedido.");

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> UpdateItem(int id, int productId, int quantity, CancellationToken ct) =>
        ExecuteItemActionAsync(
            id,
            () => _orderService.UpdateItemQuantityAsync(
                new UpdateOrderItemDto { OrderId = id, ProductId = productId, Quantity = quantity }, ct),
            "Cantidad actualizada.");

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> RemoveItem(int id, int productId, CancellationToken ct) =>
        ExecuteItemActionAsync(
            id,
            () => _orderService.RemoveItemAsync(id, productId, ct),
            "Producto quitado del pedido.");

    private async Task<IActionResult> ExecuteItemActionAsync(
        int orderId,
        Func<Task<bool>> action,
        string successMessage)
    {
        try
        {
            if (!await action())
            {
                return NotFound();
            }

            TempData["Success"] = successMessage;
        }
        catch (ValidationException exception)
        {
            TempData["Error"] = string.Join(" ", exception.Errors.Select(error => error.ErrorMessage));
        }
        catch (InvalidOperationException exception)
        {
            TempData["Error"] = exception.Message;
        }

        return RedirectToAction(nameof(Details), new { id = orderId });
    }

    private async Task PopulateCreateOptionsAsync(OrderCreateViewModel model, CancellationToken ct)
    {
        model.AvailableTables = _reservationService.GetTables()
            .Where(table => table.IsActive)
            .OrderBy(table => table.Number)
            .Select(table => new SelectListItem($"Mesa {table.Number}", table.Id.ToString()))
            .ToList();

        model.AvailableProducts = await _productService.GetAllAsync(ct: ct);
    }

    private void AddValidationErrors(ValidationException exception)
    {
        foreach (var error in exception.Errors)
        {
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
    }

    private static string GetStatusLabel(OrderStatus status) =>
        status switch
        {
            OrderStatus.Pending => "Pendiente",
            OrderStatus.InPreparation => "En preparación",
            OrderStatus.Served => "Servido",
            OrderStatus.Cancelled => "Cancelado",
            _ => status.ToString()
        };
}
