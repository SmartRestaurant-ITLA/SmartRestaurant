using SmartRestaurant.Application.DTOs.Sales;

namespace SmartRestaurant.Web.Models;

public class SalesIndexViewModel
{
    public DateTime? Date { get; set; }
    public IReadOnlyList<DailySaleDto> Sales { get; set; } = new List<DailySaleDto>();
    public int TotalUnits => Sales.Sum(s => s.Quantity);
}