using SmartRestaurant.Application.DTOs.Dashboard;

namespace SmartRestaurant.Web.Models;

public class DashboardViewModel
{
    public InventoryIndicatorsDto Inventory { get; set; } = new();
    public SalesIndicatorsDto Sales { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
}