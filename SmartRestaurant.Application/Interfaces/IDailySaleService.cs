using SmartRestaurant.Application.DTOs.Sales;

namespace SmartRestaurant.Application.Interfaces;

public interface IDailySaleService
{
    Task<IReadOnlyList<DailySaleDto>> GetAllAsync(DateTime? date = null, CancellationToken ct = default);   // listar

    Task<int> CreateAsync(CreateDailySaleDto dto, CancellationToken ct = default);                           // registrar
}