using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using SmartRestaurant.Application.DTOs.Sales;
using SmartRestaurant.Application.Service;
using SmartRestaurant.Application.Validators.Inventory;
using SmartRestaurant.Application.Validators.Sales;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.UnitTests.Fakes;

namespace SmartRestaurant.UnitTests.Services;

public class DashboardServiceTests
{
    private readonly FakeInventoryItemRepository _repository = new();
    private readonly FakeDailySaleRepository _saleRepository = new();
    private readonly DailySaleService _saleService;
    private readonly DashboardService _service;

    public DashboardServiceTests()
    {
        var config = new MapperConfiguration(
            cfg => cfg.AddMaps(typeof(SmartRestaurant.Application.ServiceRegistration).Assembly),
            NullLoggerFactory.Instance);
        var mapper = config.CreateMapper();

        var inventoryService = new InventoryService(
            _repository,
            mapper,
            new CreateInventoryItemDtoValidator(),
            new UpdateInventoryItemDtoValidator());

        _saleService = new DailySaleService(_saleRepository, mapper, new CreateDailySaleDtoValidator());

        _service = new DashboardService(inventoryService, _saleService);
    }

    private Task AddItem(string name, decimal stock, decimal minimum, DateTime? expiration = null) =>
        _repository.AddAsync(new InventoryItem
        {
            Name = name,
            Category = InventoryCategory.Grains,
            Unit = UnitOfMeasure.Pound,
            CurrentStock = stock,
            MinimumStock = minimum,
            UnitCost = 10,
            ExpirationDate = expiration
        });

    private Task AddSale(string product, int quantity, int daysAgo = 0) =>
        _saleService.CreateAsync(new CreateDailySaleDto
        {
            SaleDate = DateTime.Today.AddDays(-daysAgo),
            ProductName = product,
            Quantity = quantity
        });

    // ---------- Inventario ----------

    [Fact]
    public async Task SinInsumos_DevuelveTodoEnCeroYSinAlertas()
    {
        var summary = await _service.GetSummaryAsync();

        Assert.Equal(0, summary.Inventory.TotalItems);
        Assert.Equal(0, summary.Inventory.LowStockCount);
        Assert.Empty(summary.Inventory.AlertItems);
    }

    [Fact]
    public async Task CuentaStockBajoPorVencerYVencidos()
    {
        await AddItem("Arroz", stock: 5, minimum: 10);                                   // stock bajo
        await AddItem("Leche", stock: 50, minimum: 10, DateTime.Today.AddDays(-1));      // vencido
        await AddItem("Pollo", stock: 50, minimum: 10, DateTime.Today.AddDays(2));       // por vencer
        await AddItem("Sal", stock: 50, minimum: 10);                                    // normal

        var summary = await _service.GetSummaryAsync();

        Assert.Equal(4, summary.Inventory.TotalItems);
        Assert.Equal(1, summary.Inventory.LowStockCount);
        Assert.Equal(1, summary.Inventory.NearExpirationCount);
        Assert.Equal(1, summary.Inventory.ExpiredCount);
        Assert.Equal(3, summary.Inventory.AlertItems.Count);
    }

    [Fact]
    public async Task LasAlertas_SeLimitanACinco()
    {
        for (var i = 1; i <= 7; i++)
            await AddItem($"Insumo {i}", stock: 1, minimum: 10);

        var summary = await _service.GetSummaryAsync();

        Assert.Equal(7, summary.Inventory.LowStockCount);
        Assert.Equal(5, summary.Inventory.AlertItems.Count);
    }

    // ---------- Ventas ----------

    [Fact]
    public async Task SinVentasHoy_DevuelveCeroYSinProductoMasVendido()
    {
        var summary = await _service.GetSummaryAsync();

        Assert.Equal(0, summary.Sales.TodayUnits);
        Assert.Equal(0, summary.Sales.TodayProductsCount);
        Assert.Null(summary.Sales.TopProductName);
    }

    [Fact]
    public async Task Ventas_SoloCuentaLasDeHoy()
    {
        await AddSale("Pizza", 3);
        await AddSale("Pasta", 5);
        await AddSale("Pizza", 10, daysAgo: 1);   // ayer: no debe contar

        var summary = await _service.GetSummaryAsync();

        Assert.Equal(8, summary.Sales.TodayUnits);
        Assert.Equal(2, summary.Sales.TodayProductsCount);
        Assert.Equal("Pasta", summary.Sales.TopProductName);
        Assert.Equal(5, summary.Sales.TopProductUnits);
    }

    [Fact]
    public async Task Ventas_EnCasoDeEmpate_ElMasVendidoEsElPrimeroPorNombre()
    {
        await AddSale("Pizza", 5);
        await AddSale("Pasta", 5);

        var summary = await _service.GetSummaryAsync();

        Assert.Equal("Pasta", summary.Sales.TopProductName);
    }
}