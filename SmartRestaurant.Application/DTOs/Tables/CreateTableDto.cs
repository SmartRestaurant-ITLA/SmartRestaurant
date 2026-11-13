namespace SmartRestaurant.Application.DTOs.Tables;

public class CreateTableDto
{
    public int Number { get; set; }

    public int Capacity { get; set; }

    public bool IsActive { get; set; }
}
