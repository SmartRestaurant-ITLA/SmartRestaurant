using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SmartRestaurant.Application.DTOs.Billing;
using SmartRestaurant.Application.Interfaces;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Web.ViewModels;

namespace SmartRestaurant.Web.Controllers;

public class InvoiceController : Controller
{
    private readonly IInvoiceService _invoiceService;
    private readonly IOrderService _orderService;

    public InvoiceController(IInvoiceService invoiceService, IOrderService orderService)
    {
        _invoiceService = invoiceService;
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var invoices = await _invoiceService.GetAllAsync(ct);
        return View(new InvoiceIndexViewModel { Invoices = invoices });
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var model = new InvoiceCreateViewModel();
        await PopulateEligibleOrdersAsync(model, ct);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(InvoiceCreateViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await PopulateEligibleOrdersAsync(model, ct);
            return View(model);
        }

        try
        {
            var id = await _invoiceService.CreateForOrderAsync(model.OrderId, ct);
            TempData["Success"] = "Factura registrada correctamente.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (ValidationException exception)
        {
            AddValidationErrors(exception);
            await PopulateEligibleOrdersAsync(model, ct);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var invoice = await _invoiceService.GetByIdAsync(id, ct);
        if (invoice is null)
        {
            return NotFound();
        }

        return View(CreateDetailsViewModel(invoice));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterPayment(
        int id,
        InvoiceDetailsViewModel model,
        CancellationToken ct)
    {
        model.Payment.InvoiceId = id;

        if (!ModelState.IsValid)
        {
            return await ReturnInvoiceDetailsAsync(id, model, ct);
        }

        try
        {
            if (!await _invoiceService.RegisterPaymentAsync(model.Payment, ct))
            {
                return NotFound();
            }

            TempData["Success"] = "Pago registrado correctamente.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (ValidationException exception)
        {
            AddValidationErrors(exception, "Payment");
            return await ReturnInvoiceDetailsAsync(id, model, ct);
        }
    }

    private async Task PopulateEligibleOrdersAsync(InvoiceCreateViewModel model, CancellationToken ct)
    {
        var orders = await _orderService.GetAllAsync(ct);
        var invoices = await _invoiceService.GetAllAsync(ct);
        var billedOrderIds = invoices.Select(invoice => invoice.OrderId).ToHashSet();

        model.EligibleOrders = orders
            .Where(order => order.Status == OrderStatus.Served && !billedOrderIds.Contains(order.Id))
            .OrderBy(order => order.Id)
            .ToArray();
    }

    private async Task<IActionResult> ReturnInvoiceDetailsAsync(
        int id,
        InvoiceDetailsViewModel model,
        CancellationToken ct)
    {
        var invoice = await _invoiceService.GetByIdAsync(id, ct);
        if (invoice is null)
        {
            return NotFound();
        }

        model.Invoice = invoice;
        return View(nameof(Details), model);
    }

    private void AddValidationErrors(ValidationException exception, string? prefix = null)
    {
        foreach (var error in exception.Errors)
        {
            var key = string.IsNullOrEmpty(prefix)
                ? error.PropertyName
                : $"{prefix}.{error.PropertyName}";
            ModelState.AddModelError(key, error.ErrorMessage);
        }
    }

    private static InvoiceDetailsViewModel CreateDetailsViewModel(InvoiceDto invoice) =>
        new()
        {
            Invoice = invoice,
            Payment = new RegisterPaymentDto
            {
                InvoiceId = invoice.Id,
                PaymentDate = DateTime.Now
            }
        };
}
