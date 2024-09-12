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

        CreateMap<Employee, EmployeeGetDTO>()
       .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.First_Name))
       .ForMember(dest => dest.LastName1, opt => opt.MapFrom(src => src.Last_Name1))
       .ForMember(dest => dest.LastName2, opt => opt.MapFrom(src => src.Last_Name2))
       .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone_Number))
       .ForMember(dest => dest.EmergencyPhone, opt => opt.MapFrom(src => src.Emergency_Phone))
       .ForMember(dest => dest.ProfessionName, opt => opt.MapFrom(src => src.Profession.Name_Profession))
       .ForMember(dest => dest.TypeOfSalaryName, opt => opt.MapFrom(src => src.TypeOfSalary.Name_TypeOfSalary));
        // Corrigiendo el mapeo con el nombre correcto


        CreateMap<EmployeeCreateDTO, Employee>()
      .ForMember(dest => dest.First_Name, opt => opt.MapFrom(src => src.FirstName))
      .ForMember(dest => dest.Last_Name1, opt => opt.MapFrom(src => src.LastName1))
      .ForMember(dest => dest.Last_Name2, opt => opt.MapFrom(src => src.LastName2))
      .ForMember(dest => dest.Phone_Number, opt => opt.MapFrom(src => src.PhoneNumber))
      .ForMember(dest => dest.Emergency_Phone, opt => opt.MapFrom(src => src.EmergencyPhone))
      .ForMember(dest => dest.Id_TypeOfSalary, opt => opt.MapFrom(src => src.TypeOfSalaryId))
      .ForMember(dest => dest.Id_Profession, opt => opt.MapFrom(src => src.ProfessionId));


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
