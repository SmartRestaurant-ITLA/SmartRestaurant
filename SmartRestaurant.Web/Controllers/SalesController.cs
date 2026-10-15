using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SmartRestaurant.Application.DTOs.Sales;
using SmartRestaurant.Application.Interfaces;
using SmartRestaurant.Web.Models;

namespace SmartRestaurant.Web.Controllers;

public class SalesController : Controller
{
    private readonly IDailySaleService _saleService;

    public SalesController(IDailySaleService saleService)
    {
        _saleService = saleService;
    }

    // LISTAR: GET /Sales?date=2026-10-15
    [HttpGet]
    public async Task<IActionResult> Index(DateTime? date, CancellationToken ct)
    {
        var sales = await _saleService.GetAllAsync(date, ct);

        return View(new SalesIndexViewModel
        {
            Date = date?.Date,
            Sales = sales
        });
    }

    // REGISTRAR (mostrar formulario): GET /Sales/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateDailySaleDto());
    }

    // REGISTRAR (guardar): POST /Sales/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateDailySaleDto model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _saleService.CreateAsync(model, ct);
            TempData["Success"] = "Venta registrada correctamente.";
            return RedirectToAction(nameof(Index), new { date = model.SaleDate.ToString("yyyy-MM-dd") });
        }
        catch (ValidationException ex)
        {
            foreach (var error in ex.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            return View(model);
        }
    }
}