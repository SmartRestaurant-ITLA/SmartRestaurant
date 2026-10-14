using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Exceptions;

namespace SmartRestaurant.UnitTests.Entities;

public class InventoryItemTests
{
    // Crea un insumo de prueba
    private static InventoryItem CreateItem(decimal stock = 10, decimal minimum = 2) => new()
    {
        Name = "Arroz",
        CurrentStock = stock,
        MinimumStock = minimum
    };

    [Fact]
    public void AddStock_SumaLaCantidad()
    {
        // Arrange
        var item = CreateItem(stock: 10);

        // Act
        item.AddStock(5);

        // Assert
        Assert.Equal(15, item.CurrentStock);
    }

    [Fact]
    public void AddStock_ConCantidadCeroONegativa_LanzaExcepcion()
    {
        var item = CreateItem();

        Assert.Throws<DomainException>(() => item.AddStock(0));
        Assert.Throws<DomainException>(() => item.AddStock(-3));
    }

    [Fact]
    public void RemoveStock_DescuentaLaCantidad()
    {
        var item = CreateItem(stock: 10);

        item.RemoveStock(4);

        Assert.Equal(6, item.CurrentStock);
    }

    [Fact]
    public void RemoveStock_MayorQueElDisponible_LanzaExcepcionYNoCambiaElStock()
    {
        var item = CreateItem(stock: 3);

        Assert.Throws<DomainException>(() => item.RemoveStock(5));
        Assert.Equal(3, item.CurrentStock);
    }

    [Theory]
    [InlineData(2, 2, true)]   // igual al mínimo → stock bajo
    [InlineData(1, 2, true)]   // por debajo del mínimo → stock bajo
    [InlineData(5, 2, false)]  // por encima del mínimo → normal
    public void IsLowStock_ComparaContraElMinimo(decimal stock, decimal minimum, bool expected)
    {
        var item = CreateItem(stock, minimum);

        Assert.Equal(expected, item.IsLowStock);
    }

    [Fact]
    public void IsNearExpiration_CuandoVenceEnDosDias_EsVerdadero()
    {
        var today = new DateTime(2026, 10, 13);
        var item = CreateItem();
        item.ExpirationDate = today.AddDays(2);

        Assert.True(item.IsNearExpiration(today));
        Assert.False(item.IsExpired(today));
    }

    [Fact]
    public void IsExpired_CuandoLaFechaYaPaso_EsVerdadero()
    {
        var today = new DateTime(2026, 10, 13);
        var item = CreateItem();
        item.ExpirationDate = today.AddDays(-1);

        Assert.True(item.IsExpired(today));
    }
}