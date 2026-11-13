using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SmartRestaurant.Application.DTOs.Tables;
using SmartRestaurant.Application.Interfaces.Services;
using SmartRestaurant.Web.ViewModels;

namespace SmartRestaurant.Web.Controllers;

public class TablesController : Controller
{
    private readonly ITableService _tableService;

    public TablesController(ITableService tableService)
    {
        _tableService = tableService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var tables = await _tableService.GetAllAsync(ct);
        var model = tables
            .Select(table => new TableListItemViewModel
            {
                Id = table.Id,
                Number = table.Number,
                Capacity = table.Capacity,
                IsActive = table.IsActive
            })
            .ToArray();

        return View(model);
    }

    [HttpGet]
    public IActionResult Create() => View(new CreateTableViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTableViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _tableService.CreateAsync(
                new CreateTableDto
                {
                    Number = model.Number,
                    Capacity = model.Capacity,
                    IsActive = model.IsActive
                },
                ct);

            TempData["Success"] = $"Mesa {model.Number} registrada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (ValidationException exception)
        {
            AddValidationErrors(exception);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var table = await _tableService.GetByIdAsync(id, ct);
        if (table is null)
        {
            return NotFound();
        }

        return View(new EditTableViewModel
        {
            Id = table.Id,
            Number = table.Number,
            Capacity = table.Capacity,
            IsActive = table.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditTableViewModel model, CancellationToken ct)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        var currentTable = await _tableService.GetByIdAsync(id, ct);
        if (currentTable is null)
        {
            return NotFound();
        }

        model.Number = currentTable.Number;
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            if (!await _tableService.UpdateAsync(
                    new UpdateTableDto
                    {
                        Id = model.Id,
                        Capacity = model.Capacity,
                        IsActive = model.IsActive
                    },
                    ct))
            {
                return NotFound();
            }

            TempData["Success"] = $"Mesa {currentTable.Number} actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (ValidationException exception)
        {
            AddValidationErrors(exception);
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id, CancellationToken ct)
    {
        var isActive = await _tableService.ToggleActiveAsync(id, ct);
        if (isActive is null)
        {
            return NotFound();
        }

        TempData["Success"] = isActive.Value
            ? "Mesa activada correctamente."
            : "Mesa desactivada correctamente.";

        return RedirectToAction(nameof(Index));
    }

    private void AddValidationErrors(ValidationException exception)
    {
        foreach (var error in exception.Errors)
        {
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
    }
}
