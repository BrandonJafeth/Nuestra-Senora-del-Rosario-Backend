using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;


using Infrastructure.Services.Administrative.Appointments;
using Infrastructure.Services.Administrative.EmailServices;
using Infrastructure.Services.Administrative.EmployeeRoleService;
using Infrastructure.Services.Administrative.FormVoluntarieService;
using Infrastructure.Services.Administrative.Guardians;
using Infrastructure.Services.Administrative.Inventory;
using Infrastructure.Services.Administrative.Notifications;
using Infrastructure.Services.Administrative.PasswordResetServices;
using Infrastructure.Services.Administrative.PaymentReceiptService;
using Infrastructure.Services.Administrative.PdfReceiver;
using Infrastructure.Services.Administrative.Product;
using Infrastructure.Services.Administrative.Residents;
using Infrastructure.Services.Administrative.Users;
using Infrastructure.Services.Administrative.Notifications.Hubs;
using Infrastructure.Services.Informative.ApplicationFormService;
using Infrastructure.Services.Informative.DonationType;
using Infrastructure.Services.Informative.FormDonationService;
using Infrastructure.Services.Informative.FormVoluntarieService;
using Infrastructure.Services.Informative.GalleryItemService;
using Infrastructure.Services.Informative.MethodDonationService;
using Infrastructure.Services.Informative.NavbarItemServices;
using Services.GenericService;   // Para ISvGenericRepository, etc.

// Entidades del Dominio, si necesitas
using Infrastructure.Services.Administrative.AdministrativeDTO.EmployeeService;
using Domain.Entities.Administration;
using Domain.Entities.Informative;
using Infrastructure.Persistence.AppDbContext;
using Infrastructure.Services.Informative.MappingProfiles;
using Infrastructure.Services.Administrative.MedicationSpecifics;
using Infrastructure.Services.Administrative.ResidentMedications;
using Infrastructure.Services.Administrative.ResidentPathologies;
using Infrastructure.Services.Administrative.MedicalHistories;
using Infrastructure.Services.Administrative.GogleDrive;
using Infrastructure.Services.Administrative.ConversionService;
using Infrastructure.Services.Administrative.Assets;

