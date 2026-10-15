namespace SmartRestaurant.Application.DTOs.Sales;

public class DailySaleDto
{
    public int Id { get; set; }
    public DateTime SaleDate { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
}