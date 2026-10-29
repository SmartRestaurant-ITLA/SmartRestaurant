using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SmartRestaurant.Application.DTOs.Inventory;
using SmartRestaurant.Application.Interfaces.Services;

namespace SmartRestaurant.Web.Controllers;

public class InventoryController : Controller
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    // LISTAR: GET /Inventory?search=pollo
    [HttpGet]
    public async Task<IActionResult> Index(string? search, CancellationToken ct)
    {
        ViewData["Search"] = search;
        var items = await _inventoryService.GetAllAsync(search, false, ct);
        return View(items);
    }

    // CONSULTAR: GET /Inventory/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var item = await _inventoryService.GetByIdAsync(id, ct);
        if (item is null)
            return NotFound();

        return View(item);
    }

    // CREAR (mostrar formulario): GET /Inventory/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateInventoryItemDto());
    }

    // CREAR (guardar): POST /Inventory/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateInventoryItemDto model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var id = await _inventoryService.CreateAsync(model, ct);
            TempData["Success"] = "Insumo registrado correctamente.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (ValidationException ex)
        {
            AddValidationErrors(ex);
            return View(model);
        }
    }

    // EDITAR (mostrar formulario): GET /Inventory/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var model = await _inventoryService.GetForEditAsync(id, ct);
        if (model is null)
            return NotFound();

        return View(model);
    }

    // EDITAR (guardar): POST /Inventory/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateInventoryItemDto model, CancellationToken ct)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var updated = await _inventoryService.UpdateAsync(model, ct);
            if (!updated)
                return NotFound();

            TempData["Success"] = "Insumo actualizado correctamente.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (ValidationException ex)
        {
            AddValidationErrors(ex);
            return View(model);
        }
    }

    // Pasa los errores de FluentValidation al formulario
    private void AddValidationErrors(ValidationException ex)
    {
        foreach (var error in ex.Errors)
        {
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
    }
}