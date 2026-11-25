using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SmartRestaurant.Application.DTOs.Billing;
using SmartRestaurant.Application.Exceptions;
using SmartRestaurant.Application.Interfaces.Repositories;
using SmartRestaurant.Application.Services;
using SmartRestaurant.Application.Validators.Billing;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;
using SmartRestaurant.Infrastructure.Contexts;

namespace SmartRestaurant.UnitTests.Services;

public class InvoiceServiceTests
{
    private readonly FakeInvoiceRepository _invoices = new();
    private readonly FakeOrderRepository _orders = new();
    private readonly InvoiceService _service;

    public InvoiceServiceTests()
    {
        _service = new InvoiceService(_invoices, _orders, new RegisterPaymentDtoValidator());
    }

    [Fact]
    public async Task CreateForOrderAsync_ConPedidoServed_CopiaTodosLosDetallesHistoricos()
    {
        var order = CreateServedOrderWithDetails();
        _orders.Seed(order);

        var invoiceId = await _service.CreateForOrderAsync(order.Id);
        var invoice = await _invoices.GetByIdAsync(invoiceId);

        Assert.NotNull(invoice);
        Assert.Equal(order.Id, invoice!.OrderId);
        Assert.Equal(43m, invoice.Total);
        Assert.Collection(
            invoice.Details,
            detail =>
            {
                Assert.Equal("Producto A", detail.ProductName);
                Assert.Equal(10.25m, detail.UnitPrice);
                Assert.Equal(2, detail.Quantity);
                Assert.Equal(20.50m, detail.Subtotal);
            },
            detail =>
            {
                Assert.Equal("Producto B", detail.ProductName);
                Assert.Equal(7.50m, detail.UnitPrice);
                Assert.Equal(3, detail.Quantity);
                Assert.Equal(22.50m, detail.Subtotal);
            });
        Assert.Equal(InvoiceStatus.Issued, invoice.Status);
    }

    [Fact]
    public async Task CreateForOrderAsync_ConPedidoInexistente_LanzaErrorYNoGuarda()
    {
        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            _service.CreateForOrderAsync(999));

