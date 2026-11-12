using FluentValidation;
using SmartRestaurant.Application.DTOs.Orders;
using SmartRestaurant.Application.DTOs.Products;
using SmartRestaurant.Application.Interfaces.Repositories;
using SmartRestaurant.Application.Services;
using SmartRestaurant.Application.Validators.Orders;
using SmartRestaurant.Application.Validators.Products;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;

namespace SmartRestaurant.UnitTests.Services;

public class OrderServiceTests
{
    private readonly FakeOrderRepository _orders = new();
    private readonly FakeProductRepository _products = new();
    private readonly FakeReservationRepository _reservations = new();
    private readonly OrderService _service;

    public OrderServiceTests()
    {
        _service = new OrderService(
            _orders,
            _products,
            _reservations,
            new CreateOrderDtoValidator(),
            new OrderStatusValidator());
    }

    [Fact]
    public async Task CreateAsync_ObtieneElPrecioDelCatalogoYGuardaSnapshots()
    {
        var product = AddProduct("Pizza", 250m);

        var id = await _service.CreateAsync(new CreateOrderDto
        {
            TableId = 5,
            Details = [new CreateOrderDetailDto { ProductId = product.Id, Quantity = 2 }]
        });

        product.Update("Pizza actualizada", 300m);
        var saved = Assert.Single(_orders.Orders);
        var detail = Assert.Single(saved.Details);

        Assert.Equal(id, saved.Id);
        Assert.Equal("Pizza", detail.ProductName);
        Assert.Equal(250m, detail.UnitPrice);
        Assert.Equal(500m, detail.Subtotal);
        Assert.Equal(500m, saved.Total);
    }

    [Fact]
    public async Task CreateAsync_ConMesaInexistente_NoGuardaElPedido()
    {
        AddProduct("Pizza", 250m);

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            _service.CreateAsync(new CreateOrderDto
            {
                TableId = 999,
                Details = [new CreateOrderDetailDto { ProductId = 1, Quantity = 1 }]
            }));

        Assert.Contains(exception.Errors, error => error.PropertyName == nameof(CreateOrderDto.TableId));
        Assert.Empty(_orders.Orders);
    }

    [Fact]
    public async Task CreateAsync_ConProductoInactivo_NoGuardaElPedido()
    {
        var product = AddProduct("Pizza", 250m);
        product.Deactivate();

        await Assert.ThrowsAsync<ValidationException>(() =>
            _service.CreateAsync(new CreateOrderDto
            {
                TableId = 5,
                Details = [new CreateOrderDetailDto { ProductId = product.Id, Quantity = 1 }]
            }));

        Assert.Empty(_orders.Orders);
    }

    [Fact]
    public async Task ChangeStatusAsync_RechazaTransicionesInvalidas()
    {
        var product = AddProduct("Pizza", 250m);
        var id = await _service.CreateAsync(new CreateOrderDto
        {
            TableId = 5,
            Details = [new CreateOrderDetailDto { ProductId = product.Id, Quantity = 1 }]
        });

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.ChangeStatusAsync(id, OrderStatus.Served));

        Assert.Equal(OrderStatus.Pending, Assert.Single(_orders.Orders).Status);
    }

    [Fact]
    public async Task ProductService_CreateAsync_RechazaNombreDuplicado()
    {
        var products = new FakeProductRepository();
        var service = new ProductService(
            products,
            new CreateProductDtoValidator(),
            new UpdateProductDtoValidator());

        await service.CreateAsync(new CreateProductDto { Name = "Pizza", Price = 250m });

        await Assert.ThrowsAsync<ValidationException>(() =>
            service.CreateAsync(new CreateProductDto { Name = " pizza ", Price = 300m }));

        Assert.Single(await service.GetAllAsync());
    }

    private Product AddProduct(string name, decimal price)
    {
        var product = new Product(name, price) { Id = 1 };
        _products.Seed(product);
        return product;
    }

    private sealed class FakeOrderRepository : IOrderRepository
    {
        private int _nextId = 1;

        public List<Order> Orders { get; } = [];

        public Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyList<Order>>(Orders);

        public Task<Order?> GetByIdAsync(int id, CancellationToken ct = default) =>
            Task.FromResult(Orders.FirstOrDefault(order => order.Id == id));

        public Task AddAsync(Order order, CancellationToken ct = default)
        {
            order.Id = _nextId++;
            Orders.Add(order);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Order order, CancellationToken ct = default) =>
            Task.CompletedTask;
    }

    private sealed class FakeProductRepository : IProductRepository
    {
        private readonly List<Product> _products = [];
        private int _nextId = 1;

        public void Seed(Product product)
        {
            _products.Add(product);
            _nextId = Math.Max(_nextId, product.Id + 1);
        }

        public Task<IReadOnlyList<Product>> GetAllAsync(bool includeInactive, CancellationToken ct = default)
        {
            var products = includeInactive
                ? _products
                : _products.Where(product => product.IsActive).ToList();
            return Task.FromResult<IReadOnlyList<Product>>(products);
        }

        public Task<Product?> GetByIdAsync(int id, CancellationToken ct = default) =>
            Task.FromResult(_products.FirstOrDefault(product => product.Id == id));

        public Task<IReadOnlyList<Product>> GetByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyList<Product>>(_products.Where(product => ids.Contains(product.Id)).ToList());

        public Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken ct = default) =>
            Task.FromResult(_products.Any(product =>
                string.Equals(product.Name, name, StringComparison.OrdinalIgnoreCase)
                && product.Id != excludeId));

        public Task AddAsync(Product product, CancellationToken ct = default)
        {
            product.Id = _nextId++;
            _products.Add(product);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Product product, CancellationToken ct = default) =>
            Task.CompletedTask;
    }

    private sealed class FakeReservationRepository : IReservationRepository
    {
        private readonly IReadOnlyCollection<Table> _tables = [new Table(5, 4)];

        public IReadOnlyCollection<Table> GetTables() => _tables;

        public IReadOnlyCollection<Reservation> GetReservations() => [];

        public Reservation? GetReservation(int id) => null;

        public Reservation SaveReservation(Reservation reservation) =>
            throw new NotSupportedException();
    }
}
