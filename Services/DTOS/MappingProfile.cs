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


        CreateMap<ApplicationForm, ApplicationFormDto>()
       .ForMember(dest => dest.Name_AP, opt => opt.MapFrom(src => src.Applicant.Name_AP))
       .ForMember(dest => dest.Lastname1_AP, opt => opt.MapFrom(src => src.Applicant.Lastname1_AP))
       .ForMember(dest => dest.Lastname2_AP, opt => opt.MapFrom(src => src.Applicant.Lastname2_AP))
       .ForMember(dest => dest.Age_AP, opt => opt.MapFrom(src => src.Applicant.Age_AP))
       .ForMember(dest => dest.Cedula_AP, opt => opt.MapFrom(src => src.Applicant.Cedula_AP))
       .ForMember(dest => dest.Name_GD, opt => opt.MapFrom(src => src.Guardian.Name_GD))
       .ForMember(dest => dest.Lastname1_GD, opt => opt.MapFrom(src => src.Guardian.Lastname1_GD))
       .ForMember(dest => dest.Lastname2_GD, opt => opt.MapFrom(src => src.Guardian.Lastname2_GD))
       .ForMember(dest => dest.Cedula_GD, opt => opt.MapFrom(src => src.Guardian.Cedula_GD))
       .ForMember(dest => dest.Phone_GD, opt => opt.MapFrom(src => src.Guardian.Phone_GD))
       .ForMember(dest => dest.Email_GD, opt => opt.MapFrom(src => src.Guardian.Email_GD))
       .ForMember(dest => dest.ApplicationDate, opt => opt.MapFrom(src => src.ApplicationDate))
       // Mapear el nombre del estado, no la entidad completa
       .ForMember(dest => dest.Status_Name, opt => opt.MapFrom(src => src.ApplicationStatus.Status_Name));

        // Mapeo de ApplicationFormCreateDto a ApplicationForm (POST)
        CreateMap<ApplicationFormCreateDto, ApplicationForm>()
            .ForMember(dest => dest.Applicant, opt => opt.MapFrom(src => new Applicant
            {
                Name_AP = src.Name_AP,
                Lastname1_AP = src.Lastname1_AP,
                Lastname2_AP = src.Lastname2_AP,
                Age_AP = src.Age_AP,
                Cedula_AP = src.Cedula_AP
            }))


            .ForMember(dest => dest.Guardian, opt => opt.MapFrom(src => new Guardian
            {
                Name_GD = src.Name_GD,
                Lastname1_GD = src.Lastname1_GD,
                Lastname2_GD = src.Lastname2_GD,
                Cedula_GD = src.Cedula_GD,
                Phone_GD = src.Phone_GD,
                Email_GD = src.Email_GD
            }));

       CreateMap<ApplicationFormCreateDto, ApplicationForm>()
    .ForMember(dest => dest.Applicant, opt => opt.Ignore())   // Se gestionan en el servicio
    .ForMember(dest => dest.Guardian, opt => opt.Ignore());   // Se gestionan en el servicio



        CreateMap<ApplicationFormCreateDto, Applicant>()
    .ForMember(dest => dest.Name_AP, opt => opt.MapFrom(src => src.Name_AP))
    .ForMember(dest => dest.Lastname1_AP, opt => opt.MapFrom(src => src.Lastname1_AP))
    .ForMember(dest => dest.Lastname2_AP, opt => opt.MapFrom(src => src.Lastname2_AP))
    .ForMember(dest => dest.Age_AP, opt => opt.MapFrom(src => src.Age_AP))
    .ForMember(dest => dest.Cedula_AP, opt => opt.MapFrom(src => src.Cedula_AP));

        CreateMap<ApplicationFormCreateDto, Guardian>()
            .ForMember(dest => dest.Name_GD, opt => opt.MapFrom(src => src.Name_GD))
            .ForMember(dest => dest.Lastname1_GD, opt => opt.MapFrom(src => src.Lastname1_GD))
            .ForMember(dest => dest.Lastname2_GD, opt => opt.MapFrom(src => src.Lastname2_GD))
            .ForMember(dest => dest.Cedula_GD, opt => opt.MapFrom(src => src.Cedula_GD))
            .ForMember(dest => dest.Phone_GD, opt => opt.MapFrom(src => src.Phone_GD))
            .ForMember(dest => dest.Email_GD, opt => opt.MapFrom(src => src.Email_GD));
    }
}
