using Microsoft.AspNetCore.Mvc;
using SmartRestaurant.Application.Interfaces;
using SmartRestaurant.Web.Models;

namespace SmartRestaurant.Web.Controllers;

public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    // GET /Dashboard
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var summary = await _dashboardService.GetSummaryAsync(ct);

        var model = new DashboardViewModel
        {
            Inventory = summary.Inventory,
            GeneratedAt = DateTime.Now
        };

        return View(model);
    }
}