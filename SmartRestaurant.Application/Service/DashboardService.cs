using SmartRestaurant.Application.DTOs.Dashboard;
using SmartRestaurant.Application.Interfaces;

namespace SmartRestaurant.Application.Service;

public class DashboardService : IDashboardService
{
    private const int MaxAlerts = 5;

    private readonly IInventoryService _inventoryService;

    public DashboardService(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken ct = default)
    {
        // Solo insumos activos
        var items = await _inventoryService.GetAllAsync(null, false, ct);

        var alerts = items
            .Where(i => i.IsLowStock || i.IsExpired || i.IsNearExpiration)
            .OrderByDescending(i => i.IsExpired)
            .ThenByDescending(i => i.IsLowStock)
            .ThenBy(i => i.Name)
            .Take(MaxAlerts)
            .ToList();

        return new DashboardSummaryDto
        {
            Inventory = new InventoryIndicatorsDto
            {
                TotalItems = items.Count,
                LowStockCount = items.Count(i => i.IsLowStock),
                NearExpirationCount = items.Count(i => i.IsNearExpiration),
                ExpiredCount = items.Count(i => i.IsExpired),
                AlertItems = alerts
            }
        };
    }
}