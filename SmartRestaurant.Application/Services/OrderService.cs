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

    /*Cambio 2: dos campos nuevos
    Descripción: 
    guardan los validadores de los DTOs nuevos.*/

    private readonly IValidator<AddOrderItemDto> _addItemValidator;
    private readonly IValidator<UpdateOrderItemDto> _updateItemValidator;

    /*Cambio 3: constructor
    Descripción: 
    recibe los dos validadores por inyección de dependencias. AddValidatorsFromAssembly ya los registró, así que no hay que tocar el DI.*/
    public OrderService(
     IOrderRepository orderRepository,
     IProductRepository productRepository,
     IReservationRepository reservationRepository,
     IValidator<CreateOrderDto> createValidator,
     IValidator<OrderStatus> statusValidator,
     IValidator<AddOrderItemDto> addItemValidator,
     IValidator<UpdateOrderItemDto> updateItemValidator)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _reservationRepository = reservationRepository;
        _createValidator = createValidator;
        _statusValidator = statusValidator;
        _addItemValidator = addItemValidator;
        _updateItemValidator = updateItemValidator;
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

    /*Cambio único: agrupar por producto en CreateAsync
    Descripción: antes de buscar los productos, agrupa las líneas solicitadas por ProductId y suma sus cantidades, de modo que cada producto aparezca una sola vez en el pedido.
    Luego el resto del método trabaja sobre esa lista agrupada. El nombre y el precio siguen saliendo del catálogo.*/
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

        // Una mesa solo puede tener un pedido abierto (Pending o InPreparation)
        if (await _orderRepository.HasOpenOrderForTableAsync(dto.TableId, null, ct))
        {
            throw new ValidationException(
            [
                new ValidationFailure(
                nameof(CreateOrderDto.TableId),
                "La mesa ya tiene un pedido abierto.")
            ]);
        }

        // Una sola línea por producto: si se repite, se suman las cantidades
        var requestedDetails = dto.Details
            .GroupBy(detail => detail.ProductId)
            .Select(group => new CreateOrderDetailDto
            {
                ProductId = group.Key,
                Quantity = group.Sum(detail => detail.Quantity)
            })
            .ToList();

        var productIds = requestedDetails.Select(detail => detail.ProductId).ToArray();
        var products = await _productRepository.GetByIdsAsync(productIds, ct);
        var productsById = products.ToDictionary(product => product.Id);
        var orderDetails = new List<OrderDetail>(requestedDetails.Count);

        for (var index = 0; index < requestedDetails.Count; index++)
        {
            var requestedDetail = requestedDetails[index];
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

    /*Cambio 4: AddItemAsync
    Descripción: valida el DTO, busca el pedido (devuelve false si no existe) y toma nombre y precio del catálogo, nunca del navegador. 
    Rechaza productos inexistentes o inactivos con ValidationException. Delega la regla de negocio al dominio (Order.AddItem), que exige estado Pending, y guarda.*/
    public async Task<bool> AddItemAsync(AddOrderItemDto dto, CancellationToken ct = default)
    {
        await _addItemValidator.ValidateAndThrowAsync(dto, ct);

        var order = await _orderRepository.GetByIdAsync(dto.OrderId, ct);
        if (order is null)
        {
            return false;
        }

        var product = await _productRepository.GetByIdAsync(dto.ProductId, ct);
        if (product is null || !product.IsActive)
        {
            throw new ValidationException(
            [
                new ValidationFailure(
                nameof(AddOrderItemDto.ProductId),
                "El producto seleccionado no existe o está inactivo.")
            ]);
        }

        order.AddItem(product.Id, product.Name, product.Price, dto.Quantity);
        await _orderRepository.UpdateAsync(order, ct);
        return true;
    }

    /*Cambio 5: UpdateItemQuantityAsync
    Descripción: valida el DTO, busca el pedido y cambia la cantidad de la línea mediante Order.UpdateItemQuantity. 
    No consulta el catálogo, porque el precio de la línea ya quedó guardado. 
    Si el pedido no está en Pending o el producto no está en él, el dominio lanza InvalidOperationException.*/
    public async Task<bool> UpdateItemQuantityAsync(UpdateOrderItemDto dto, CancellationToken ct = default)
    {
        await _updateItemValidator.ValidateAndThrowAsync(dto, ct);

        var order = await _orderRepository.GetByIdAsync(dto.OrderId, ct);
        if (order is null)
        {
            return false;
        }

        order.UpdateItemQuantity(dto.ProductId, dto.Quantity);
        await _orderRepository.UpdateAsync(order, ct);
        return true;
    }

    /*Cambio 6: RemoveItemAsync
    Descripción: busca el pedido (devuelve false si no existe) y quita la línea del producto con Order.RemoveItem. 
    El dominio impide quitar la última línea y exige estado Pending. No usa DTO porque solo recibe dos identificadores.*/
    public async Task<bool> RemoveItemAsync(int orderId, int productId, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, ct);
        if (order is null)
        {
            return false;
        }

        order.RemoveItem(productId);
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
