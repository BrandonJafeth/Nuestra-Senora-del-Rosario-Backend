using AutoMapper;
using Entities.Informative;
using Services.DTOS;


public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<NavbarItem, NavbarItemDto>();
        CreateMap<GalleryCategory, GalleryCategoryDto>();
        CreateMap<GalleryItem, GalleryItemDto>().ReverseMap();

        // Mapeo para DonationType
        CreateMap<DonationType, DonationTypeDto>()
            .ForMember(dest => dest.MethodDonations, opt => opt.MapFrom(src => src.MethodDonations));

        // Mapeo para MethodDonation (sin la referencia al DonationType)
        CreateMap<MethodDonation, MethodDonationDto>();

        // Mapeo entre FormDonation y FormDonationDto
        CreateMap<FormDonation, FormDonationDto>()
            .ForMember(dest => dest.DonationType, opt => opt.MapFrom(src => src.DonationType))
            .ForMember(dest => dest.MethodDonation, opt => opt.MapFrom(src => src.MethodDonation))
            .ReverseMap();

    }
}
