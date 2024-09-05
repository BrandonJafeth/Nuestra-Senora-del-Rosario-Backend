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


        /// Mapeo para VoluntarieType con su relación a FormVoluntaries
        CreateMap<VoluntarieType, VoluntarieTypeDto>().ReverseMap();

        // Mapeo para FormVoluntarie (GET)
        CreateMap<FormVoluntarie, FormVoluntarieDto>()
            .ForMember(dest => dest.VoluntarieTypeName, opt => opt.MapFrom(src => src.VoluntarieType.Name_VoluntarieType)); // Solo el nombre del tipo de voluntariado

        // Mapeo inverso de FormVoluntarieCreateDto a FormVoluntarie (POST)
        CreateMap<FormVoluntarieCreateDto, FormVoluntarie>()
            .ForMember(dest => dest.VoluntarieType, opt => opt.Ignore()) // Ignoramos el objeto VoluntarieType completo
            .ForMember(dest => dest.Id_VoluntarieType, opt => opt.MapFrom(src => src.VoluntarieTypeId)); // Mapear el ID directamente


    }
}
