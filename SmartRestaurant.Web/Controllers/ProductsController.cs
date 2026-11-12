using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SmartRestaurant.Application.DTOs.Products;
using SmartRestaurant.Application.Interfaces.Services;
using SmartRestaurant.Web.ViewModels;

namespace SmartRestaurant.Web.Controllers;

public class ProductsController : Controller
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var products = await _productService.GetAllAsync(includeInactive: true, ct);
        return View(new ProductIndexViewModel { Products = products });
    }

    [HttpGet]
    public IActionResult Create() => View(new CreateProductDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProductDto model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
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
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var model = await _productService.GetForEditAsync(id, ct);
        return model is null ? NotFound() : View(model);
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

    private void AddValidationErrors(ValidationException exception)
    {
        foreach (var error in exception.Errors)
        {
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
    }
}
