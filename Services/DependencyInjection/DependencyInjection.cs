using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// Referencias a tus namespaces de Infrastructure
using Infrastructure.Persistence.MyDbAdministrativeContext;
using Infrastructure.Persistence.MyDbContextInformative;

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

namespace Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            #region DbContexts
            // 1. Registrar DbContexts
            services.AddDbContext<MyInformativeContext>(options =>
                options.UseMySql(configuration.GetConnectionString("DefaultConnection"),
                    ServerVersion.AutoDetect(configuration.GetConnectionString("DefaultConnection"))));

            services.AddDbContext<AdministrativeContext>(options =>
                options.UseMySql(configuration.GetConnectionString("DefaultConnection"),
                    ServerVersion.AutoDetect(configuration.GetConnectionString("DefaultConnection"))));
            #endregion

            #region AutoMapper
            services.AddAutoMapper(typeof(AdministrativeMappingProfile).Assembly);

            #endregion

            #region Repos_Generic_MyInformativeContext
            services.AddScoped<ISvGenericRepository<FormVoluntarie>, SvGenericRepository<FormVoluntarie, MyInformativeContext>>();
            services.AddScoped<ISvGenericRepository<NavbarItem>, SvGenericRepository<NavbarItem, MyInformativeContext>>();
            services.AddScoped<ISvGenericRepository<GalleryItem>, SvGenericRepository<GalleryItem, MyInformativeContext>>();
            services.AddScoped<ISvGenericRepository<MethodDonation>, SvGenericRepository<MethodDonation, MyInformativeContext>>();
            services.AddScoped<ISvGenericRepository<DonationType>, SvGenericRepository<DonationType, MyInformativeContext>>();
            services.AddScoped<ISvGenericRepository<FormDonation>, SvGenericRepository<FormDonation, MyInformativeContext>>();
            services.AddScoped<ISvGenericRepository<ApplicationForm>, SvGenericRepository<ApplicationForm, MyInformativeContext>>();
            services.AddScoped<ISvGenericRepository<Status>, SvGenericRepository<Status, MyInformativeContext>>();
            #endregion

            #region Repos_Generic_AdministrativeContext
            services.AddScoped<ISvGenericRepository<User>, SvGenericRepository<User, AdministrativeContext>>();
            services.AddScoped<ISvGenericRepository<Employee>, SvGenericRepository<Employee, AdministrativeContext>>();
            services.AddScoped<ISvGenericRepository<EmployeeRole>, SvGenericRepository<EmployeeRole, AdministrativeContext>>();
            services.AddScoped<ISvGenericRepository<TypeOfSalary>, SvGenericRepository<TypeOfSalary, AdministrativeContext>>();
            services.AddScoped<ISvGenericRepository<Profession>, SvGenericRepository<Profession, AdministrativeContext>>();
            services.AddScoped<ISvGenericRepository<PasswordResetToken>, SvGenericRepository<PasswordResetToken, AdministrativeContext>>();
            services.AddScoped<ISvGenericRepository<Rol>, SvGenericRepository<Rol, AdministrativeContext>>();
            services.AddScoped<ISvGenericRepository<Room>, SvGenericRepository<Room, AdministrativeContext>>();
            services.AddScoped<ISvGenericRepository<DependencyLevel>, SvGenericRepository<DependencyLevel, AdministrativeContext>>();
            services.AddScoped<ISvGenericRepository<DependencyHistory>, SvGenericRepository<DependencyHistory, AdministrativeContext>>();
            services.AddScoped<ISvGenericRepository<ResidentApplication>, SvGenericRepository<ResidentApplication, AdministrativeContext>>();
            services.AddScoped<ISvGenericRepository<Resident>, SvGenericRepository<Resident, AdministrativeContext>>();
            services.AddScoped<ISvGenericRepository<Guardian>, SvGenericRepository<Guardian, AdministrativeContext>>();
            services.AddScoped<ISvGenericRepository<Applicant>, SvGenericRepository<Applicant, AdministrativeContext>>();
            services.AddScoped<ISvGenericRepository<Notification>, SvGenericRepository<Notification, AdministrativeContext>>();
            services.AddScoped<ISvGenericRepository<UnitOfMeasure>, SvGenericRepository<UnitOfMeasure, AdministrativeContext>>();
            services.AddScoped<ISvGenericRepository<Category>, SvGenericRepository<Category, AdministrativeContext>>();
            services.AddScoped<ISvGenericRepository<Product>, SvGenericRepository<Product, AdministrativeContext>>();
            services.AddScoped<ISvGenericRepository<Inventory>, SvGenericRepository<Inventory, AdministrativeContext>>();
            services.AddScoped<ISvGenericRepository<Appointment>, SvGenericRepository<Appointment, AdministrativeContext>>();
            services.AddScoped<ISvGenericRepository<Specialty>, SvGenericRepository<Specialty, AdministrativeContext>>();
            services.AddScoped<ISvGenericRepository<HealthcareCenter>, SvGenericRepository<HealthcareCenter, AdministrativeContext>>();
            services.AddScoped<ISvGenericRepository<AppointmentStatus>, SvGenericRepository<AppointmentStatus, AdministrativeContext>>();
            services.AddScoped<ISvGenericRepository<Note>, SvGenericRepository<Note, AdministrativeContext>>();
            #endregion

            #region Services_Administrative
            services.AddScoped<ISvEmployee, SvEmployee>();
            services.AddScoped<ISvEmployeeRole, SvEmployeeRole>();
            services.AddScoped<ISvPasswordResetService, SvPasswordResetService>();
            services.AddScoped<ISvPdfReceiverService, SvPdfReceiverService>();
            services.AddScoped<ISvNotification, SvNotification>(); // Registro del servicio de notificaciones
            services.AddScoped<ISvProductService, SvProductService>();
            services.AddScoped<ISvInventoryService, SvInventoryService>();
            services.AddScoped<ISvAppointment, SvAppointment>();
            services.AddScoped<ISvGuardian, SvGuardian>();
            services.AddScoped<ISvPaymentReceipt, SvPaymentReceipt>();
            services.AddScoped<ISvResident, SvResident>();
            services.AddScoped<ISvUser, SvUser>();
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
