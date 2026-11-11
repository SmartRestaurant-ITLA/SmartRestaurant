using Microsoft.AspNetCore.Mvc;
using SmartRestaurant.Application.Interfaces.Services;
using SmartRestaurant.Domain.Exceptions;
using SmartRestaurant.Web.ViewModels;

namespace SmartRestaurant.Web.Controllers;

public class CategoriesController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    // LISTAR
    public async Task<IActionResult> Index()
    {
        var categories = await _categoryService.GetAllAsync();
        return View(categories);
    }

    // CONSULTAR
    public async Task<IActionResult> Details(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category is null) return NotFound();
        return View(category);
    }

    // CREAR (formulario)
    public IActionResult Create()
    {
        return View(new CategoryFormViewModel());
    }

    // CREAR (guardar)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        try
        {
            await _categoryService.CreateAsync(model.Name);
            TempData["Success"] = "Categoría creada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    // EDITAR (formulario)
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category is null) return NotFound();

        return View(new CategoryFormViewModel { Id = category.Id, Name = category.Name });
    }

    // EDITAR (guardar)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryFormViewModel model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);

        try
        {
            var updated = await _categoryService.UpdateAsync(id, model.Name);
            if (!updated) return NotFound();

            TempData["Success"] = "Categoría actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    // ACTIVAR / DESACTIVAR
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category is null) return NotFound();

        await _categoryService.SetActiveAsync(id, !category.IsActive);
        TempData["Success"] = category.IsActive ? "Categoría desactivada." : "Categoría activada.";
        return RedirectToAction(nameof(Index));
    }
}