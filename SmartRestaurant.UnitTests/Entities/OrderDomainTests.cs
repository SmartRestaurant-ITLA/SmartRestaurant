using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;

namespace SmartRestaurant.UnitTests.Entities;

public class OrderDomainTests
{
    [Fact]
    public void Constructor_CalculaElTotalEnBaseALosDetalles()
    {
        var order = new Order(3,
        [
            new OrderDetail(1, "Hamburguesa", 250m, 2),
            new OrderDetail(2, "Jugo", 75m, 1)
        ]);

        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Equal(575m, order.Total);
        Assert.Equal(500m, order.Details[0].Subtotal);
    }

    [Fact]
    public void Constructor_SinDetalles_Falla()
    {
        Assert.Throws<ArgumentException>(() => new Order(3, []));
    }

    [Theory]
    [InlineData(OrderStatus.Pending, OrderStatus.InPreparation)]
    [InlineData(OrderStatus.Pending, OrderStatus.Cancelled)]
    [InlineData(OrderStatus.InPreparation, OrderStatus.Served)]
    [InlineData(OrderStatus.InPreparation, OrderStatus.Cancelled)]
    public void ChangeStatus_AceptaTransicionesPermitidas(OrderStatus initial, OrderStatus next)
    {
        var order = CreateOrder();
        if (initial == OrderStatus.InPreparation)
        {
            order.ChangeStatus(OrderStatus.InPreparation);
        }

        order.ChangeStatus(next);

        Assert.Equal(next, order.Status);
    }

    [Theory]
    [InlineData(OrderStatus.Pending, OrderStatus.Served)]
    [InlineData(OrderStatus.Served, OrderStatus.Cancelled)]
    [InlineData(OrderStatus.Cancelled, OrderStatus.InPreparation)]
    public void ChangeStatus_RechazaTransicionesNoPermitidas(OrderStatus initial, OrderStatus next)
    {
        var order = CreateOrder();
        if (initial != OrderStatus.Pending)
        {
            order.ChangeStatus(OrderStatus.InPreparation);
            order.ChangeStatus(initial);
        }

        Assert.Throws<InvalidOperationException>(() => order.ChangeStatus(next));
    }

    [Fact]
    public void OrderDetail_RechazaCantidadInvalida()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new OrderDetail(1, "Hamburguesa", 250m, 0));
    }

    [Fact]
    public void Product_RechazaPrecioNoPositivo()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Product("Hamburguesa", 0m));
    }

    private static Order CreateOrder() =>
        new(3, [new OrderDetail(1, "Hamburguesa", 250m, 1)]);
}
