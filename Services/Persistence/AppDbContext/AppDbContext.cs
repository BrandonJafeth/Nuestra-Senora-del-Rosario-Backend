using Domain.Entities.Administration;
using Domain.Entities.Informative;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.AppDbContext
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        #region Administrative_DbSets
        public DbSet<TypeOfSalary> TypeOfSalaries { get; set; }
        public DbSet<Profession> Professions { get; set; }
        public DbSet<Rol> Roles { get; set; }


        public DbSet<ApplicationForm> ApplicationForms { get; set; }
        public DbSet<ApplicationStatus> ApplicationStatuses { get; set; }
        public DbSet<Guardian> Guardians { get; set; }

 
        public DbSet<FormVoluntarie> FormVoluntaries { get; set; }
        public DbSet<VoluntarieType> VoluntarieTypes { get; set; }

 
        public DbSet<Employee> Employees { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<EmployeeRole> EmployeeRoles { get; set; }

        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        public DbSet<PaymentReceipt> PaymentReceipts { get; set; }
        public DbSet<Deduction> Deductions { get; set; }


        public DbSet<Room> Rooms { get; set; }
        public DbSet<Resident> Residents { get; set; }
        public DbSet<DependencyLevel> DependencyLevels { get; set; }
        public DbSet<DependencyHistory> DependencyHistories { get; set; }

        public DbSet<HealthcareCenter> HealthcareCenters { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<AppointmentStatus> AppointmentStatuses { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Note> Notes { get; set; }

        public DbSet<UnitOfMeasure> UnitOfMeasures { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        #endregion

        #region Informative_DbSets
        public DbSet<NavbarItem> NavbarItems { get; set; }
        public DbSet<HeroSection> HeroSections { get; set; }
        public DbSet<AboutUsSection> AboutUsSections { get; set; }
        public DbSet<SiteSettings> SiteSettings { get; set; }
        public DbSet<TitleSection> TitleSections { get; set; }
        public DbSet<ServiceSection> ServiceSections { get; set; }
        public DbSet<RegistrationSection> RegistrationSections { get; set; }
        public DbSet<VolunteeringSection> VolunteeringSections { get; set; }
        public DbSet<DonationsSection> DonationsSections { get; set; }
        public DbSet<GalleryCategory> GalleryCategories { get; set; }
        public DbSet<GalleryItem> GalleryItems { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ButtonInfo> ButtonInfos { get; set; }
        public DbSet<VolunteerProfile> VolunteerProfiles { get; set; }
        public DbSet<AssociatesSection> AssociatesSections { get; set; }
        public DbSet<AdministrativeRequirements> AdministrativeRequirements { get; set; }
        public DbSet<NursingRequirements> NursingRequirements { get; set; }
        public DbSet<ImportantInformation> ImportantInformation { get; set; }

        public DbSet<DonationType> DonationTypes { get; set; }

        public DbSet<MethodDonation> MethodDonations { get; set; }

        public DbSet<Status> Statuses { get; set; }

        public DbSet<FormDonation> FormDonations { get; set; }
        #endregion
    }
}
