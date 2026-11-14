using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartRestaurant.Application.DTOs.Products;
using SmartRestaurant.Application.Interfaces.Services;
using SmartRestaurant.Web.ViewModels;

namespace SmartRestaurant.Web.Controllers;

public class ProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;   // Catálogo (César)

    public ProductsController(IProductService productService, ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var products = await _productService.GetAllAsync(includeInactive: true, ct);
        return View(new ProductIndexViewModel { Products = products });
    }

    // Catálogo (César): consultar un producto
    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var product = await _productService.GetByIdAsync(id, ct);
        return product is null ? NotFound() : View(product);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadCategoriesAsync();
        return View(new CreateProductDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProductDto model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategoriesAsync();
            return View(model);
        }

        try
        {
            await _productService.CreateAsync(model, ct);
            TempData["Success"] = "Producto registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (ValidationException exception)
        {
            AddValidationErrors(exception);
            await LoadCategoriesAsync();
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var model = await _productService.GetForEditAsync(id, ct);
        if (model is null)
        {
            return NotFound();
        }

        await LoadCategoriesAsync();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateProductDto model, CancellationToken ct)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await LoadCategoriesAsync();
            return View(model);
        }

        try
        {
            if (!await _productService.UpdateAsync(model, ct))
            {
                return NotFound();
            }

            TempData["Success"] = "Producto actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (ValidationException exception)
        {
            AddValidationErrors(exception);
            await LoadCategoriesAsync();
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetActive(int id, bool isActive, CancellationToken ct)
    {
        if (!await _productService.SetActiveAsync(id, isActive, ct))
        {
            return NotFound();
        }

        TempData["Success"] = isActive
            ? "Producto activado correctamente."
            : "Producto desactivado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    // Catálogo (César): categorías activas para el desplegable
    private async Task LoadCategoriesAsync()
    {
        var categories = await _categoryService.GetAllAsync();
        ViewData["Categories"] = categories
            .Where(category => category.IsActive)
            .Select(category => new SelectListItem(category.Name, category.Id.ToString()))
            .ToList();
    }

    private void AddValidationErrors(ValidationException exception)
    {
        foreach (var error in exception.Errors)
        {
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
    }
}