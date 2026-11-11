using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartRestaurant.Application.Interfaces.Services;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Exceptions;
using SmartRestaurant.Web.ViewModels;

namespace SmartRestaurant.Web.Controllers;

public class ProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public ProductsController(IProductService productService, ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    // LISTAR
    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetAllAsync();
        var categoryNames = await GetCategoryNamesAsync();

        var model = products.Select(p => ToListItem(p, categoryNames)).ToList();
        return View(model);
    }

    // CONSULTAR
    public async Task<IActionResult> Details(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product is null) return NotFound();

        var categoryNames = await GetCategoryNamesAsync();
        return View(ToListItem(product, categoryNames));
    }

    // CREAR (formulario)
    public async Task<IActionResult> Create()
    {
        var model = new ProductFormViewModel();
        await LoadCategoriesAsync(model);
        return View(model);
    }

    // CREAR (guardar)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategoriesAsync(model);
            return View(model);
        }

        try
        {
            await _productService.CreateAsync(model.Name, model.Description, model.Price, model.CategoryId);
            TempData["Success"] = "Producto creado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadCategoriesAsync(model);
            return View(model);
        }
    }

    // EDITAR (formulario)
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product is null) return NotFound();

        var model = new ProductFormViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CategoryId = product.CategoryId
        };

        await LoadCategoriesAsync(model);
        return View(model);
    }

    // EDITAR (guardar)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductFormViewModel model)
    {
        if (id != model.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            await LoadCategoriesAsync(model);
            return View(model);
        }

        try
        {
            var updated = await _productService.UpdateAsync(id, model.Name, model.Description, model.Price, model.CategoryId);
            if (!updated) return NotFound();

            TempData["Success"] = "Producto actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadCategoriesAsync(model);
            return View(model);
        }
    }

    // ACTIVAR / DESACTIVAR
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product is null) return NotFound();

        await _productService.SetActiveAsync(id, !product.IsActive);
        TempData["Success"] = product.IsActive ? "Producto desactivado." : "Producto activado.";
        return RedirectToAction(nameof(Index));
    }

    // ---- Ayudantes (solo preparan datos para la vista) ----

    private async Task LoadCategoriesAsync(ProductFormViewModel model)
    {
        var categories = await _categoryService.GetAllAsync();

        model.Categories = categories
            .Where(c => c.IsActive)
            .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
            .ToList();
    }

    private async Task<Dictionary<int, string>> GetCategoryNamesAsync()
    {
        var categories = await _categoryService.GetAllAsync();
        return categories.ToDictionary(c => c.Id, c => c.Name);
    }

    private static ProductListItemViewModel ToListItem(Product product, Dictionary<int, string> categoryNames)
    {
        return new ProductListItemViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CategoryName = categoryNames.TryGetValue(product.CategoryId, out var name) ? name : "—",
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}