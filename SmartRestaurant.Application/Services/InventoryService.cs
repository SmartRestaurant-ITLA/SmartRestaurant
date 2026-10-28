using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using SmartRestaurant.Application.DTOs.Inventory;
using SmartRestaurant.Application.Interfaces.Services;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Application.Interfaces.Repositories;

namespace SmartRestaurant.Application.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryItemRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateInventoryItemDto> _createValidator;
    private readonly IValidator<UpdateInventoryItemDto> _updateValidator;

    public InventoryService(
        IInventoryItemRepository repository,
        IMapper mapper,
        IValidator<CreateInventoryItemDto> createValidator,
        IValidator<UpdateInventoryItemDto> updateValidator)
    {
        _repository = repository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    // LISTAR
    public async Task<IReadOnlyList<InventoryItemDto>> GetAllAsync(string? search = null, bool includeInactive = false, CancellationToken ct = default)
    {
        var items = await _repository.GetAllAsync(search, includeInactive, ct);
        return _mapper.Map<List<InventoryItemDto>>(items);
    }

    // CONSULTAR
    public async Task<InventoryItemDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var item = await _repository.GetByIdAsync(id, ct);
        return item is null ? null : _mapper.Map<InventoryItemDto>(item);
    }

    // DATOS PARA EL FORMULARIO DE EDITAR
    public async Task<UpdateInventoryItemDto?> GetForEditAsync(int id, CancellationToken ct = default)
    {
        var item = await _repository.GetByIdAsync(id, ct);
        return item is null ? null : _mapper.Map<UpdateInventoryItemDto>(item);
    }

    // CREAR
    public async Task<int> CreateAsync(CreateInventoryItemDto dto, CancellationToken ct = default)
    {
        await _createValidator.ValidateAndThrowAsync(dto, ct);

        dto.Name = dto.Name.Trim();
        await EnsureNameIsUniqueAsync(dto.Name, null, ct);

        var item = _mapper.Map<InventoryItem>(dto);
        await _repository.AddAsync(item, ct);
        return item.Id;
    }

    // EDITAR
    public async Task<bool> UpdateAsync(UpdateInventoryItemDto dto, CancellationToken ct = default)
    {
        await _updateValidator.ValidateAndThrowAsync(dto, ct);

        var item = await _repository.GetByIdAsync(dto.Id, ct);
        if (item is null)
            return false;

        dto.Name = dto.Name.Trim();
        await EnsureNameIsUniqueAsync(dto.Name, dto.Id, ct);

        _mapper.Map(dto, item);
        item.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(item, ct);
        return true;
    }

    // Regla: no puede haber dos insumos con el mismo nombre
    private async Task EnsureNameIsUniqueAsync(string name, int? excludeId, CancellationToken ct)
    {
        if (await _repository.ExistsByNameAsync(name, excludeId, ct))
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(CreateInventoryItemDto.Name), "Ya existe un insumo con ese nombre.")
            });
        }
    }
}