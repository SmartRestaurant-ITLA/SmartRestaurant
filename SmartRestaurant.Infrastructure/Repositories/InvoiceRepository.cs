using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.Exceptions;
using SmartRestaurant.Application.Interfaces.Repositories;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Infrastructure.Contexts;

namespace SmartRestaurant.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly SmartRestaurantDbContext _context;
    private readonly DbSet<Invoice> _invoices;

    public InvoiceRepository(SmartRestaurantDbContext context)
    {
        _context = context;
        _invoices = context.Set<Invoice>();
    }

    public async Task<IReadOnlyList<Invoice>> GetAllAsync(CancellationToken ct = default) =>
        await _invoices
            .AsNoTracking()
            .Include(invoice => invoice.Details)
            .Include(invoice => invoice.Payments)
            .AsSplitQuery()
            .OrderByDescending(invoice => invoice.IssuedAt)
            .ToListAsync(ct);

    public Task<Invoice?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _invoices
            .Include(invoice => invoice.Details)
            .Include(invoice => invoice.Payments)
            .AsSplitQuery()
            .FirstOrDefaultAsync(invoice => invoice.Id == id, ct);

    public Task<bool> ExistsForOrderAsync(int orderId, CancellationToken ct = default) =>
        _invoices.AnyAsync(invoice => invoice.OrderId == orderId, ct);

    public async Task AddAsync(Invoice invoice, CancellationToken ct = default)
    {
        _invoices.Add(invoice);
        try
        {
            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
        {
            throw new DuplicateInvoiceException(
                "La factura ya existe para este pedido.",
                exception);
        }
    }

    public async Task UpdateAsync(Invoice invoice, CancellationToken ct = default)
    {
        _invoices.Update(invoice);
        try
        {
            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            throw new InvoiceConcurrencyException(
                "La factura fue modificada por otra solicitud.",
                exception);
        }
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException exception) =>
        exception.GetBaseException() is SqlException { Number: 2601 or 2627 };
}
