using FluentValidation;
using FluentValidation.Results;
using SmartRestaurant.Application.DTOs.Orders;
using SmartRestaurant.Application.Interfaces;
using SmartRestaurant.Application.Interfaces.Repositories;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Enums;

namespace SmartRestaurant.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IValidator<CreateOrderDto> _createValidator;
    private readonly IValidator<OrderStatus> _statusValidator;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IReservationRepository reservationRepository,
        IValidator<CreateOrderDto> createValidator,
        IValidator<OrderStatus> statusValidator)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _reservationRepository = reservationRepository;
        _createValidator = createValidator;
        _statusValidator = statusValidator;
    }

    public async Task<IReadOnlyList<OrderListItemDto>> GetAllAsync(CancellationToken ct = default)
    {
        var orders = await _orderRepository.GetAllAsync(ct);
        var tableNumbers = GetTableNumbers();

        return orders.Select(order => ToListItemDto(order, tableNumbers)).ToArray();
    }

    public async Task<OrderDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, ct);
        if (order is null)
        {
            return null;
        }

        var tableNumbers = GetTableNumbers();
        return ToOrderDto(order, tableNumbers);
    }

    public async Task<int> CreateAsync(CreateOrderDto dto, CancellationToken ct = default)
    {
        await _createValidator.ValidateAndThrowAsync(dto, ct);

        var table = _reservationRepository.GetTables().FirstOrDefault(candidate => candidate.Id == dto.TableId);
        if (table is null || !table.IsActive)
        {
            throw new ValidationException(
            [
                new ValidationFailure(nameof(CreateOrderDto.TableId), "Selecciona una mesa existente y activa.")
            ]);
        }

        var productIds = dto.Details.Select(detail => detail.ProductId).Distinct().ToArray();
        var products = await _productRepository.GetByIdsAsync(productIds, ct);
        var productsById = products.ToDictionary(product => product.Id);
        var orderDetails = new List<OrderDetail>(dto.Details.Count);

        for (var index = 0; index < dto.Details.Count; index++)
        {
            var requestedDetail = dto.Details[index];
            if (!productsById.TryGetValue(requestedDetail.ProductId, out var product) || !product.IsActive)
            {
                throw new ValidationException(
                [
                    new ValidationFailure(
                        $"{nameof(CreateOrderDto.Details)}[{index}].{nameof(CreateOrderDetailDto.ProductId)}",
                        "El producto seleccionado no existe o está inactivo.")
                ]);
            }

            orderDetails.Add(new OrderDetail(
                product.Id,
                product.Name,
                product.Price,
                requestedDetail.Quantity));
        }

        var now = DateTime.UtcNow;
        var order = new Order(dto.TableId, orderDetails)
        {
            CreatedAt = now,
            UpdatedAt = now
        };

        await _orderRepository.AddAsync(order, ct);
        return order.Id;
    }

    public async Task<bool> ChangeStatusAsync(int orderId, OrderStatus status, CancellationToken ct = default)
    {
        await _statusValidator.ValidateAndThrowAsync(status, ct);

        var order = await _orderRepository.GetByIdAsync(orderId, ct);
        if (order is null)
        {
            return false;
        }

        order.ChangeStatus(status);
        await _orderRepository.UpdateAsync(order, ct);
        return true;
    }

    private Dictionary<int, int> GetTableNumbers() =>
        _reservationRepository.GetTables().ToDictionary(table => table.Id, table => table.Number);

    private static OrderListItemDto ToListItemDto(Order order, IReadOnlyDictionary<int, int> tableNumbers) =>
        new()
        {
            Id = order.Id,
            TableId = order.TableId,
            TableNumber = tableNumbers.TryGetValue(order.TableId, out var tableNumber) ? tableNumber : order.TableId,
            CreatedAt = order.CreatedAt,
            Status = order.Status,
            DetailCount = order.Details.Count,
            Total = order.Total
        };

    private static OrderDto ToOrderDto(Order order, IReadOnlyDictionary<int, int> tableNumbers) =>
        new()
        {
            Id = order.Id,
            TableId = order.TableId,
            TableNumber = tableNumbers.TryGetValue(order.TableId, out var tableNumber) ? tableNumber : order.TableId,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Status = order.Status,
            Total = order.Total,
            AvailableStatusTransitions = order.GetAllowedStatusTransitions().ToArray(),
            Details = order.Details.Select(detail => new OrderDetailDto
            {
                ProductId = detail.ProductId,
                ProductName = detail.ProductName,
                UnitPrice = detail.UnitPrice,
                Quantity = detail.Quantity,
                Subtotal = detail.Subtotal
            }).ToArray()
        };
}
