using AutoMapper;
using Domain.Entities.Informative;
using Infrastructure.Services.Informative.DTOS;
using Infrastructure.Services.Informative.DTOS.CreatesDto;

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

        // Mapeo de MethodDonation a MethodDonationDto
        CreateMap<MethodDonation, MethodDonationDto>();

        // Mapeo para FormDonation a FormDonationDto (GET)
        CreateMap<FormDonation, FormDonationDto>()
            .ForMember(dest => dest.DonationType, opt => opt.MapFrom(src => src.DonationType.Name_DonationType))
            .ForMember(dest => dest.MethodDonation, opt => opt.MapFrom(src => src.MethodDonation.Name_MethodDonation))
            .ForMember(dest => dest.Status_Name, opt => opt.MapFrom(src => src.Status.Status_Name));  // Nombre del estado

        // Mapeo de FormDonationCreateDto a FormDonation (POST)
        CreateMap<FormDonationCreateDto, FormDonation>();

        /// Mapeo para VoluntarieType con su relación a FormVoluntaries
        CreateMap<VoluntarieType, VoluntarieTypeDto>().ReverseMap();

        // Mapeo para FormVoluntarie (GET)
        CreateMap<FormVoluntarie, FormVoluntarieDto>()
       .ForMember(dest => dest.Name_voluntarieType, opt => opt.MapFrom(src => src.VoluntarieType.Name_VoluntarieType))
       .ForMember(dest => dest.Status_Name, opt => opt.MapFrom(src => src.Status.Status_Name));  // Mapear el nombre del estado

        // Mapeo inverso de FormVoluntarieCreateDto a FormVoluntarie (POST)
        CreateMap<FormVoluntarieCreateDto, FormVoluntarie>()
            .ForMember(dest => dest.VoluntarieType, opt => opt.Ignore()) // Ignoramos el objeto VoluntarieType completo
            .ForMember(dest => dest.Id_VoluntarieType, opt => opt.MapFrom(src => src.VoluntarieTypeId)); // Mapear el ID directamente


        // De la Entidad a DTO para GET
        CreateMap<ApplicationForm, ApplicationFormDto>()
            // Example: si quieres mapear un "status_name":
            .ForMember(dest => dest.Status_Name, opt => opt.MapFrom(
               src => src.ApplicationStatus.Status_Name
            ));

        // De DTO para POST a la Entidad
        CreateMap<ApplicationFormCreateDto, ApplicationForm>()
            // Ajustas lo que quieras ignorar
            .ForMember(dest => dest.Id_ApplicationForm, opt => opt.Ignore())
            .ForMember(dest => dest.ApplicationDate, opt => opt.Ignore())
            .ForMember(dest => dest.Id_Status, opt => opt.Ignore());
    }
}