namespace Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            #region DbContext
            services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(
            configuration.GetConnectionString("DefaultConnection"),
            ServerVersion.AutoDetect(configuration.GetConnectionString("DefaultConnection"))
                )
            );
            #endregion

            #region AutoMapper
            services.AddAutoMapper(typeof(AdministrativeMappingProfile),
                          typeof(FormDonationMappingProfile),
                          typeof(FormVoluntarieMappingProfile));


            #endregion

            #region Repos_Generic_MyInformativeContext
            services.AddScoped<ISvGenericRepository<FormVoluntarie>, SvGenericRepository<FormVoluntarie>>();
            services.AddScoped<ISvGenericRepository<NavbarItem>, SvGenericRepository<NavbarItem>>();
            services.AddScoped<ISvGenericRepository<GalleryItem>, SvGenericRepository<GalleryItem>>();
            services.AddScoped<ISvGenericRepository<MethodDonation>, SvGenericRepository<MethodDonation>>();
            services.AddScoped<ISvGenericRepository<DonationType>, SvGenericRepository<DonationType>>();
            services.AddScoped<ISvGenericRepository<FormDonation>, SvGenericRepository<FormDonation>>();
            services.AddScoped<ISvGenericRepository<ApplicationForm>, SvGenericRepository<ApplicationForm>>();
            services.AddScoped<ISvGenericRepository<Status>, SvGenericRepository<Status>>();
            services.AddScoped<ISvGenericRepository<AdministrativeRequirements>, SvGenericRepository<AdministrativeRequirements>>();
            services.AddScoped<ISvGenericRepository<AssociatesSection>, SvGenericRepository<AssociatesSection>>();
            services.AddScoped<ISvGenericRepository<ButtonInfo>, SvGenericRepository<ButtonInfo>>();
            services.AddScoped<ISvGenericRepository<Contact>, SvGenericRepository<Contact>>();
            services.AddScoped<ISvGenericRepository<DonationsSection>, SvGenericRepository<DonationsSection>>();
            services.AddScoped<ISvGenericRepository<GalleryCategory>, SvGenericRepository<GalleryCategory>>();
            services.AddScoped<ISvGenericRepository<HeroSection>, SvGenericRepository<HeroSection>>();
            services.AddScoped<ISvGenericRepository<ImportantInformation>, SvGenericRepository<ImportantInformation>>();
            services.AddScoped<ISvGenericRepository<NursingRequirements>, SvGenericRepository<NursingRequirements>>();
            services.AddScoped<ISvGenericRepository<RegistrationSection>, SvGenericRepository<RegistrationSection>>();
            services.AddScoped<ISvGenericRepository<ServiceSection>, SvGenericRepository<ServiceSection>>();
            services.AddScoped<ISvGenericRepository<SiteSettings>, SvGenericRepository<SiteSettings>>();
            services.AddScoped<ISvGenericRepository<TitleSection>, SvGenericRepository<TitleSection>>();
            services.AddScoped<ISvGenericRepository<VoluntarieType>, SvGenericRepository<VoluntarieType>>();
            services.AddScoped<ISvGenericRepository<VolunteeringSection>, SvGenericRepository<VolunteeringSection>>();
            services.AddScoped<ISvGenericRepository<VolunteerProfile>, SvGenericRepository<VolunteerProfile>>();
            services.AddScoped<ISvGenericRepository<AboutUsSection>, SvGenericRepository<AboutUsSection>>();
            services.AddScoped<ISvGenericRepository<ApplicationStatus>, SvGenericRepository<ApplicationStatus>>();

            #endregion

            #region Repos_Generic_AdministrativeContext
            services.AddScoped<ISvGenericRepository<User>, SvGenericRepository<User>>();
            services.AddScoped<ISvGenericRepository<Employee>, SvGenericRepository<Employee>>();
            services.AddScoped<ISvGenericRepository<UserRoles>, SvGenericRepository<UserRoles>>();
            services.AddScoped<ISvGenericRepository<TypeOfSalary>, SvGenericRepository<TypeOfSalary>>();
            services.AddScoped<ISvGenericRepository<Profession>, SvGenericRepository<Profession>>();
            services.AddScoped<ISvGenericRepository<PasswordResetToken>, SvGenericRepository<PasswordResetToken>>();
            services.AddScoped<ISvGenericRepository<Rol>, SvGenericRepository<Rol>>();
            services.AddScoped<ISvGenericRepository<Room>, SvGenericRepository<Room>>();
            services.AddScoped<ISvGenericRepository<DependencyLevel>, SvGenericRepository<DependencyLevel>>();
            services.AddScoped<ISvGenericRepository<DependencyHistory>, SvGenericRepository<DependencyHistory>>();
            services.AddScoped<ISvGenericRepository<Resident>, SvGenericRepository<Resident>>();
            services.AddScoped<ISvGenericRepository<Guardian>, SvGenericRepository<Guardian>>();
            services.AddScoped<ISvGenericRepository<Notification>, SvGenericRepository<Notification>>();
            services.AddScoped<ISvGenericRepository<UnitOfMeasure>, SvGenericRepository<UnitOfMeasure>>();
            services.AddScoped<ISvGenericRepository<Category>, SvGenericRepository<Category>>();
            services.AddScoped<ISvGenericRepository<Product>, SvGenericRepository<Product>>();
            services.AddScoped<ISvGenericRepository<Inventory>, SvGenericRepository<Inventory>>();
            services.AddScoped<ISvGenericRepository<Appointment>, SvGenericRepository<Appointment>>();
            services.AddScoped<ISvGenericRepository<Specialty>, SvGenericRepository<Specialty>>();
            services.AddScoped<ISvGenericRepository<HealthcareCenter>, SvGenericRepository<HealthcareCenter>>();
            services.AddScoped<ISvGenericRepository<AppointmentStatus>, SvGenericRepository<AppointmentStatus>>();
            services.AddScoped<ISvGenericRepository<Note>, SvGenericRepository<Note>>();
            services.AddScoped<ISvGenericRepository<MedicationSpecific>, SvGenericRepository<MedicationSpecific>>(); 
            services.AddScoped<ISvGenericRepository<AdministrationRoute>, SvGenericRepository<AdministrationRoute>>();
            services.AddScoped<ISvGenericRepository<Pathology>, SvGenericRepository<Pathology>>();
            services.AddScoped<ISvGenericRepository<ResidentPathology>, SvGenericRepository<ResidentPathology>>();
            services.AddScoped<ISvGenericRepository<ResidentMedication>, SvGenericRepository<ResidentMedication>>();
            services.AddScoped<ISvGenericRepository<MedicalHistory>, SvGenericRepository<MedicalHistory>>();
            services.AddScoped<ISvGenericRepository<Brand>, SvGenericRepository<Brand>>();
            services.AddScoped<ISvGenericRepository<Model>, SvGenericRepository<Model>>();
            services.AddScoped<ISvGenericRepository<Asset>, SvGenericRepository<Asset>>();
            services.AddScoped<ISvGenericRepository<AssetCategory>, SvGenericRepository<AssetCategory>>();
            services.AddScoped<ISvGenericRepository<Law>, SvGenericRepository<Law>>();
            #endregion

            #region Services_Administrative
            services.AddScoped<ISvEmployee, SvEmployee>();
            services.AddScoped<ISvUserRole, SvUserRole>();
            services.AddScoped<ISvPasswordResetService, SvPasswordResetService>();
            services.AddScoped<ISvPdfReceiverService, SvPdfReceiverService>();
            services.AddScoped<ISvNotification, SvNotification>(); 
            services.AddScoped<ISvProductService, SvProductService>();
            services.AddScoped<ISvInventoryService, SvInventoryService>();
            services.AddScoped<ISvAppointment, SvAppointment>();
            services.AddScoped<ISvGuardian, SvGuardian>();
            services.AddScoped<ISvPaymentReceipt, SvPaymentReceipt>();
            services.AddScoped<ISvResident, SvResident>();
            services.AddScoped<ISvUser, SvUser>();
            services.AddScoped<ISvMedicationSpecific, SvMedicationSpecific>();
            services.AddScoped<ISvResidentMedication, SvResidentMedication>();
            services.AddScoped<ISvResidentPathology, SvResidentPathology>();
            services.AddScoped<ISvMedicalHistory, SvMedicalHistory>();
            services.AddScoped<ISvGoogleDrive, SvGoogleDrive>();
            services.AddScoped<ISvConversionService, SvConversionService>();
            services.AddScoped<ISvAssetService, SvAssetService>();
            #endregion

            #region Services_Informative
            services.AddScoped<ISvNavbarItemService, SvNavbarItem>();
            services.AddScoped<ISvGalleryItem, SvGalleryItem>();
            services.AddScoped<ISvMethodDonation, SvMethodDonation>();
            services.AddScoped<ISvDonationType, SvDonationType>();
            services.AddScoped<ISvFormDonation, SvFormDonationService>();
            services.AddScoped<ISvApplicationForm, SvApplicationForm>();
            services.AddScoped<ISvFormVoluntarieService, SvFormVoluntarieService>();
            services.AddScoped<IAdministrativeFormVoluntarieService, AdministrativeFormVoluntarieService>();
            services.AddScoped<ISvEmailService, SvEmailService>();
            #endregion

            #region Return
            return services;
            #endregion
        }
    }
}
