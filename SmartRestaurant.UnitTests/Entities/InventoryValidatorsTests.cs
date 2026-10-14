using FluentValidation.TestHelper;
using SmartRestaurant.Application.DTOs.Inventory;
using SmartRestaurant.Application.Validators.Inventory;
using SmartRestaurant.Domain.Enums;

namespace SmartRestaurant.UnitTests.Validators;

public class InventoryValidatorsTests
{
    private readonly CreateInventoryItemDtoValidator _createValidator = new();
    private readonly UpdateInventoryItemDtoValidator _updateValidator = new();

    // Datos correctos de ejemplo
    private static CreateInventoryItemDto ValidDto() => new()
    {
        Name = "Pechuga de pollo",
        Category = InventoryCategory.Meats,
        Unit = UnitOfMeasure.Pound,
        CurrentStock = 25,
        MinimumStock = 5,
        UnitCost = 185.50m,
        ExpirationDate = DateTime.Today.AddDays(5)
    };

    // ---------- DATOS CORRECTOS ----------

    [Fact]
    public void DatosValidos_NoTienenErrores()
    {
        var result = _createValidator.TestValidate(ValidDto());

        result.ShouldNotHaveAnyValidationErrors();
    }

    // ---------- DATOS OBLIGATORIOS ----------

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NombreVacio_TieneError(string name)
    {
        var dto = ValidDto();
        dto.Name = name;

        var result = _createValidator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void NombreDeMasDe100Caracteres_TieneError()
    {
        var dto = ValidDto();
        dto.Name = new string('a', 101);

        var result = _createValidator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void CategoriaSinSeleccionar_TieneError()
    {
        var dto = ValidDto();
        dto.Category = 0; // el formulario envía 0 cuando no se elige nada

        var result = _createValidator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Category);
    }

    [Fact]
    public void UnidadSinSeleccionar_TieneError()
    {
        var dto = ValidDto();
        dto.Unit = 0;

        var result = _createValidator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Unit);
    }

    // ---------- VALORES DE STOCK ----------

    [Fact]
    public void StockActualNegativo_TieneError()
    {
        var dto = ValidDto();
        dto.CurrentStock = -1;

        var result = _createValidator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.CurrentStock);
    }

    [Fact]
    public void StockMinimoNegativo_TieneError()
    {
        var dto = ValidDto();
        dto.MinimumStock = -1;

        var result = _createValidator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.MinimumStock);
    }

    [Fact]
    public void StockEnCero_EsValido()
    {
        var dto = ValidDto();
        dto.CurrentStock = 0;

        var result = _createValidator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.CurrentStock);
    }

    [Fact]
    public void CostoNegativo_TieneError()
    {
        var dto = ValidDto();
        dto.UnitCost = -10;

        var result = _createValidator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.UnitCost);
    }

    // ---------- FECHA DE VENCIMIENTO ----------

    [Fact]
    public void FechaDeVencimientoPasada_AlCrear_TieneError()
    {
        var dto = ValidDto();
        dto.ExpirationDate = DateTime.Today.AddDays(-1);

        var result = _createValidator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.ExpirationDate);
    }

    [Fact]
    public void SinFechaDeVencimiento_EsValido()
    {
        var dto = ValidDto();
        dto.ExpirationDate = null;

        var result = _createValidator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.ExpirationDate);
    }

    // ---------- EDITAR ----------

    [Fact]
    public void Editar_ConIdCero_TieneError()
    {
        var dto = new UpdateInventoryItemDto
        {
            Id = 0,
            Name = "Arroz",
            Category = InventoryCategory.Grains,
            Unit = UnitOfMeasure.Pound
        };

        var result = _updateValidator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}