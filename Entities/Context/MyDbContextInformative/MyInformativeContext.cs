using DataAccess.Entities.Informative;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Services.MyDbContext
{
    public class MyInformativeContext : DbContext
    {
        public MyInformativeContext(DbContextOptions<MyInformativeContext> options) : base(options)
        {
        }

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
        public DbSet<Applicant> Applicants { get; set; }
        public DbSet<Guardian> Guardians { get; set; }
        public DbSet<ApplicationForm> ApplicationForms { get; set; }
        public DbSet<FormDonation> FormDonations { get; set; }
        public DbSet<VoluntarieType> VoluntarieTypes { get; set; }
        public DbSet<FormVoluntarie> FormVoluntaries { get; set; }

        public DbSet<ApplicationStatus> ApplicationStatuses { get; set; }

        public DbSet<Status> Statuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<NavbarItem>()
                .HasMany(n => n.Children)
                .WithOne(n => n.Parent)
                .HasForeignKey(n => n.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de claves primarias auto-incrementales
            modelBuilder.Entity<NavbarItem>()
                .HasKey(n => n.Id_Nav_It);
            modelBuilder.Entity<NavbarItem>()
                .Property(n => n.Id_Nav_It)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<HeroSection>()
                .HasKey(h => h.Id_Hero);
            modelBuilder.Entity<HeroSection>()
                .Property(h => h.Id_Hero)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<AboutUsSection>()
                .HasKey(a => a.Id_About_Us);
            modelBuilder.Entity<AboutUsSection>()
                .Property(a => a.Id_About_Us)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<AssociatesSection>()
               .HasKey(a => a.Id_AssociatesSection);
            modelBuilder.Entity<AssociatesSection>()
                .Property(a => a.Id_AssociatesSection)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<SiteSettings>()
                .HasKey(s => s.Id_Site_Settings);
            modelBuilder.Entity<SiteSettings>()
                .Property(s => s.Id_Site_Settings)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<TitleSection>()
                .HasKey(t => t.Id_TitleSection);
            modelBuilder.Entity<TitleSection>()
                .Property(t => t.Id_TitleSection)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<ServiceSection>()
                .HasKey(s => s.Id_ServiceSection);
            modelBuilder.Entity<ServiceSection>()
                .Property(s => s.Id_ServiceSection)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<RegistrationSection>()
                .HasKey(r => r.Id_RegistrationSection);
            modelBuilder.Entity<RegistrationSection>()
                .Property(r => r.Id_RegistrationSection)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<TitleSection>()
           .Property(ts => ts.Description_Section)
           .IsRequired(false);

            // VolunteeringSection
            modelBuilder.Entity<VolunteeringSection>()
                .HasKey(vs => vs.Id_VolunteeringSection);
            modelBuilder.Entity<VolunteeringSection>()
                .Property(vs => vs.Id_VolunteeringSection)
                .ValueGeneratedOnAdd();

            // DonationsSection
            modelBuilder.Entity<DonationsSection>()
                .HasKey(ds => ds.Id_DonationsSection);
            modelBuilder.Entity<DonationsSection>()
                .Property(ds => ds.Id_DonationsSection)
                .ValueGeneratedOnAdd();

            // GalleryCategory
            modelBuilder.Entity<GalleryCategory>()
                .HasKey(gc => gc.Id_GalleryCategory);
            modelBuilder.Entity<GalleryCategory>()
                .Property(gc => gc.Id_GalleryCategory)
                .ValueGeneratedOnAdd();

            // GalleryItem
            modelBuilder.Entity<GalleryItem>()
                .HasKey(gi => gi.Id_GalleryItem);
            modelBuilder.Entity<GalleryItem>()
                .Property(gi => gi.Id_GalleryItem)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<GalleryItem>()
                .HasOne(gi => gi.GalleryCategory)
                .WithMany(gc => gc.GalleryItems)
                .HasForeignKey(gi => gi.Id_GalleryCategory)
                .OnDelete(DeleteBehavior.Cascade);

            // Contact
            modelBuilder.Entity<Contact>()
                .HasKey(c => c.Id_Contact);
            modelBuilder.Entity<Contact>()
                .Property(c => c.Id_Contact)
                .ValueGeneratedOnAdd();

            // ButtonInfo
            modelBuilder.Entity<ButtonInfo>()
                .HasKey(bi => bi.Id_ButtonInfo);
            modelBuilder.Entity<ButtonInfo>()
                .Property(bi => bi.Id_ButtonInfo)
                .ValueGeneratedOnAdd();


            modelBuilder.Entity<VolunteerProfile>()
              .HasKey(v => v.Id_Volunteer_Profile);
            modelBuilder.Entity<VolunteerProfile>()
                .Property(v => v.Id_Volunteer_Profile)
                .ValueGeneratedOnAdd();


            modelBuilder.Entity<AdministrativeRequirements>().HasKey(ar => ar.Id_AdministrativeRequirement);
            modelBuilder.Entity<AdministrativeRequirements>().Property(ar => ar.Id_AdministrativeRequirement).ValueGeneratedOnAdd();

            modelBuilder.Entity<NursingRequirements>().HasKey(nr => nr.Id_NursingRequirement);
            modelBuilder.Entity<NursingRequirements>().Property(nr => nr.Id_NursingRequirement).ValueGeneratedOnAdd();


            modelBuilder.Entity<ImportantInformation>().HasKey(ii => ii.Id_ImportantInformation);
            modelBuilder.Entity<ImportantInformation>().Property(ii => ii.Id_ImportantInformation).ValueGeneratedOnAdd();






            // Configuración para DonationType
            modelBuilder.Entity<DonationType>()
                .HasKey(d => d.Id_DonationType);  // Llave primaria
            modelBuilder.Entity<DonationType>()
                .Property(d => d.Id_DonationType)
                .ValueGeneratedOnAdd();  // Auto-incremental

            modelBuilder.Entity<FormDonation>()
            .HasOne(f => f.Status)  // Relación con Status
              .WithMany()  // Sin necesidad de navegación inversa
              .HasForeignKey(f => f.Id_Status)  // La clave foránea es Id_Status
              .OnDelete(DeleteBehavior.Restrict);  // Evitar eliminación en cascada

            // Configuración para MethodDonation
            modelBuilder.Entity<MethodDonation>()
                .HasKey(m => m.Id_MethodDonation);  // Llave primaria
            modelBuilder.Entity<MethodDonation>()
                .Property(m => m.Id_MethodDonation)
                .ValueGeneratedOnAdd();  // Auto-incremental

            // Relación entre MethodDonation y DonationType
            modelBuilder.Entity<MethodDonation>()
                .HasOne(m => m.DonationType)  // Relación con DonationType
                .WithMany(d => d.MethodDonations)  // Relación uno a muchos
                .HasForeignKey(m => m.DonationTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración para FormDonation
            modelBuilder.Entity<FormDonation>()
                .HasKey(f => f.Id_FormDonation);  // Llave primaria
            modelBuilder.Entity<FormDonation>()
                .Property(f => f.Id_FormDonation)
                .ValueGeneratedOnAdd();  // Auto-incremental

            // Relación entre FormDonation y DonationType
            modelBuilder.Entity<FormDonation>()
                .HasOne(f => f.DonationType)  // Relación con DonationType
                .WithMany(d => d.FormDonations)  // Relación uno a muchos
                .HasForeignKey(f => f.Id_DonationType)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación entre FormDonation y MethodDonation
            modelBuilder.Entity<FormDonation>()
                .HasOne(f => f.MethodDonation)  // Relación con MethodDonation
                .WithMany(m => m.FormDonations)  // Relación uno a muchos
                .HasForeignKey(f => f.Id_MethodDonation)
                .OnDelete(DeleteBehavior.Cascade);

            // -- Configuración adicional para Voluntariados --

            // Relación entre FormVoluntarie y VoluntarieType
            modelBuilder.Entity<FormVoluntarie>()
                .HasOne(f => f.VoluntarieType)
                .WithMany(v => v.FormVoluntaries)  // Relación inversa
                .HasForeignKey(f => f.Id_VoluntarieType)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación entre FormVoluntarie y Status (nuevo)
            modelBuilder.Entity<FormVoluntarie>()
                .HasOne(f => f.Status)
                .WithMany()  // Sin relación inversa necesaria
                .HasForeignKey(f => f.Id_Status)
                .OnDelete(DeleteBehavior.Restrict);  // No eliminar estado al eliminar el FormVoluntarie


            // Configuración para VoluntarieType
            modelBuilder.Entity<VoluntarieType>()
                .HasKey(v => v.Id_VoluntarieType);
            modelBuilder.Entity<VoluntarieType>()
                .Property(v => v.Id_VoluntarieType)
                .ValueGeneratedOnAdd();

            // Configuración para FormVoluntarie
            modelBuilder.Entity<FormVoluntarie>()
                .HasKey(f => f.Id_FormVoluntarie);
            modelBuilder.Entity<FormVoluntarie>()
                .Property(f => f.Id_FormVoluntarie)
                .ValueGeneratedOnAdd();

            // Relación entre FormVoluntarie y VoluntarieType
            modelBuilder.Entity<FormVoluntarie>()
                .HasOne(f => f.VoluntarieType)
                .WithMany(v => v.FormVoluntaries)  // Relación inversa
                .HasForeignKey(f => f.Id_VoluntarieType)
                .OnDelete(DeleteBehavior.Cascade);  // Borrar en cascada si se elimina VoluntarieType

            // Relación entre FormVoluntarie y Status (nuevo)
            modelBuilder.Entity<FormVoluntarie>()
        .HasOne(f => f.Status)
        .WithMany()  // No necesitas navegación inversa desde Status
        .HasForeignKey(f => f.Id_Status)  // Usar el nombre correcto de la columna en la base de datos
        .OnDelete(DeleteBehavior.Restrict);

            // Configuración para Status
            modelBuilder.Entity<Status>()
                .HasKey(s => s.Id_Status);
            modelBuilder.Entity<Status>()
                .Property(s => s.Id_Status)
                .ValueGeneratedOnAdd();



            // Configuración para Applicant
            modelBuilder.Entity<Applicant>()
                .HasKey(a => a.Id_Applicant);  // Llave primaria
            modelBuilder.Entity<Applicant>()
                .Property(a => a.Id_Applicant)
                .ValueGeneratedOnAdd();  // Auto incremento

            // Configuración para Guardian
            modelBuilder.Entity<Guardian>()
                .HasKey(g => g.Id_Guardian);  // Llave primaria
            modelBuilder.Entity<Guardian>()
                .Property(g => g.Id_Guardian)
                .ValueGeneratedOnAdd();  // Auto incremento

            // Configuración para ApplicationStatus
            modelBuilder.Entity<ApplicationStatus>()
                .HasKey(s => s.Id_Status);  // Llave primaria
            modelBuilder.Entity<ApplicationStatus>()
                .Property(s => s.Id_Status)
                .ValueGeneratedOnAdd();  // Auto incremento
            modelBuilder.Entity<ApplicationStatus>()
                .Property(s => s.Status_Name)
                .IsRequired()  // Campo requerido
                .HasMaxLength(50);  // Longitud máxima del estado

            // Configuración para ApplicationForm
            modelBuilder.Entity<ApplicationForm>()
                .HasKey(af => af.Id_ApplicationForm);  // Llave primaria
            modelBuilder.Entity<ApplicationForm>()
                .Property(af => af.Id_ApplicationForm)
                .ValueGeneratedOnAdd();  // Auto incremento

            // Relación entre ApplicationForm y Applicant
            modelBuilder.Entity<ApplicationForm>()
                .HasOne(af => af.Applicant)  // Relación con Applicant
                .WithMany()  // Un Applicant puede tener varias solicitudes
                .HasForeignKey(af => af.Id_Applicant)
                .OnDelete(DeleteBehavior.Cascade);  // Borrar en cascada si el Applicant es eliminado

            // Relación entre ApplicationForm y Guardian
            modelBuilder.Entity<ApplicationForm>()
                .HasOne(af => af.Guardian)  // Relación con Guardian
                .WithMany()  // Un Guardian puede tener varias solicitudes
                .HasForeignKey(af => af.Id_Guardian)
                .OnDelete(DeleteBehavior.Cascade);  // Borrar en cascada si el Guardian es eliminado

            // Relación entre ApplicationForm y ApplicationStatus
            modelBuilder.Entity<ApplicationForm>()
                .HasOne(af => af.ApplicationStatus)  // Relación con ApplicationStatus
                .WithMany()  // Un estado puede estar asignado a muchas solicitudes
                .HasForeignKey(af => af.Id_Status)
                .OnDelete(DeleteBehavior.Restrict);  // No eliminar el estado si está asignado a solicitudes
        }
    }
}