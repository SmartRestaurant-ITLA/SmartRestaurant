using AutoMapper;
using SmartRestaurant.Application.DTOs.Inventory;
using SmartRestaurant.Domain.Entities;

namespace SmartRestaurant.Application.Mappings.EntitiesAndDtos;

public class InventoryItemMappingProfile : Profile
{
    public InventoryItemMappingProfile()
    {
        // Entidad → DTO para mostrar (listar y consultar)
        CreateMap<InventoryItem, InventoryItemDto>()
            .ForMember(dest => dest.IsNearExpiration, opt => opt.MapFrom(src => src.IsNearExpiration(DateTime.Today, 3)))
            .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => src.IsExpired(DateTime.Today)));

        // Entidad → DTO para llenar el formulario de editar
        CreateMap<InventoryItem, UpdateInventoryItemDto>();

        // Formulario "Nuevo" → Entidad
        CreateMap<CreateInventoryItemDto, InventoryItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        // Formulario "Editar" → Entidad existente
        CreateMap<UpdateInventoryItemDto, InventoryItem>()
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }
}