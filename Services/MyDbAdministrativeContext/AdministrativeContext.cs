using Entities.Administration;
using Entities.Informative;
using Microsoft.EntityFrameworkCore;

namespace Services.MyDbContext
{
    public class AdministrativeContext : DbContext
    {
        public AdministrativeContext(DbContextOptions<AdministrativeContext> options) : base(options)
        {
        }

        // DbSet para las tablas independientes
        public DbSet<TypeOfSalary> TypeOfSalaries { get; set; }
        public DbSet<Profession> Professions { get; set; }
        public DbSet<Rol> Roles { get; set; }

        // DbSet para FormVoluntarie
        public DbSet<FormVoluntarie> FormVoluntaries { get; set; }
        public DbSet<VoluntarieType> VoluntarieTypes { get; set; }

        // DbSet para las tablas que dependen de otras
        public DbSet<Employee> Employees { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<EmployeeRole> EmployeeRoles { get; set; }

        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración para TypeOfSalary
            modelBuilder.Entity<TypeOfSalary>()
                .HasKey(t => t.Id_TypeOfSalary);

            modelBuilder.Entity<TypeOfSalary>()
                .Property(t => t.Id_TypeOfSalary)
                .ValueGeneratedOnAdd();

            // Configuración para Profession
            modelBuilder.Entity<Profession>()
                .HasKey(p => p.Id_Profession);

            modelBuilder.Entity<Profession>()
                .Property(p => p.Id_Profession)
                .ValueGeneratedOnAdd();

            // Configuración para Rol
            modelBuilder.Entity<Rol>()
                .HasKey(r => r.Id_Role);

            modelBuilder.Entity<Rol>()
                .Property(r => r.Id_Role)
                .ValueGeneratedOnAdd();

            // Configuración para Employee
            modelBuilder.Entity<Employee>()
                .HasKey(e => e.Dni);

            modelBuilder.Entity<Employee>()
                .Property(e => e.Dni)
                .ValueGeneratedNever();  // No auto-incremento, ya que es una identificación externa

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.TypeOfSalary)
                .WithMany()
                .HasForeignKey(e => e.Id_TypeOfSalary)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Profession)
                .WithMany()
                .HasForeignKey(e => e.Id_Profession)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración para User (1:1 con Employee)
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id_User);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Employee)
                .WithOne()
                .HasForeignKey<User>(u => u.Dni_Employee)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración para EmployeeRole (relación M:N entre Employee y Rol)
            modelBuilder.Entity<EmployeeRole>()
                .HasKey(er => new { er.Dni_Employee, er.Id_Role });

            modelBuilder.Entity<EmployeeRole>()
                .HasOne(er => er.Employee)
                .WithMany(e => e.EmployeeRoles)
                .HasForeignKey(er => er.Dni_Employee)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmployeeRole>()
                .HasOne(er => er.Rol)
                .WithMany(r => r.EmployeeRoles)
                .HasForeignKey(er => er.Id_Role)
                .OnDelete(DeleteBehavior.Cascade);

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





            // Configuración adicional (si la necesitas)
            modelBuilder.Entity<PasswordResetToken>().HasKey(p => p.Id);
        }
    }
}