        Assert.Contains(exception.Errors, error => error.ErrorMessage.Contains("no existe"));
        Assert.Empty(_invoices.Invoices);
    }

    [Fact]
    public async Task CreateForOrderAsync_ConFacturaExistente_LanzaErrorYNoDuplica()
    {
        var order = CreateServedOrder();
        _orders.Seed(order);
        await _service.CreateForOrderAsync(order.Id);

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            _service.CreateForOrderAsync(order.Id));

        Assert.Contains(exception.Errors, error => error.ErrorMessage.Contains("ya tiene una factura"));
        Assert.Single(_invoices.Invoices);
    }

    [Fact]
    public async Task CreateForOrderAsync_ConflictoDeIndiceUnico_DevuelveErrorControlado()
    {
        var order = CreateServedOrder();
        _orders.Seed(order);
        _invoices.AddException = new DuplicateInvoiceException("duplicate key");

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            _service.CreateForOrderAsync(order.Id));

        Assert.Contains(exception.Errors, error =>
            error.ErrorMessage.Contains("ya tiene una factura"));
    }

    [Fact]
    public async Task CreateForOrderAsync_ConservaElTotalAunqueCambieElPedido()
    {
        var order = CreateServedOrderWithDetails();
        _orders.Seed(order);

        var invoiceId = await _service.CreateForOrderAsync(order.Id);
        var invoice = await _invoices.GetByIdAsync(invoiceId);
        var originalTotal = invoice!.Total;

        order.Details.Clear();

        Assert.Equal(43m, originalTotal);
        Assert.Equal(originalTotal, invoice.Total);
    }

    [Fact]
    public async Task CreateForOrderAsync_ConPedidoPendiente_LanzaErrorYNoGuarda()
    {
        var order = CreateOrder();
        _orders.Seed(order);

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            _service.CreateForOrderAsync(order.Id));

        Assert.Contains(exception.Errors, error => error.PropertyName == "orderId");
        Assert.Empty(_invoices.Invoices);
    }

    [Fact]
    public async Task CreateForOrderAsync_ConPedidoCancelado_LanzaErrorYNoGuarda()
    {
        var order = CreateOrder();
        order.ChangeStatus(OrderStatus.Cancelled);
        _orders.Seed(order);

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            _service.CreateForOrderAsync(order.Id));

        Assert.Contains(exception.Errors, error => error.ErrorMessage.Contains("cancelado"));
        Assert.Empty(_invoices.Invoices);
    }

    [Fact]
    public async Task RegisterPaymentAsync_ImporteSuperiorAlSaldo_LanzaErrorYNoGuarda()
    {
        var order = CreateServedOrder();
        _orders.Seed(order);
        var invoiceId = await _service.CreateForOrderAsync(order.Id);

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            _service.RegisterPaymentAsync(new RegisterPaymentDto
            {
                InvoiceId = invoiceId,
                Amount = 100.01m,
                PaymentDate = DateTime.UtcNow,
                Method = PaymentMethod.Cash
            }));

        Assert.Contains(exception.Errors, error => error.PropertyName == nameof(RegisterPaymentDto.Amount));
        var invoice = await _invoices.GetByIdAsync(invoiceId);
        Assert.Equal(0m, invoice!.AmountPaid);
        Assert.Equal(InvoiceStatus.Issued, invoice.Status);
    }

    [Fact]
    public async Task RegisterPaymentAsync_ImporteIgualAlSaldo_PagaLaFacturaCompleta()
    {
        var order = CreateServedOrder();
        _orders.Seed(order);
        var invoiceId = await _service.CreateForOrderAsync(order.Id);

        var registered = await _service.RegisterPaymentAsync(new RegisterPaymentDto
        {
            InvoiceId = invoiceId,
            Amount = 100m,
            PaymentDate = DateTime.UtcNow,
            Method = PaymentMethod.Cash
        });

        var invoice = await _invoices.GetByIdAsync(invoiceId);
        Assert.True(registered);
        Assert.Equal(InvoiceStatus.Paid, invoice!.Status);
        Assert.Equal(0m, invoice.Balance);
    }

    [Fact]
    public async Task RegisterPaymentAsync_ConflictoDeConcurrencia_DevuelveErrorControlado()
    {
        var order = CreateServedOrder();
        _orders.Seed(order);
        var invoiceId = await _service.CreateForOrderAsync(order.Id);
        _invoices.UpdateException = new InvoiceConcurrencyException("stale invoice");

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            _service.RegisterPaymentAsync(new RegisterPaymentDto
            {
                InvoiceId = invoiceId,
                Amount = 50m,
                PaymentDate = DateTime.Now,
                Method = PaymentMethod.Cash
            }));

        Assert.Contains(exception.Errors, error =>
            error.ErrorMessage.Contains("balance cambió"));
    }

    [Fact]
    public void RegisterPaymentDto_PorDefectoUsaFechaLocal()
    {
        var dto = new RegisterPaymentDto();

        Assert.Equal(DateTimeKind.Local, dto.PaymentDate.Kind);
    }

    [Fact]
    public void Invoice_RowVersionEstaConfiguradoComoTokenDeConcurrencia()
    {
        var options = new DbContextOptionsBuilder<SmartRestaurantDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=SmartRestaurantTests;Trusted_Connection=True")
            .Options;

        using var context = new SmartRestaurantDbContext(options);
        var rowVersion = context.Model.FindEntityType(typeof(Invoice))?.FindProperty("RowVersion");

        Assert.NotNull(rowVersion);
        Assert.True(rowVersion!.IsConcurrencyToken);
        Assert.False(rowVersion.IsNullable);
        Assert.Equal(
            Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAddOrUpdate,
            rowVersion.ValueGenerated);
    }

    [Fact]
    public async Task RegisterPaymentAsync_PagosParcialesActualizaEstadoYSaldos()
    {
        var order = CreateServedOrder();
        _orders.Seed(order);
        var invoiceId = await _service.CreateForOrderAsync(order.Id);

        var firstPaymentRegistered = await _service.RegisterPaymentAsync(new RegisterPaymentDto
        {
            InvoiceId = invoiceId,
            Amount = 40m,
            PaymentDate = DateTime.UtcNow,
            Method = PaymentMethod.Cash
        });

        var partiallyPaidInvoice = await _invoices.GetByIdAsync(invoiceId);
        Assert.True(firstPaymentRegistered);
        Assert.Equal(InvoiceStatus.PartiallyPaid, partiallyPaidInvoice!.Status);
        Assert.Equal(60m, partiallyPaidInvoice.Balance);

        var secondPaymentRegistered = await _service.RegisterPaymentAsync(new RegisterPaymentDto
        {
            InvoiceId = invoiceId,
            Amount = 60m,
            PaymentDate = DateTime.UtcNow,
            Method = PaymentMethod.CreditCard
        });

        var paidInvoice = await _invoices.GetByIdAsync(invoiceId);
        Assert.True(secondPaymentRegistered);
        Assert.Equal(InvoiceStatus.Paid, paidInvoice!.Status);
        Assert.Equal(0m, paidInvoice.Balance);
    }

    [Fact]
    public void RegisterPayment_ImporteSuperiorAlSaldo_RechazaElPagoEnDomain()
    {
        var invoice = new Invoice(
            10,
            "INV-00000010",
            DateTime.UtcNow,
            [new InvoiceDetail("Producto", 100m, 1)])
        {
            Id = 1
        };
        var payment = new Payment(1, 100.01m, DateTime.UtcNow, PaymentMethod.Cash);

        Assert.Throws<InvalidOperationException>(() => invoice.RegisterPayment(payment));
        Assert.Empty(invoice.Payments);
        Assert.Equal(InvoiceStatus.Issued, invoice.Status);
    }

    [Fact]
    public void Invoice_ColeccionesExpuestasNoPermitenMutacionDirecta()
    {
        var invoice = new Invoice(
            10,
            "INV-00000010",
            DateTime.UtcNow,
            [new InvoiceDetail("Producto", 100m, 1)])
        {
            Id = 1
        };

        var details = Assert.IsAssignableFrom<IList<InvoiceDetail>>(invoice.Details);
        var payments = Assert.IsAssignableFrom<IList<Payment>>(invoice.Payments);

        Assert.Throws<NotSupportedException>(() => details.Add(new InvoiceDetail("Otro", 10m, 1)));
        Assert.Throws<NotSupportedException>(() =>
            payments.Add(new Payment(1, 10m, DateTime.UtcNow, PaymentMethod.Cash)));
    }

    private static Order CreateOrder() =>
        new(1, [new OrderDetail(1, "Producto histórico", 100m, 1)]) { Id = 10 };

    private static Order CreateServedOrder()
    {
        var order = CreateOrder();
        order.ChangeStatus(OrderStatus.InPreparation);
        order.ChangeStatus(OrderStatus.Served);
        return order;
    }

    private static Order CreateServedOrderWithDetails()
    {
        var order = new Order(1,
        [
            new OrderDetail(1, "Producto A", 10.25m, 2),
            new OrderDetail(2, "Producto B", 7.50m, 3)
        ])
        {
            Id = 10
        };
        order.ChangeStatus(OrderStatus.InPreparation);
        order.ChangeStatus(OrderStatus.Served);
        return order;
    }

    private sealed class FakeInvoiceRepository : IInvoiceRepository
    {
        private int _nextId = 1;

        public List<Invoice> Invoices { get; } = [];

        public Exception? AddException { get; set; }

        public Exception? UpdateException { get; set; }

        public Task<IReadOnlyList<Invoice>> GetAllAsync(CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyList<Invoice>>(Invoices);

        public Task<Invoice?> GetByIdAsync(int id, CancellationToken ct = default) =>
            Task.FromResult(Invoices.FirstOrDefault(invoice => invoice.Id == id));

        public Task<bool> ExistsForOrderAsync(int orderId, CancellationToken ct = default) =>
            Task.FromResult(Invoices.Any(invoice => invoice.OrderId == orderId));

        public Task AddAsync(Invoice invoice, CancellationToken ct = default)
        {
            if (AddException is not null)
            {
                throw AddException;
            }

            invoice.Id = _nextId++;
            Invoices.Add(invoice);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Invoice invoice, CancellationToken ct = default)
        {
            if (UpdateException is not null)
            {
                throw UpdateException;
            }

            return Task.CompletedTask;
        }
    }

    private sealed class FakeOrderRepository : IOrderRepository
    {
        private readonly List<Order> _orders = [];

        public void Seed(Order order) => _orders.Add(order);

        public Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyList<Order>>(_orders);

        public Task<Order?> GetByIdAsync(int id, CancellationToken ct = default) =>
            Task.FromResult(_orders.FirstOrDefault(order => order.Id == id));

        public Task<bool> HasOpenOrderForTableAsync(
            int tableId,
            int? excludeOrderId = null,
            CancellationToken ct = default) =>
            Task.FromResult(false);

        public Task<IReadOnlyCollection<int>> GetOpenOrderTableIdsAsync(
    CancellationToken ct = default) =>
    Task.FromResult<IReadOnlyCollection<int>>(Array.Empty<int>());    

        public Task AddAsync(Order order, CancellationToken ct = default) =>
            Task.CompletedTask;

        public Task UpdateAsync(Order order, CancellationToken ct = default) =>
            Task.CompletedTask;
    }
}
