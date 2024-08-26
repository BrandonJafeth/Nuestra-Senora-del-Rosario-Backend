using AutoMapper;
using Entities.Informative;
using Services.DTOS;


public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<NavbarItem, NavbarItemDto>();
    
    }
}
