using FluentValidation;
using FluentValidation.Results;
using SmartRestaurant.Application.DTOs.Billing;
using SmartRestaurant.Application.Exceptions;
using SmartRestaurant.Application.Interfaces;
using SmartRestaurant.Application.Interfaces.Repositories;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;

namespace SmartRestaurant.Application.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IValidator<RegisterPaymentDto> _paymentValidator;

    public InvoiceService(
        IInvoiceRepository invoiceRepository,
        IOrderRepository orderRepository,
        IValidator<RegisterPaymentDto> paymentValidator)
    {
        _invoiceRepository = invoiceRepository;
        _orderRepository = orderRepository;
        _paymentValidator = paymentValidator;
    }

    public async Task<IReadOnlyList<InvoiceDto>> GetAllAsync(CancellationToken ct = default)
    {
        var invoices = await _invoiceRepository.GetAllAsync(ct);
        return invoices.Select(ToDto).ToArray();
    }

    public async Task<InvoiceDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id, ct);
        return invoice is null ? null : ToDto(invoice);
    }

    public async Task<int> CreateForOrderAsync(int orderId, CancellationToken ct = default)
    {
        if (orderId <= 0)
        {
            throw new ValidationException(
            [
                new ValidationFailure(nameof(orderId), "El pedido seleccionado no es válido.")
            ]);
        }

        var order = await _orderRepository.GetByIdAsync(orderId, ct);
        if (order is null)
        {
            throw new ValidationException(
            [
                new ValidationFailure(nameof(orderId), "El pedido seleccionado no existe.")
            ]);
        }

        if (order.Status == OrderStatus.Cancelled)
        {
            throw new ValidationException(
            [
                new ValidationFailure(nameof(orderId), "No se puede facturar un pedido cancelado.")
            ]);
        }

        if (order.Status != OrderStatus.Served)
        {
            throw new ValidationException(
            [
                new ValidationFailure(nameof(orderId), "Solo se pueden facturar pedidos en estado Served.")
            ]);
        }

        if (await _invoiceRepository.ExistsForOrderAsync(orderId, ct))
        {
            throw new ValidationException(
            [
                new ValidationFailure(nameof(orderId), "El pedido ya tiene una factura.")
            ]);
        }

        var now = DateTime.UtcNow;
        var details = order.Details
            .Select(detail => new InvoiceDetail(detail.ProductName, detail.UnitPrice, detail.Quantity))
            .ToList();
        var invoice = new Invoice(order.Id, $"INV-{order.Id:D8}", now, details)
        {
            CreatedAt = now,
            UpdatedAt = now
        };

        try
        {
            await _invoiceRepository.AddAsync(invoice, ct);
        }
        catch (DuplicateInvoiceException)
        {
            throw new ValidationException(
            [
                new ValidationFailure(
                    nameof(orderId),
                    "El pedido ya tiene una factura. Actualiza la lista e inténtalo nuevamente.")
            ]);
        }

        return invoice.Id;
    }

    public async Task<bool> RegisterPaymentAsync(RegisterPaymentDto dto, CancellationToken ct = default)
    {
        await _paymentValidator.ValidateAndThrowAsync(dto, ct);

        var invoice = await _invoiceRepository.GetByIdAsync(dto.InvoiceId, ct);
        if (invoice is null)
        {
            return false;
        }

        if (invoice.Status is InvoiceStatus.Paid or InvoiceStatus.Cancelled)
        {
            throw new ValidationException(
            [
                new ValidationFailure(nameof(dto.InvoiceId), "No se pueden registrar pagos en una factura pagada o cancelada.")
            ]);
        }

        if (dto.Amount > invoice.Balance)
        {
            throw new ValidationException(
            [
                new ValidationFailure(nameof(dto.Amount), "El importe supera el saldo pendiente de la factura.")
            ]);
        }

        var payment = new Payment(
            invoice.Id,
            dto.Amount,
            dto.PaymentDate,
            dto.Method,
            dto.Reference);

        invoice.RegisterPayment(payment);
        try
        {
            await _invoiceRepository.UpdateAsync(invoice, ct);
        }
        catch (InvoiceConcurrencyException)
        {
            throw new ValidationException(
            [
                new ValidationFailure(
                    nameof(dto.Amount),
                    "El balance cambió porque se registró otro pago. Actualiza la factura e inténtalo nuevamente.")
            ]);
        }

        return true;
    }

    private static InvoiceDto ToDto(Invoice invoice) =>
        new()
        {
            Id = invoice.Id,
            OrderId = invoice.OrderId,
            InvoiceNumber = invoice.InvoiceNumber,
            IssuedAt = invoice.IssuedAt,
            Status = invoice.Status,
            Total = invoice.Total,
            AmountPaid = invoice.AmountPaid,
            Balance = invoice.Balance,
            Details = invoice.Details.Select(detail => new InvoiceDetailDto
            {
                ProductName = detail.ProductName,
                UnitPrice = detail.UnitPrice,
                Quantity = detail.Quantity,
                Subtotal = detail.Subtotal
            }).ToArray(),
            Payments = invoice.Payments.Select(payment => new PaymentDto
            {
                Id = payment.Id,
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                Method = payment.Method,
                Reference = payment.Reference
            }).ToArray()
        };
}
