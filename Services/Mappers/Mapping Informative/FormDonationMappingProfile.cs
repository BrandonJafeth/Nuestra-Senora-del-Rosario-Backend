using AutoMapper;
using Domain.Entities.Informative;
using Infrastructure.Services.Informative.DTOS;
using Infrastructure.Services.Informative.DTOS.CreatesDto;

namespace Infrastructure.Services.Informative.MappingProfiles
{
    public class FormDonationMappingProfile : Profile
    {
        public FormDonationMappingProfile()
        {
            // Mapea de la entidad al DTO de salida.
            CreateMap<FormDonation, FormDonationDto>()
                .ForMember(dest => dest.DonationType, opt => opt.MapFrom(src => src.DonationType.Name_DonationType))
                .ForMember(dest => dest.MethodDonation, opt => opt.MapFrom(src => src.MethodDonation.Name_MethodDonation))
                .ForMember(dest => dest.Status_Name, opt => opt.MapFrom(src => src.Status.Status_Name));

            // Mapea del DTO de creación a la entidad.
            CreateMap<FormDonationCreateDto, FormDonation>();
        }
    }
}
