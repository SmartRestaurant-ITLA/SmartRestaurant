using SmartRestaurant.Application.DTOs.Inventory;

namespace SmartRestaurant.Application.DTOs.Dashboard;

public class DashboardSummaryDto
{
    public InventoryIndicatorsDto Inventory { get; set; } = new();
    public SalesIndicatorsDto Sales { get; set; } = new();
}

public class InventoryIndicatorsDto
{
    public int TotalItems { get; set; }
    public int LowStockCount { get; set; }
    public int NearExpirationCount { get; set; }
    public int ExpiredCount { get; set; }

    // Los insumos que requieren atencion (maximo 5)
    public IReadOnlyList<InventoryItemDto> AlertItems { get; set; } = new List<InventoryItemDto>();
}

public class SalesIndicatorsDto
{
    public int TodayUnits { get; set; }
    public int TodayProductsCount { get; set; }
    public string? TopProductName { get; set; }
    public int TopProductUnits { get; set; }
}