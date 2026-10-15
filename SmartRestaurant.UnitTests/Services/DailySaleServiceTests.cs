using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging.Abstractions;
using SmartRestaurant.Application.DTOs.Sales;
using SmartRestaurant.Application.Service;
using SmartRestaurant.Application.Validators.Sales;
using SmartRestaurant.UnitTests.Fakes;

namespace SmartRestaurant.UnitTests.Services;

public class DailySaleServiceTests
{
    private readonly FakeDailySaleRepository _repository = new();
    private readonly DailySaleService _service;

    public DailySaleServiceTests()
    {
        var config = new MapperConfiguration(
            cfg => cfg.AddMaps(typeof(SmartRestaurant.Application.ServiceRegistration).Assembly),
            NullLoggerFactory.Instance);

        _service = new DailySaleService(
            _repository,
            config.CreateMapper(),
            new CreateDailySaleDtoValidator());
    }

    private static CreateDailySaleDto NewSale(string product = "Pizza", int quantity = 3, DateTime? date = null) => new()
    {
        SaleDate = date ?? DateTime.Today,
        ProductName = product,
        Quantity = quantity
    };

    [Fact]
    public async Task Crear_ConDatosValidos_GuardaLaVenta()
    {
        var id = await _service.CreateAsync(NewSale());

        var saved = Assert.Single(_repository.Sales);
        Assert.Equal(id, saved.Id);
        Assert.Equal("Pizza", saved.ProductName);
        Assert.Equal(3, saved.Quantity);
    }

    [Fact]
    public async Task Crear_QuitaLosEspaciosDelNombre()
    {
        await _service.CreateAsync(NewSale("  Pizza  "));

        Assert.Equal("Pizza", Assert.Single(_repository.Sales).ProductName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task Crear_ConCantidadInvalida_Falla(int quantity)
    {
        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(NewSale(quantity: quantity)));
        Assert.Empty(_repository.Sales);
    }

    [Fact]
    public async Task Crear_SinProducto_Falla()
    {
        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(NewSale(product: "   ")));
    }

    [Fact]
    public async Task Crear_ConFechaFutura_Falla()
    {
        await Assert.ThrowsAsync<ValidationException>(
            () => _service.CreateAsync(NewSale(date: DateTime.Today.AddDays(1))));
    }

    [Fact]
    public async Task Crear_MismoProductoElMismoDia_Falla()
    {
        await _service.CreateAsync(NewSale("Pizza"));

        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(NewSale("pizza")));
        Assert.Single(_repository.Sales);
    }

    [Fact]
    public async Task Crear_MismoProductoEnOtroDia_Funciona()
    {
        await _service.CreateAsync(NewSale("Pizza"));
        await _service.CreateAsync(NewSale("Pizza", date: DateTime.Today.AddDays(-1)));

        Assert.Equal(2, _repository.Sales.Count);
    }

    [Fact]
    public async Task Listar_FiltraPorFecha()
    {
        await _service.CreateAsync(NewSale("Pizza"));
        await _service.CreateAsync(NewSale("Pasta", date: DateTime.Today.AddDays(-1)));

        var today = await _service.GetAllAsync(DateTime.Today);

        Assert.Equal("Pizza", Assert.Single(today).ProductName);
    }
}