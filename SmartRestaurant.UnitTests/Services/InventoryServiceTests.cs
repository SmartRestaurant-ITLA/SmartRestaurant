using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging.Abstractions;
using SmartRestaurant.Application.DTOs.Inventory;
using SmartRestaurant.Application.Services;
using SmartRestaurant.Application.Validators.Inventory;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.UnitTests.Fakes;

namespace SmartRestaurant.UnitTests.Services;

public class InventoryServiceTests
{
    private readonly FakeInventoryItemRepository _repository = new();
    private readonly InventoryService _service;

    public InventoryServiceTests()
    {
        // AutoMapper con los mismos Profiles de la aplicación
        var config = new MapperConfiguration(
           cfg => cfg.AddMaps(typeof(SmartRestaurant.Application.ServiceRegistration).Assembly),
            NullLoggerFactory.Instance);

        var mapper = config.CreateMapper();

        _service = new InventoryService(
            _repository,
            mapper,
            new CreateInventoryItemDtoValidator(),
            new UpdateInventoryItemDtoValidator());
    }

    private static CreateInventoryItemDto NewItem(string name = "Arroz") => new()
    {
        Name = name,
        Category = InventoryCategory.Grains,
        Unit = UnitOfMeasure.Pound,
        CurrentStock = 50,
        MinimumStock = 10,
        UnitCost = 32
    };

    // ---------- CREAR ----------

    [Fact]
    public async Task Crear_ConDatosValidos_GuardaElInsumo()
    {
        var id = await _service.CreateAsync(NewItem());

        var saved = await _service.GetByIdAsync(id);
        Assert.NotNull(saved);
        Assert.Equal("Arroz", saved!.Name);
        Assert.Equal(50, saved.CurrentStock);
        Assert.True(saved.IsActive);
    }

    [Fact]
    public async Task Crear_QuitaLosEspaciosDelNombre()
    {
        var id = await _service.CreateAsync(NewItem("  Aceite  "));

        var saved = await _service.GetByIdAsync(id);
        Assert.Equal("Aceite", saved!.Name);
    }

    [Fact]
    public async Task Crear_ConStockNegativo_LanzaErrorYNoGuarda()
    {
        var dto = NewItem();
        dto.CurrentStock = -5;

        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(dto));
        Assert.Empty(_repository.Items);
    }

    [Fact]
    public async Task Crear_ConNombreRepetido_LanzaError()
    {
        await _service.CreateAsync(NewItem("Arroz"));

        var ex = await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(NewItem("arroz")));

        Assert.Contains(ex.Errors, e => e.PropertyName == nameof(CreateInventoryItemDto.Name));
        Assert.Single(_repository.Items);
    }

    // ---------- EDITAR ----------

    [Fact]
    public async Task Editar_ActualizaLosDatos()
    {
        var id = await _service.CreateAsync(NewItem());
        var edit = await _service.GetForEditAsync(id);
        edit!.CurrentStock = 5;
        edit.UnitCost = 35;

        var updated = await _service.UpdateAsync(edit);

        var saved = await _service.GetByIdAsync(id);
        Assert.True(updated);
        Assert.Equal(5, saved!.CurrentStock);
        Assert.Equal(35, saved.UnitCost);
        Assert.True(saved.IsLowStock);   // 5 está por debajo del mínimo de 10
    }

    [Fact]
    public async Task Editar_InsumoQueNoExiste_DevuelveFalse()
    {
        var dto = new UpdateInventoryItemDto
        {
            Id = 999,
            Name = "Sal",
            Category = InventoryCategory.Spices,
            Unit = UnitOfMeasure.Pound
        };

        var updated = await _service.UpdateAsync(dto);

        Assert.False(updated);
    }

    [Fact]
    public async Task Editar_ConNombreDeOtroInsumo_LanzaError()
    {
        await _service.CreateAsync(NewItem("Arroz"));
        var idPollo = await _service.CreateAsync(NewItem("Pollo"));

        var edit = await _service.GetForEditAsync(idPollo);
        edit!.Name = "Arroz";

        await Assert.ThrowsAsync<ValidationException>(() => _service.UpdateAsync(edit));
    }

    // ---------- CONSULTAR Y LISTAR ----------

    [Fact]
    public async Task Consultar_InsumoQueNoExiste_DevuelveNull()
    {
        var result = await _service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task Listar_FiltraPorNombre()
    {
        await _service.CreateAsync(NewItem("Arroz"));
        await _service.CreateAsync(NewItem("Pollo"));

        var list = await _service.GetAllAsync("pol");

        Assert.Single(list);
        Assert.Equal("Pollo", list[0].Name);
    }
}