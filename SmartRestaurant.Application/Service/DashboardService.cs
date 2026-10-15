using SmartRestaurant.Application.DTOs.Dashboard;
using SmartRestaurant.Application.Interfaces;

namespace SmartRestaurant.Application.Service;

public class DashboardService : IDashboardService
{
    private const int MaxAlerts = 5;

    private readonly IInventoryService _inventoryService;
    private readonly IDailySaleService _saleService;

    public DashboardService(IInventoryService inventoryService, IDailySaleService saleService)
    {
        _inventoryService = inventoryService;
        _saleService = saleService;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken ct = default)
    {
        // Las consultas se hacen una tras otra (no en paralelo): comparten el mismo DbContext
        var items = await _inventoryService.GetAllAsync(null, false, ct);
        var todaySales = await _saleService.GetAllAsync(DateTime.Today, ct);

        var alerts = items
            .Where(i => i.IsLowStock || i.IsExpired || i.IsNearExpiration)
            .OrderByDescending(i => i.IsExpired)
            .ThenByDescending(i => i.IsLowStock)
            .ThenBy(i => i.Name)
            .Take(MaxAlerts)
            .ToList();

        // Mas vendido: mayor cantidad; si hay empate, el primero por nombre
        var top = todaySales
            .OrderByDescending(s => s.Quantity)
            .ThenBy(s => s.ProductName)
            .FirstOrDefault();

        return new DashboardSummaryDto
        {
            Inventory = new InventoryIndicatorsDto
            {
                TotalItems = items.Count,
                LowStockCount = items.Count(i => i.IsLowStock),
                NearExpirationCount = items.Count(i => i.IsNearExpiration),
                ExpiredCount = items.Count(i => i.IsExpired),
                AlertItems = alerts
            },
            Sales = new SalesIndicatorsDto
            {
                TodayUnits = todaySales.Sum(s => s.Quantity),
                TodayProductsCount = todaySales.Count,
                TopProductName = top?.ProductName,
                TopProductUnits = top?.Quantity ?? 0
            }
        };
    }
}