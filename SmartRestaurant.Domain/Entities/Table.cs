namespace SmartRestaurant.Domain.Entities;

public class Table
{
    public int Id { get; private set; }

    public int Number { get; private set; }

    public int Capacity { get; private set; }

    public bool IsActive { get; private set; }

    public Table(int number, int capacity, bool isActive = true)
    {
        if (number <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(number),
                "El número debe ser mayor que cero.");
        }

        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(capacity),
                "La capacidad debe ser mayor que cero.");
        }

        Number = number;
        Id = number;
        Capacity = capacity;
        IsActive = isActive;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}