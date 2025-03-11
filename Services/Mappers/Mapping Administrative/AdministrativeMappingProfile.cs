using AutoMapper;
using Domain.Entities.Administration;
using Domain.Entities.Informative;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Infrastructure.Services.Informative.DTOS.CreatesDto;
using System.Data;

public class AdministrativeMappingProfile : Profile
{
    public AdministrativeMappingProfile()
    {
   
        // Mapping para UserCreateFromEmployeeDto -> User
        CreateMap<UserCreateFromEmployeeDto, User>()
            .ForMember(dest => dest.DNI, opt => opt.MapFrom(src => src.DniEmployee))
            .ForMember(dest => dest.Is_Active, opt => opt.MapFrom(src => src.IsActive));
            

        CreateMap<UserCreateDto, User>();

        CreateMap<Employee, EmployeeGetDTO>()
       .ForMember(dest => dest.First_Name, opt => opt.MapFrom(src => src.First_Name))
       .ForMember(dest => dest.Last_Name1, opt => opt.MapFrom(src => src.Last_Name1))
       .ForMember(dest => dest.Last_Name2, opt => opt.MapFrom(src => src.Last_Name2))
       .ForMember(dest => dest.Phone_Number, opt => opt.MapFrom(src => src.Phone_Number))
       .ForMember(dest => dest.Emergency_Phone, opt => opt.MapFrom(src => src.Emergency_Phone))
       .ForMember(dest => dest.ProfessionName, opt => opt.MapFrom(src => src.Profession.Name_Profession))
       .ForMember(dest => dest.TypeOfSalaryName, opt => opt.MapFrom(src => src.TypeOfSalary.Name_TypeOfSalary));
        // Corrigiendo el mapeo con el nombre correcto


        CreateMap<EmployeeCreateDTO, Employee>()
      .ForMember(dest => dest.First_Name, opt => opt.MapFrom(src => src.First_Name))
      .ForMember(dest => dest.Last_Name1, opt => opt.MapFrom(src => src.Last_Name1))
      .ForMember(dest => dest.Last_Name2, opt => opt.MapFrom(src => src.Last_Name2))
      .ForMember(dest => dest.Phone_Number, opt => opt.MapFrom(src => src.Phone_Number))
      .ForMember(dest => dest.Emergency_Phone, opt => opt.MapFrom(src => src.Emergency_Phone))
      .ForMember(dest => dest.Id_TypeOfSalary, opt => opt.MapFrom(src => src.Id_TypeOfSalary))
      .ForMember(dest => dest.Id_Profession, opt => opt.MapFrom(src => src.Id_Profession));

        CreateMap<User, UserGetDto>()
                   .ForMember(dest => dest.Roles, opt => opt.MapFrom(src =>
                       src.UserRoles.Select(ur => ur.Role.Name_Role).ToList()));

        // Mapping para Rol
        CreateMap<RolCreateDTO, Rol>()
        .ForMember(dest => dest.Name_Role, opt => opt.MapFrom(src => src.NameRole));



        CreateMap<Rol, RolGetDTO>()
            .ForMember(dest => dest.IdRole, opt => opt.MapFrom(src => src.Id_Role))
            .ForMember(dest => dest.NameRole, opt => opt.MapFrom(src => src.Name_Role));

        CreateMap<PasswordResetRequestDTO, Employee>();


        // Mapear PaymentReceipt a PaymentReceiptDto
        CreateMap<PaymentReceipt, PaymentReceiptDto>()
            .ForMember(dest => dest.Dni, opt => opt.MapFrom(src => src.Employee.Dni))
            .ForMember(dest => dest.EmployeeFullName, opt => opt.MapFrom(src => $"{src.Employee.First_Name} {src.Employee.Last_Name1} {src.Employee.Last_Name2}"))
            .ForMember(dest => dest.EmployeeEmail, opt => opt.MapFrom(src => src.Employee.Email))
            .ForMember(dest => dest.Profession, opt => opt.MapFrom(src => src.Employee.Profession.Name_Profession))
            .ForMember(dest => dest.SalaryType, opt => opt.MapFrom(src => src.Employee.TypeOfSalary.Name_TypeOfSalary))
            .ForMember(dest => dest.DeductionsList, opt => opt.MapFrom(src => src.DeductionsList))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))

            // Mapeo de los nuevos campos
            .ForMember(dest => dest.WorkedDays, opt => opt.MapFrom(src => src.WorkedDays))
            .ForMember(dest => dest.GrossIncome, opt => opt.MapFrom(src => src.GrossIncome))  // Ahora se mapea desde GrossIncome
            .ForMember(dest => dest.TotalExtraHoursAmount, opt => opt.MapFrom(src => src.TotalExtraHoursAmount))
            .ForMember(dest => dest.ExtraHourRate, opt => opt.MapFrom(src => src.ExtraHourRate))
            .ForMember(dest => dest.DoubleExtras, opt => opt.MapFrom(src => src.DoubleExtras))
            .ForMember(dest => dest.NightHours, opt => opt.MapFrom(src => src.NightHours))
            .ForMember(dest => dest.Adjustments, opt => opt.MapFrom(src => src.Adjustments))
            .ForMember(dest => dest.Incapacity, opt => opt.MapFrom(src => src.Incapacity))
            .ForMember(dest => dest.Absence, opt => opt.MapFrom(src => src.Absence))
            .ForMember(dest => dest.VacationDays, opt => opt.MapFrom(src => src.VacationDays))

            .ReverseMap();

        // Mapear Deduction a DeductionDto
        CreateMap<Deduction, DeductionDto>()
            .ForMember(dest => dest.PaymentReceiptId, opt => opt.MapFrom(src => src.PaymentReceiptId))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ReverseMap();  // Permite el mapeo inverso si es necesario

        // Mapear PaymentReceiptCreateDto a PaymentReceipt
        CreateMap<PaymentReceiptCreateDto, PaymentReceipt>()
            .ForMember(dest => dest.Id_Employee, opt => opt.MapFrom(src => src.Id_Employee))
            .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => src.PaymentDate))
            .ForMember(dest => dest.Salary, opt => opt.MapFrom(src => src.Salary))
            .ForMember(dest => dest.Overtime, opt => opt.MapFrom(src => src.Overtime))

            // Mapeo de los nuevos campos
            .ForMember(dest => dest.WorkedDays, opt => opt.MapFrom(src => src.WorkedDays))
            .ForMember(dest => dest.GrossIncome, opt => opt.MapFrom(src => src.GrossIncome))  // Mapeado directamente a GrossIncome
            .ForMember(dest => dest.TotalExtraHoursAmount, opt => opt.MapFrom(src => src.TotalExtraHoursAmount))
            .ForMember(dest => dest.ExtraHourRate, opt => opt.MapFrom(src => src.ExtraHourRate))
            .ForMember(dest => dest.DoubleExtras, opt => opt.MapFrom(src => src.DoubleExtras))
            .ForMember(dest => dest.NightHours, opt => opt.MapFrom(src => src.NightHours))
            .ForMember(dest => dest.Adjustments, opt => opt.MapFrom(src => src.Adjustments))
            .ForMember(dest => dest.Incapacity, opt => opt.MapFrom(src => src.Incapacity))
            .ForMember(dest => dest.Absence, opt => opt.MapFrom(src => src.Absence))
            .ForMember(dest => dest.VacationDays, opt => opt.MapFrom(src => src.VacationDays))

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
            .ForMember(dest => dest.Sexo, opt => opt.MapFrom(src => src.Sexo))
               .ForMember(dest => dest.Location_RD, opt => opt.MapFrom(src => src.Location_RD));

        // Mapeo para obtener información completa de un residente
        CreateMap<Resident, ResidentGetDto>()
          .ForMember(dest => dest.GuardianName, opt => opt.MapFrom(src => $"{src.Guardian.Name_GD} {src.Guardian.Lastname1_GD} {src.Guardian.Lastname2_GD}"))
          .ForMember(dest => dest.GuardianPhone, opt => opt.MapFrom(src => src.Guardian.Phone_GD)) // Mapeo para el teléfono del guardián
          .ForMember(dest => dest.RoomNumber, opt => opt.MapFrom(src => src.Room.RoomNumber))
          .ForMember(dest => dest.DependencyLevel, opt => opt.MapFrom(src => src.DependencyHistories.OrderByDescending(dh => dh.Id_History).FirstOrDefault().DependencyLevel.LevelName))
          .ForMember(dest => dest.Edad, opt => opt.Ignore())  // Calculado dinámicamente
          .ForMember(dest => dest.Location_RD, opt => opt.MapFrom(src => src.Location_RD));  // Mapeo de la nueva propiedad Location

        // Mapeo adicional para añadir un residente desde Applicant
        CreateMap<ResidentFromApplicantDto, Resident>()
            .ForMember(dest => dest.Sexo, opt => opt.MapFrom(src => src.Sexo))
            .ForMember(dest => dest.EntryDate, opt => opt.MapFrom(src => src.EntryDate))
            .ForMember(dest => dest.FechaNacimiento, opt => opt.MapFrom(src => src.FechaNacimiento));




        // Mapeo de AppointmentPostDto a Appointment (POST)
        CreateMap<AppointmentPostDto, Appointment>()
            .ForMember(dest => dest.Id_Resident, opt => opt.MapFrom(src => src.Id_Resident))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
            .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Time))
            .ForMember(dest => dest.Id_HC, opt => opt.MapFrom(src => src.Id_HC))
            .ForMember(dest => dest.Id_Specialty, opt => opt.MapFrom(src => src.Id_Specialty))
            .ForMember(dest => dest.Id_Companion, opt => opt.MapFrom(src => src.Id_Companion))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes));

        // Mapeo de AppointmentUpdateDto a Appointment (PATCH / PUT)
        CreateMap<AppointmentUpdateDto, Appointment>()
            .ForMember(dest => dest.Id_Appointment, opt => opt.MapFrom(src => src.Id_Appointment))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        // Solo actualiza los valores que no sean nulos.

        // Mapeo de Appointment a AppointmentGetDto (GET)
        CreateMap<Appointment, AppointmentGetDto>()
            .ForMember(dest => dest.Id_Appointment, opt => opt.MapFrom(src => src.Id_Appointment))
            .ForMember(dest => dest.ResidentFullName, opt => opt.MapFrom(src =>
                $"{src.Resident.Name_RD} {src.Resident.Lastname1_RD} {src.Resident.Lastname2_RD}"))
            .ForMember(dest => dest.ResidentCedula, opt => opt.MapFrom(src => src.Resident.Cedula_RD))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
            .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Time))
            .ForMember(dest => dest.SpecialtyName, opt => opt.MapFrom(src => src.Specialty.Name_Specialty))
            .ForMember(dest => dest.HealthcareCenterName, opt => opt.MapFrom(src => src.HealthcareCenter.Name_HC))
            .ForMember(dest => dest.CompanionName, opt => opt.MapFrom(src =>
                $"{src.Companion.First_Name} {src.Companion.Last_Name1} {src.Companion.Last_Name2}"))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.AppointmentStatus.Name_StatusAP))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes));

        // AppointmentStatus Mapping
        CreateMap<AppointmentStatus, AppointmentStatusGetDto>()
            .ForMember(dest => dest.Id_StatusAP, opt => opt.MapFrom(src => src.Id_StatusAP))
            .ForMember(dest => dest.Name_StatusAP, opt => opt.MapFrom(src => src.Name_StatusAP));

        CreateMap<AppointmentStatusCreateUpdateDto, AppointmentStatus>()
            .ForMember(dest => dest.Name_StatusAP, opt => opt.MapFrom(src => src.Name_StatusAP));

        // HealthcareCenter Mapping
        CreateMap<HealthcareCenter, HealthcareCenterGetDto>()
            .ForMember(dest => dest.Id_HC, opt => opt.MapFrom(src => src.Id_HC))
            .ForMember(dest => dest.Name_HC, opt => opt.MapFrom(src => src.Name_HC))
            .ForMember(dest => dest.Location_HC, opt => opt.MapFrom(src => src.Location_HC))
            .ForMember(dest => dest.Type_HC, opt => opt.MapFrom(src => src.Type_HC));

        CreateMap<HealthcareCenterCreateUpdateDto, HealthcareCenter>()
            .ForMember(dest => dest.Name_HC, opt => opt.MapFrom(src => src.Name_HC))
            .ForMember(dest => dest.Location_HC, opt => opt.MapFrom(src => src.Location_HC))
            .ForMember(dest => dest.Type_HC, opt => opt.MapFrom(src => src.Type_HC));

        // Specialty Mapping
        CreateMap<Specialty, SpecialtyGetDto>()
            .ForMember(dest => dest.Id_Specialty, opt => opt.MapFrom(src => src.Id_Specialty))
            .ForMember(dest => dest.Name_Specialty, opt => opt.MapFrom(src => src.Name_Specialty));

        CreateMap<SpecialtyCreateUpdateDto, Specialty>()
            .ForMember(dest => dest.Name_Specialty, opt => opt.MapFrom(src => src.Name_Specialty));



        // Mapeo para Notificaciones
        CreateMap<Notification, NotificationGetDto>();


        CreateMap<EmployeeCreateDTO, Employee>();

        // Mapping para Employee -> EmployeeGetDto
        CreateMap<Employee, EmployeeGetDTO>()
            .ForMember(dest => dest.TypeOfSalaryName, opt => opt.MapFrom(src => src.TypeOfSalary.Name_TypeOfSalary))
            .ForMember(dest => dest.ProfessionName, opt => opt.MapFrom(src => src.Profession.Name_Profession));


        CreateMap<UnitOfMeasure, UnitOfMeasureGetDTO>()
    .ForMember(dest => dest.UnitOfMeasureID, opt => opt.MapFrom(src => src.UnitOfMeasureID))
    .ForMember(dest => dest.NombreUnidad, opt => opt.MapFrom(src => src.UnitName));

        CreateMap<UnitOfMeasureCreateDTO, UnitOfMeasure>()
            .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.NombreUnidad));


        CreateMap<Category, CategoryGetDTO>()
    .ForMember(dest => dest.CategoryID, opt => opt.MapFrom(src => src.CategoryID))
    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryName));

        CreateMap<CategoryCreateDTO, Category>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryName));

        CreateMap<ProductCreateDTO, Product>()
    .ForMember(dest => dest.TotalQuantity, opt => opt.MapFrom(src => src.InitialQuantity))
    .ForMember(dest => dest.CategoryID, opt => opt.MapFrom(src => src.CategoryID))
    .ForMember(dest => dest.UnitOfMeasureID, opt => opt.MapFrom(src => src.UnitOfMeasureID));


        CreateMap<Product, ProductGetDTO>()
      .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : "Unknown"))
      .ForMember(dest => dest.UnitOfMeasure, opt => opt.MapFrom(src => src.UnitOfMeasure != null ? src.UnitOfMeasure.UnitName : "Unknown"));


        CreateMap<InventoryCreateDTO, Inventory>()
    .ForMember(dest => dest.ProductID, opt => opt.MapFrom(src => src.ProductID))
    .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
    .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
    .ForMember(dest => dest.MovementType, opt => opt.MapFrom(src => src.MovementType));


        // Mapeo de Inventory a InventoryGetDTO, eliminando CategoryName y UnitOfMeasure
        CreateMap<Inventory, InventoryGetDTO>()
            .ForMember(dest => dest.ProductID, opt => opt.MapFrom(src => src.ProductID))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
            .ForMember(dest => dest.MovementType, opt => opt.MapFrom(src => src.MovementType))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

        // Mapeo de Inventory a InventoryReportDTO, solo con UnitOfMeasure
        CreateMap<Inventory, InventoryReportDTO>()
            .ForMember(dest => dest.ProductID, opt => opt.MapFrom(src => src.ProductID))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.TotalInStock, opt => opt.Ignore()) // Calculado fuera de AutoMapper
            .ForMember(dest => dest.TotalIngresos, opt => opt.Ignore()) // Calculado fuera de AutoMapper
            .ForMember(dest => dest.TotalEgresos, opt => opt.Ignore()) // Calculado fuera de AutoMapper
            .ForMember(dest => dest.UnitOfMeasure, opt => opt.MapFrom(src => src.Product.UnitOfMeasure.UnitName));


        // Mapeo de Inventory a InventoryDailyReportDTO
        CreateMap<Inventory, InventoryDailyReportDTO>()
            .ForMember(dest => dest.ProductID, opt => opt.MapFrom(src => src.ProductID))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.UnitOfMeasure, opt => opt.MapFrom(src => src.Product.UnitOfMeasure.UnitName))
            // Los campos TotalIngresos y TotalEgresos se calcularán manualmente en el servicio.
            .ForMember(dest => dest.TotalIngresos, opt => opt.Ignore())
            .ForMember(dest => dest.TotalEgresos, opt => opt.Ignore());


        CreateMap<MedicationSpecific, MedicationSpecificGetDto>()
    .ForMember(dest => dest.UnitOfMeasureName, opt => opt.MapFrom(src => src.UnitOfMeasure.UnitName))
    .ForMember(dest => dest.RouteName, opt => opt.MapFrom(src => src.AdministrationRoute.RouteName));

        CreateMap<MedicationSpecificCreateDto, MedicationSpecific>();
        CreateMap<MedicationSpecificUpdateDto, MedicationSpecific>();


        CreateMap<AdministrationRoute, AdministrationRouteGetDto>()
    .ForMember(dest => dest.Id_AdministrationRoute, opt => opt.MapFrom(src => src.Id_AdministrationRoute))
    .ForMember(dest => dest.RouteName, opt => opt.MapFrom(src => src.RouteName));
  
        CreateMap<AdministrationRouteCreateDto, AdministrationRoute>();
        CreateMap<AdministrationRouteUpdateDto, AdministrationRoute>();


        CreateMap<Pathology, PathologyGetDto>()
     .ForMember(dest => dest.Id_Pathology, opt => opt.MapFrom(src => src.Id_Pathology))
      .ForMember(dest => dest.Name_Pathology, opt => opt.MapFrom(src => src.Name_Pathology));




        CreateMap<PathologyCreateDto, Pathology>();
        CreateMap<PathologyUpdateDto, Pathology>();


        CreateMap<ResidentMedication, ResidentMedicationGetDto>()
             .ForMember(dest => dest.ResidentFullName, opt => opt.MapFrom(src => $"{src.Resident.Name_RD} {src.Resident.Lastname1_RD} {src.Resident.Lastname2_RD}"))
            .ForMember(dest => dest.Name_MedicamentSpecific, opt => opt.MapFrom(src => src.MedicationSpecific.Name_MedicamentSpecific))
            .ForMember(dest => dest.UnitOfMeasureName, opt => opt.MapFrom(src => src.MedicationSpecific.UnitOfMeasure.UnitName));


        CreateMap<ResidentMedicationCreateDto, ResidentMedication>();

        CreateMap<ResidentMedicationUpdateDto, ResidentMedication>();


        CreateMap<ResidentPathology, ResidentPathologyGetDto>()
            .ForMember(dest => dest.DiagnosisDate, opt => opt.MapFrom(src => src.DiagnosisDate.ToString("yyyy-MM-dd")))
            .ForMember(dest => dest.RegisterDate, opt => opt.MapFrom(src => src.RegisterDate.ToString("yyyy-MM-dd")))
            .ForMember(dest => dest.ResidentFullName, opt => opt.MapFrom(src => $"{src.Resident.Name_RD} {src.Resident.Lastname1_RD} {src.Resident.Lastname2_RD}"))
            .ForMember(dest => dest.Name_Pathology, opt => opt.MapFrom(src => src.Pathology.Name_Pathology));

        CreateMap<ResidentPathologyCreateDto, ResidentPathology>()
            .ForMember(dest => dest.DiagnosisDate, opt => opt.MapFrom(src => DateOnly.Parse(src.DiagnosisDate)))
            .ForMember(dest => dest.RegisterDate, opt => opt.MapFrom(src => DateOnly.Parse(src.RegisterDate)));

        CreateMap<ResidentPathologyUpdateDto, ResidentPathology>()
            .ForMember(dest => dest.DiagnosisDate, opt => opt.MapFrom(src => DateOnly.Parse(src.DiagnosisDate)))
            .ForMember(dest => dest.RegisterDate, opt => opt.MapFrom(src => DateOnly.Parse(src.RegisterDate)));


        // Mapear de Resident a ResidentMinimalInfoDto
        CreateMap<Resident, ResidentMinimalInfoDto>()
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src =>
                DateTime.Now.Month > src.FechaNacimiento.Month ||
                (DateTime.Now.Month == src.FechaNacimiento.Month && DateTime.Now.Day >= src.FechaNacimiento.Day)
                    ? DateTime.Now.Year - src.FechaNacimiento.Year
                    : DateTime.Now.Year - src.FechaNacimiento.Year - 1))
            // Mapear las colecciones utilizando los DTOs mínimos
            .ForMember(dest => dest.Medications, opt => opt.MapFrom(src => src.ResidentMedications))
            .ForMember(dest => dest.Pathologies, opt => opt.MapFrom(src => src.ResidentPathologies))
            .ForMember(dest => dest.Appointments, opt => opt.MapFrom(src => src.Appointments));

        CreateMap<ResidentMedication, ResidentMedicationMinimalDto>()
    .ForMember(dest => dest.Id_ResidentMedication, opt => opt.MapFrom(src => src.Id_ResidentMedication))
    .ForMember(dest => dest.Id_MedicamentSpecific, opt => opt.MapFrom(src => src.Id_MedicamentSpecific))
    .ForMember(dest => dest.Name_MedicamentSpecific, opt => opt.MapFrom(src => src.MedicationSpecific.Name_MedicamentSpecific))
    .ForMember(dest => dest.PrescribedDose, opt => opt.MapFrom(src => src.PrescribedDose))
    .ForMember(dest => dest.UnitOfMeasureName, opt => opt.MapFrom(src => src.MedicationSpecific.UnitOfMeasure.UnitName))
    .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes));

        CreateMap<ResidentPathology, ResidentPathologyMinimalDto>()
            .ForMember(dest => dest.Id_ResidentPathology, opt => opt.MapFrom(src => src.Id_ResidentPathology))
            .ForMember(dest => dest.Id_Pathology, opt => opt.MapFrom(src => src.Id_Pathology))
            .ForMember(dest => dest.Name_Pathology, opt => opt.MapFrom(src => src.Pathology.Name_Pathology))
            .ForMember(dest => dest.Resume_Pathology, opt => opt.MapFrom(src => src.Resume_Pathology))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes));

        // Mapear de Appointment a AppointmentMinimalDto
        CreateMap<Appointment, AppointmentMinimalDto>()
                .ForMember(dest => dest.Id_Appointment, opt => opt.MapFrom(src => src.Id_Appointment))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Time))
            .ForMember(dest => dest.AppointmentManager,
               opt => opt.MapFrom(src => src.Companion.First_Name + " " + src.Companion.Last_Name1))
             .ForMember(dest => dest.HealthcareCenterName,
               opt => opt.MapFrom(src => src.HealthcareCenter.Name_HC));



        CreateMap<MedicalHistory, MedicalHistoryGetDto>()
            .ForMember(dest => dest.ResidentFullName, opt => opt.MapFrom(src => $"{src.Resident.Name_RD} {src.Resident.Lastname1_RD} {src.Resident.Lastname2_RD}"));


        CreateMap<MedicalHistoryCreateDto, MedicalHistory>()
                   .ForMember(dest => dest.CreationDate, opt => opt.Ignore())
                   .ForMember(dest => dest.EditDate, opt => opt.Ignore());


        CreateMap<MedicalHistoryUpdateDto, MedicalHistory>()
                .ForMember(dest => dest.Id_MedicalHistory, opt => opt.Ignore())
                .ForMember(dest => dest.Id_Resident, opt => opt.Ignore())
                .ForMember(dest => dest.Resident, opt => opt.Ignore())
                .ForMember(dest => dest.CreationDate, opt => opt.Ignore())
                .ForMember(dest => dest.EditDate, opt => opt.MapFrom(src => DateTime.Now));


        CreateMap<AppointmentStatusUpdateDTO,AppointmentStatus>()
            .ForMember(dest => dest.Id_StatusAP, opt => opt.Ignore());


         CreateMap<DependencyLevelUpdateDTO,DependencyLevel>()
            .ForMember(dest => dest.Id_DependencyLevel, opt => opt.Ignore());

        CreateMap<HealthcareCenterUpdateDTO, HealthcareCenter>()
            .ForMember(dest => dest.Id_HC, opt => opt.Ignore());
        
     

       CreateMap<NoteUpdateDTO,Note>()
        .ForMember(dest => dest.Id_Note, opt => opt.Ignore());

        CreateMap<ProfessionUpdateDTO, Profession>()
            .ForMember(dest => dest.Id_Profession, opt => opt.Ignore());
 
        CreateMap<RoomUpdateDTO, Room>()
            .ForMember(dest => dest.Id_Room, opt => opt.Ignore());

        CreateMap<CategoryUpdateDTO, Category>()
            .ForMember(dest => dest.CategoryID, opt => opt.Ignore());


        CreateMap<ApplicationFormUpdateDto, ApplicationForm>()
            .ForMember(dest => dest.Id_ApplicationForm, opt => opt.Ignore())
            .ForMember(dest => dest.ApplicationDate, opt => opt.Ignore());

        CreateMap<SpecialtyUpdateDto, Specialty>()
    .ForMember(dest => dest.Id_Specialty, opt => opt.Ignore());

        CreateMap<TypeOfSalaryUpdateDto, TypeOfSalary>()
            .ForMember(dest => dest.Id_TypeOfSalary, opt => opt.Ignore());

        CreateMap<UnitOfMeasureUpdateDto, UnitOfMeasure>()
            .ForMember(dest => dest.UnitOfMeasureID, opt => opt.Ignore())
        .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.NombreUnidad));



        CreateMap<Employee, EmployeeFilterDTO>()
          .ForMember(dest => dest.Name_TypeOfSalary, opt => opt.MapFrom(src => src.TypeOfSalary.Name_TypeOfSalary))
          .ForMember(dest => dest.Name_Profession, opt => opt.MapFrom(src => src.Profession.Name_Profession));


        CreateMap<Product, ProductGetConvertDTO>()
                     .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
            .ForMember(dest => dest.UnitOfMeasure, opt => opt.MapFrom(src => src.UnitOfMeasure.UnitName));
    }
}
