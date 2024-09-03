using Entities.Informative;
using Microsoft.EntityFrameworkCore;
using Services.Informative;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Services.MyDbContext
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options)
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

        }
    }
}