using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using SmartRestaurant.Application.DTOs.Sales;
using SmartRestaurant.Application.Interfaces;
using SmartRestaurant.Domain.Entities;
using SmartRestaurant.Domain.Interfaces;

namespace SmartRestaurant.Application.Service;

public class DailySaleService : IDailySaleService
{
    private readonly IDailySaleRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateDailySaleDto> _createValidator;

    public DailySaleService(
        IDailySaleRepository repository,
        IMapper mapper,
        IValidator<CreateDailySaleDto> createValidator)
    {
        _repository = repository;
        _mapper = mapper;
        _createValidator = createValidator;
    }

    // LISTAR
    public async Task<IReadOnlyList<DailySaleDto>> GetAllAsync(DateTime? date = null, CancellationToken ct = default)
    {
        var sales = await _repository.GetAllAsync(date?.Date, ct);
        return _mapper.Map<List<DailySaleDto>>(sales);
    }

    // REGISTRAR
    public async Task<int> CreateAsync(CreateDailySaleDto dto, CancellationToken ct = default)
    {
        await _createValidator.ValidateAndThrowAsync(dto, ct);

        dto.ProductName = dto.ProductName.Trim();
        dto.SaleDate = dto.SaleDate.Date;

        // Regla: un producto no puede registrarse dos veces el mismo dia
        if (await _repository.ExistsAsync(dto.SaleDate, dto.ProductName, ct))
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(nameof(CreateDailySaleDto.ProductName), "Ya existe un registro de ese producto para esa fecha.")
            });
        }

        var sale = _mapper.Map<DailySale>(dto);
        await _repository.AddAsync(sale, ct);
        return sale.Id;
    }
}