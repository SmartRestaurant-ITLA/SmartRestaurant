using AutoMapper;
using SmartRestaurant.Application.DTOs.Sales;
using SmartRestaurant.Domain.Entities;

namespace SmartRestaurant.Application.Mappings.EntitiesAndDtos;

public class DailySaleMappingProfile : Profile
{
    public DailySaleMappingProfile()
    {
        CreateMap<DailySale, DailySaleDto>();

        CreateMap<CreateDailySaleDto, DailySale>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }
}