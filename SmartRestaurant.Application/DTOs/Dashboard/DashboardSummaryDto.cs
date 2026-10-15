using SmartRestaurant.Application.DTOs.Inventory;

namespace SmartRestaurant.Application.DTOs.Dashboard;

public class DashboardSummaryDto
{
    public InventoryIndicatorsDto Inventory { get; set; } = new();
}

public class InventoryIndicatorsDto
{
    public int TotalItems { get; set; }
    public int LowStockCount { get; set; }
    public int NearExpirationCount { get; set; }
    public int ExpiredCount { get; set; }

    // Los insumos que requieren atención (máximo 5)
    public IReadOnlyList<InventoryItemDto> AlertItems { get; set; } = new List<InventoryItemDto>();
}