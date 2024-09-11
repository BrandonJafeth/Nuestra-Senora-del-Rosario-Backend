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
        }
    }
}
