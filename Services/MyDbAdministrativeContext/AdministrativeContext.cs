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

        // DbSet para tablas independientes
        public DbSet<TypeOfSalary> TypeOfSalaries { get; set; }
        public DbSet<Profession> Professions { get; set; }
        public DbSet<Rol> Roles { get; set; }

        // DbSet para Application Form
        public DbSet<ApplicationForm> ApplicationForms { get; set; }
        public DbSet<ApplicationStatus> ApplicationStatuses { get; set; }
        public DbSet<Applicant> Applicants { get; set; }
        public DbSet<Guardian> Guardians { get; set; }

        // DbSet para FormVoluntarie y VoluntarieType
        public DbSet<FormVoluntarie> FormVoluntaries { get; set; }
        public DbSet<VoluntarieType> VoluntarieTypes { get; set; }

        // DbSet para tablas relacionadas
        public DbSet<Employee> Employees { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<EmployeeRole> EmployeeRoles { get; set; }

        // DbSet para PasswordResetToken
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        // DbSet para PaymentReceipt y Deduction
        public DbSet<PaymentReceipt> PaymentReceipts { get; set; }
        public DbSet<Deduction> Deductions { get; set; }

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

            // Configuración para ApplicationStatus
            modelBuilder.Entity<ApplicationStatus>()
                .HasKey(s => s.Id_Status);
            modelBuilder.Entity<ApplicationStatus>()
                .Property(s => s.Id_Status)
                .ValueGeneratedOnAdd();

            // Configuración para Applicant
            modelBuilder.Entity<Applicant>()
                .HasKey(a => a.Id_Applicant);
            modelBuilder.Entity<Applicant>()
                .Property(a => a.Id_Applicant)
                .ValueGeneratedOnAdd();

            // Configuración para Guardian
            modelBuilder.Entity<Guardian>()
                .HasKey(g => g.Id_Guardian);
            modelBuilder.Entity<Guardian>()
                .Property(g => g.Id_Guardian)
                .ValueGeneratedOnAdd();

            // Configuración para ApplicationForm
            modelBuilder.Entity<ApplicationForm>()
                .HasKey(af => af.Id_ApplicationForm);
            modelBuilder.Entity<ApplicationForm>()
                .Property(af => af.Id_ApplicationForm)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<ApplicationForm>()
                .HasOne(af => af.Applicant)
                .WithMany()
                .HasForeignKey(af => af.Id_Applicant)
                .OnDelete(DeleteBehavior.Cascade);  // Borrar en cascada si el Applicant es eliminado
            modelBuilder.Entity<ApplicationForm>()
                .HasOne(af => af.Guardian)
                .WithMany()
                .HasForeignKey(af => af.Id_Guardian)
                .OnDelete(DeleteBehavior.Cascade);  // Borrar en cascada si el Guardian es eliminado
            modelBuilder.Entity<ApplicationForm>()
                .HasOne(af => af.ApplicationStatus)
                .WithMany()
                .HasForeignKey(af => af.Id_Status)
                .OnDelete(DeleteBehavior.Restrict);  // No eliminar el estado si está asignado a solicitudes


            // Configuración para Status
            modelBuilder.Entity<Status>()
                .HasKey(s => s.Id_Status);  // Clave primaria
            modelBuilder.Entity<Status>()
                .Property(s => s.Id_Status)
                .ValueGeneratedOnAdd();  // Auto incremento

            modelBuilder.Entity<PaymentReceipt>()
          .HasKey(pr => pr.Id);
            modelBuilder.Entity<PaymentReceipt>()
                .Property(pr => pr.Salary)
                .HasColumnType("decimal(10, 2)")
                .IsRequired();
            modelBuilder.Entity<PaymentReceipt>()
                .Property(pr => pr.Overtime)
                .HasColumnType("decimal(10, 2)")
                .IsRequired()
                .HasDefaultValue(0);
            modelBuilder.Entity<PaymentReceipt>()
                .Property(pr => pr.TotalDeductions)
                .HasColumnType("decimal(10, 2)")
                .IsRequired();
            modelBuilder.Entity<PaymentReceipt>()
                .Property(pr => pr.NetAmount)
                .HasColumnType("decimal(10, 2)")
                .IsRequired();
            modelBuilder.Entity<PaymentReceipt>()
                .Property(pr => pr.GrossAmount)
                .HasColumnType("decimal(10, 2)")
                .IsRequired();
            modelBuilder.Entity<PaymentReceipt>()
                .Property(pr => pr.Notes)
                .HasMaxLength(500);
            modelBuilder.Entity<PaymentReceipt>()
                .Property(pr => pr.PdfFilePath)
                .HasMaxLength(255);

            // Relación con Employee
            modelBuilder.Entity<PaymentReceipt>()
                .HasOne(pr => pr.Employee)
                .WithMany(e => e.PaymentReceipts)
                .HasForeignKey(pr => pr.EmployeeDni)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices
            modelBuilder.Entity<PaymentReceipt>()
                .HasIndex(pr => new { pr.EmployeeDni, pr.PaymentDate })
                .HasDatabaseName("IX_PaymentReceipt_EmployeeDni_PaymentDate");

            modelBuilder.Entity<Deduction>()
                .HasKey(d => d.Id);
            modelBuilder.Entity<Deduction>()
                .Property(d => d.Type)
                .HasMaxLength(100)
                .IsRequired();
            modelBuilder.Entity<Deduction>()
                .Property(d => d.Amount)
                .HasColumnType("decimal(10, 2)")
                .IsRequired();

            // Relación con PaymentReceipt
            modelBuilder.Entity<Deduction>()
                .HasOne(d => d.PaymentReceipt)
                .WithMany(pr => pr.DeductionsList)
                .HasForeignKey(d => d.PaymentReceiptId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices
            modelBuilder.Entity<Deduction>()
                .HasIndex(d => d.PaymentReceiptId)
                .HasDatabaseName("IX_Deduction_PaymentReceiptId");


            // Relación entre FormVoluntarie y VoluntarieType
            modelBuilder.Entity<FormVoluntarie>()
                .HasOne(f => f.VoluntarieType)
                .WithMany(v => v.FormVoluntaries)
                .HasForeignKey(f => f.Id_VoluntarieType)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación entre FormVoluntarie y Status
            modelBuilder.Entity<FormVoluntarie>()
                .HasOne(f => f.Status)
                .WithMany()
                .HasForeignKey(f => f.Id_Status)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración adicional para VoluntarieType
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

            // Configuración adicional para PasswordResetToken
            modelBuilder.Entity<PasswordResetToken>()
                .HasKey(p => p.Id);
        }
    }
}
