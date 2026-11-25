using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SmartRestaurant.Application.DTOs.Inventory;
using SmartRestaurant.Application.Interfaces.Services;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Domain.Exceptions;

namespace SmartRestaurant.Web.Controllers;

public class InventoryController : Controller
{
    private readonly IInventoryService _inventoryService;
    private readonly IInventoryMovementService _movementService;

    public InventoryController(IInventoryService inventoryService, IInventoryMovementService movementService)
    {
        _inventoryService = inventoryService;
        _movementService = movementService;
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

    // MOVIMIENTO (mostrar formulario): GET /Inventory/Movement  o  /Inventory/Movement/5
    // Con id, el insumo ya viene seleccionado. Sin id, se elige de la lista.
    [HttpGet]
    public async Task<IActionResult> Movement(int? id, CancellationToken ct)
    {
        var model = new RegisterInventoryMovementDto();

        if (id.HasValue)
        {
            var item = await _inventoryService.GetByIdAsync(id.Value, ct);
            if (item is null)
                return NotFound();

            model.InventoryItemId = id.Value;
        }

        await LoadMovementFormDataAsync(ct);
        return View(model);
    }

    // MOVIMIENTO (guardar entrada o salida): POST /Inventory/Movement
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Movement(RegisterInventoryMovementDto model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await LoadMovementFormDataAsync(ct);
            return View(model);
        }

        try
        {
            await _movementService.RegisterAsync(model, ct);
            TempData["Success"] = model.Type == InventoryMovementType.Entry
                ? "Entrada registrada correctamente."
                : "Salida registrada correctamente.";
            return RedirectToAction(nameof(Details), new { id = model.InventoryItemId });
        }
        catch (ValidationException ex)
        {
            AddValidationErrors(ex);
        }
        catch (DomainException ex)
        {
            // Reglas del negocio: stock insuficiente o insumo inactivo
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        await LoadMovementFormDataAsync(ct);
        return View(model);
    }

    // HISTORIAL: GET /Inventory/History?inventoryItemId=5&type=Exit&from=2026-11-01&to=2026-11-30
    // Todos los filtros son opcionales; sin filtros muestra todos los movimientos.
    [HttpGet]
    public async Task<IActionResult> History(int? inventoryItemId, InventoryMovementType? type, DateTime? from, DateTime? to, CancellationToken ct)
    {
        ViewData["InventoryItemId"] = inventoryItemId;
        ViewData["Type"] = type;
        ViewData["From"] = from;
        ViewData["To"] = to;

        // Lista de insumos para el filtro (incluye inactivos, porque también tienen historial)
        ViewData["Items"] = await _inventoryService.GetAllAsync(null, true, ct);

        var movements = await _movementService.GetHistoryAsync(inventoryItemId, type, from, to, ct);
        return View(movements);
    }

    // Insumos activos para el selector del formulario de movimiento
    // (los inactivos no admiten movimientos)
    private async Task LoadMovementFormDataAsync(CancellationToken ct)
    {
        ViewData["Items"] = await _inventoryService.GetAllAsync(null, false, ct);
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