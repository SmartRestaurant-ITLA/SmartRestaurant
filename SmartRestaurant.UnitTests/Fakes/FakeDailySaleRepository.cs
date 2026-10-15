using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Interfaces;

namespace SmartRestaurant.UnitTests.Fakes;

// Repositorio en memoria: guarda las ventas en una lista en vez de SQL Server
public class FakeDailySaleRepository : IDailySaleRepository
{
    private readonly List<DailySale> _sales = new();
    private int _nextId = 1;

    public IReadOnlyList<DailySale> Sales => _sales;

    public Task<IReadOnlyList<DailySale>> GetAllAsync(DateTime? date, CancellationToken ct = default)
    {
        IEnumerable<DailySale> query = _sales;

        if (date.HasValue)
            query = query.Where(s => s.SaleDate.Date == date.Value.Date);

        IReadOnlyList<DailySale> result = query
            .OrderByDescending(s => s.SaleDate)
            .ThenBy(s => s.ProductName)
            .ToList();

        return Task.FromResult(result);
    }

    public Task<bool> ExistsAsync(DateTime saleDate, string productName, CancellationToken ct = default)
    {
        var exists = _sales.Any(s =>
            s.SaleDate.Date == saleDate.Date
            && string.Equals(s.ProductName, productName, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(exists);
    }

    public Task AddAsync(DailySale sale, CancellationToken ct = default)
    {
        sale.Id = _nextId++;   // simula el Id que pondria la base de datos
        _sales.Add(sale);
        return Task.CompletedTask;
    }
}