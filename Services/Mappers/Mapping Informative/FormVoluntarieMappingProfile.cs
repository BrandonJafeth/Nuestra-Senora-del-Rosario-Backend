using AutoMapper;
using Domain.Entities.Informative;
using Infrastructure.Services.Informative.DTOS;
using Infrastructure.Services.Informative.DTOS.CreatesDto;

namespace Infrastructure.Services.Informative.MappingProfiles
{
    public class FormVoluntarieMappingProfile : Profile
    {
        public FormVoluntarieMappingProfile()
        {
            // Mapea de la entidad FormVoluntarie al DTO de salida.
            CreateMap<FormVoluntarie, FormVoluntarieDto>()
                .ForMember(dest => dest.Name_voluntarieType, opt => opt.MapFrom(src => src.VoluntarieType.Name_VoluntarieType))
                .ForMember(dest => dest.Status_Name, opt => opt.MapFrom(src => src.Status.Status_Name));

            // Si necesitas mapping inverso o del DTO de creación a la entidad:
            CreateMap<FormVoluntarieCreateDto, FormVoluntarie>();
        }
    }
}
