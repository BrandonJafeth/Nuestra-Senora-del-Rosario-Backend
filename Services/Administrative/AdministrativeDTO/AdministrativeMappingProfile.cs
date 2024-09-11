using AutoMapper;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using System.Data;

public class AdministrativeMappingProfile : Profile
{
    public AdministrativeMappingProfile()
    {
        // Mapping para el usuario (User) desde su DTO de creación
        CreateMap<UserCreateDTO, User>();

        // Mapping para empleado y su DTO de obtención
        CreateMap<Employee, EmployeeGetDTO>()
         .ForMember(dest => dest.ProfessionName, opt => opt.MapFrom(src => src.Profession.Name_Profession))
         .ForMember(dest => dest.TypeOfSalaryName, opt => opt.MapFrom(src => src.TypeOfSalary.Name_TypeOfSalary));

        CreateMap<User, UserGetDTO>()
           .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.First_Name + " " + src.Employee.Last_Name1 + " " + src.Employee.Last_Name2))  // Nombre completo del empleado
           .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src =>
               src.Employee.EmployeeRoles.FirstOrDefault().Rol.Name_Role))  // Obtener el primer rol del usuario
           .ForMember(dest => dest.Is_Active, opt => opt.MapFrom(src => src.Is_Active));

        // Mapping para Rol
        CreateMap<RolCreateDTO, Rol>();
        CreateMap<Rol, RolGetDTO>();
    }
}
