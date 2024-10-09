using AutoMapper;
using Entities.Administration;
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
        CreateMap<Rol, RolGetDTO>()
            .ForMember(dest => dest.IdRole, opt => opt.MapFrom(src => src.Id_Role))
            .ForMember(dest => dest.NameRole, opt => opt.MapFrom(src => src.Name_Role));

        CreateMap<PasswordResetRequestDTO, Employee>();


        // Mapear PaymentReceipt a PaymentReceiptDto
        CreateMap<PaymentReceipt, PaymentReceiptDto>()
            .ForMember(dest => dest.EmployeeFullName, opt => opt.MapFrom(src => $"{src.Employee.First_Name} {src.Employee.Last_Name1} {src.Employee.Last_Name2}"))
            .ForMember(dest => dest.EmployeeEmail, opt => opt.MapFrom(src => src.Employee.Email))
            .ForMember(dest => dest.Profession, opt => opt.MapFrom(src => src.Employee.Profession.Name_Profession))
            .ForMember(dest => dest.SalaryType, opt => opt.MapFrom(src => src.Employee.TypeOfSalary.Name_TypeOfSalary))
            .ForMember(dest => dest.DeductionsList, opt => opt.MapFrom(src => src.DeductionsList))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ReverseMap();




        // Mapear Deduction a DeductionDto
        CreateMap<Deduction, DeductionDto>()
            .ForMember(dest => dest.PaymentReceiptId, opt => opt.MapFrom(src => src.PaymentReceiptId))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ReverseMap();  // Permite el mapeo inverso si es necesario

        // Mapear PaymentReceiptCreateDto a PaymentReceipt
        CreateMap<PaymentReceiptCreateDto, PaymentReceipt>()
            .ForMember(dest => dest.EmployeeDni, opt => opt.MapFrom(src => src.EmployeeDni))
            .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => src.PaymentDate))
            .ForMember(dest => dest.Salary, opt => opt.MapFrom(src => src.Salary))
            .ForMember(dest => dest.Overtime, opt => opt.MapFrom(src => src.Overtime))
            .ForMember(dest => dest.GrossAmount, opt => opt.MapFrom(src => src.GrossAmount))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
            .ForMember(dest => dest.DeductionsList, opt => opt.Ignore())  // Las deducciones se gestionan por separado
            .ReverseMap();

        // Mapear DeductionCreateDto a Deduction
        CreateMap<DeductionCreateDto, Deduction>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.PaymentReceiptId, opt => opt.Ignore())  // Se asigna al agregar a un PaymentReceipt
            .ReverseMap();



        // Mapeo de ResidentCreateDto a Resident
        CreateMap<ResidentCreateDto, Resident>()
            .ForMember(dest => dest.Sexo, opt => opt.MapFrom(src => src.Sexo));

        // Mapeo para obtener información completa de un residente
        CreateMap<Resident, ResidentGetDto>()
        .ForMember(dest => dest.GuardianName, opt => opt.MapFrom(src => $"{src.Guardian.Name_GD} {src.Guardian.Lastname1_GD} {src.Guardian.Lastname2_GD}"))
        .ForMember(dest => dest.Edad, opt => opt.Ignore())  // No se necesita mapear porque es calculado
        .ForMember(dest => dest.RoomNumber, opt => opt.MapFrom(src => src.Room.RoomNumber))
        .ForMember(dest => dest.DependencyLevel, opt => opt.MapFrom(src => src.DependencyHistories
            .OrderByDescending(dh => dh.Id_History) // Tomar el nivel más reciente
            .FirstOrDefault().DependencyLevel.LevelName));

        // Mapeo adicional para añadir un residente desde Applicant
        CreateMap<ResidentFromApplicantDto, Resident>()
            .ForMember(dest => dest.Sexo, opt => opt.MapFrom(src => src.Sexo))
            .ForMember(dest => dest.EntryDate, opt => opt.MapFrom(src => src.EntryDate));

    }
}
