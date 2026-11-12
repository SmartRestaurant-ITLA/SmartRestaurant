using AutoMapper;
using SmartRestaurant.Application.DTOs.Inventory;
using SmartRestaurant.Domain.Entities;

namespace SmartRestaurant.Application.Mappings.EntitiesAndDtos;

public class InventoryMovementMappingProfile : Profile
{
    public InventoryMovementMappingProfile()
    {
        // Entidad → DTO para mostrar el historial.
        // El nombre y la unidad se toman del insumo relacionado (navegación InventoryItem).
        // No hay mapeo DTO → Entidad: los movimientos solo se crean con
        // InventoryItem.RegisterEntry / RegisterExit.
        CreateMap<InventoryMovement, InventoryMovementDto>()
            .ForMember(dest => dest.InventoryItemName, opt => opt.MapFrom(src => src.InventoryItem!.Name))
            .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.InventoryItem!.Unit));
    }
}
