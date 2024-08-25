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


        }
    }
}