using AutoMapper;
using Entities.Informative;
using Services.DTOS;
using Services.DTOS.CreatesDto;


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

        // Map de FormDonation a FormDonationDto (GET)
        CreateMap<FormDonation, FormDonationDto>()
            .ForMember(dest => dest.DonationType, opt => opt.MapFrom(src => src.DonationType.Name_DonationType))
            .ForMember(dest => dest.MethodDonation, opt => opt.MapFrom(src => src.MethodDonation.Name_MethodDonation));

        // Map de FormDonationCreateDto a FormDonation (POST)
        CreateMap<FormDonationCreateDto, FormDonation>();

    }
}
