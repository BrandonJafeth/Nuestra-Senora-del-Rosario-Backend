using Entities.Administration;
using Entities.Informative;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.MyDbAdministrativeContext
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


        public DbSet<Room> Rooms { get; set; }
        public DbSet<Resident> Residents { get; set; }
        public DbSet<DependencyLevel> DependencyLevels { get; set; }
        public DbSet<DependencyHistory> DependencyHistories { get; set; }
        public DbSet<ResidentApplication> Residents_Applications { get; set; }

        public DbSet<HealthcareCenter> HealthcareCenters { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<AppointmentStatus> AppointmentStatuses { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Note> Notes { get; set; }

        // Inventory Management DbSets
        public DbSet<UnitOfMeasure> UnitOfMeasures { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Inventory> Inventories { get; set; }

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

            // Configuración de propiedades para PaymentReceipt
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
                .Property(pr => pr.GrossIncome)  // Cambié de GrossAmount a GrossIncome si es el término que usas
                .HasColumnType("decimal(10, 2)")
                .IsRequired();

            modelBuilder.Entity<PaymentReceipt>()
                .Property(pr => pr.Notes)
                .HasMaxLength(500);


            // Relación con Employee (uno a muchos)
            modelBuilder.Entity<PaymentReceipt>()
                .HasOne(pr => pr.Employee)
                .WithMany(e => e.PaymentReceipts)
                .HasForeignKey(pr => pr.EmployeeDni)
                .OnDelete(DeleteBehavior.Cascade);  // Eliminar los recibos si se elimina el empleado

            // Índice en EmployeeDni y PaymentDate para consultas más rápidas
            modelBuilder.Entity<PaymentReceipt>()
                .HasIndex(pr => new { pr.EmployeeDni, pr.PaymentDate })
                .HasDatabaseName("IX_PaymentReceipt_EmployeeDni_PaymentDate");

            // Configuración de propiedades para Deduction
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

            // Relación entre Deduction y PaymentReceipt (uno a muchos)
            modelBuilder.Entity<Deduction>()
                .HasOne(d => d.PaymentReceipt)
                .WithMany(pr => pr.DeductionsList)
                .HasForeignKey(d => d.PaymentReceiptId)
                .OnDelete(DeleteBehavior.Cascade);  // Eliminar deducciones si se elimina el recibo de pago

            // Índice en PaymentReceiptId para mejorar las consultas
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



            modelBuilder.Entity<Room>().HasKey(r => r.Id_Room);
            modelBuilder.Entity<Resident>().HasKey(r => r.Id_Resident);
            modelBuilder.Entity<DependencyLevel>().HasKey(dl => dl.Id_DependencyLevel);
            modelBuilder.Entity<DependencyHistory>().HasKey(dh => dh.Id_History);
            modelBuilder.Entity<ResidentApplication>().HasKey(ra => ra.Id_Relation);

            // Relaciones
            modelBuilder.Entity<Resident>()
                .HasOne(r => r.Guardian)
                .WithMany()
                .HasForeignKey(r => r.Id_Guardian);

            modelBuilder.Entity<Resident>()
                .HasOne(r => r.Room)
                .WithMany()
                .HasForeignKey(r => r.Id_Room);

            modelBuilder.Entity<DependencyHistory>()
                .HasOne(dh => dh.Resident)
                .WithMany()
                .HasForeignKey(dh => dh.Id_Resident);

            modelBuilder.Entity<DependencyHistory>()
                .HasOne(dh => dh.DependencyLevel)
                .WithMany()
                .HasForeignKey(dh => dh.Id_DependencyLevel);

            modelBuilder.Entity<ResidentApplication>()
                .HasOne(ra => ra.Resident)
                .WithMany()
                .HasForeignKey(ra => ra.Id_Resident);

            modelBuilder.Entity<ResidentApplication>()
                .HasOne(ra => ra.Applicant)
                .WithMany()
                .HasForeignKey(ra => ra.Id_Applicant);



            modelBuilder.Entity<DependencyHistory>()
      .HasOne(d => d.Resident)
      .WithMany(r => r.DependencyHistories)
      .HasForeignKey(d => d.Id_Resident)
      .OnDelete(DeleteBehavior.Cascade);  // Configura el comportamiento de eliminación en cascada si es necesario

            modelBuilder.Entity<DependencyHistory>()
                .HasOne(d => d.DependencyLevel)
                .WithMany()
                .HasForeignKey(d => d.Id_DependencyLevel)
                .OnDelete(DeleteBehavior.Restrict);  // Restricción de eliminación para mantener la consistencia




            modelBuilder.Entity<Appointment>()
            .HasKey(a => a.Id_Appointment);

            modelBuilder.Entity<AppointmentStatus>()
     .HasKey(status => status.Id_StatusAP);


            modelBuilder.Entity<HealthcareCenter>()
     .HasKey(hc => hc.Id_HC);


            modelBuilder.Entity<Specialty>()
     .HasKey(s => s.Id_Specialty);


            // Índice único para evitar citas duplicadas para el mismo residente a la misma hora
            modelBuilder.Entity<Appointment>()
                .HasIndex(a => new { a.Id_Resident, a.Date, a.Time })
                .IsUnique()
                .HasDatabaseName("idx_unique_appointment");

            // Relación Resident -> Appointments (1 a muchos)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Resident)
                .WithMany(r => r.Appointments)
                .HasForeignKey(a => a.Id_Resident)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación HealthcareCenter -> Appointments (1 a muchos)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.HealthcareCenter)
                .WithMany(hc => hc.Appointments)
                .HasForeignKey(a => a.Id_HC);

            // Relación Specialty -> Appointments (1 a muchos)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Specialty)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.Id_Specialty);

            // Relación Employee (acompañante) -> Appointments (1 a muchos)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Companion)
                .WithMany(e => e.CompanionAppointments)
                .HasForeignKey(a => a.Id_Companion)
                .OnDelete(DeleteBehavior.Restrict); // No permite eliminar empleados si están como acompañantes

            // Relación AppointmentStatus -> Appointments (1 a muchos)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.AppointmentStatus)
                .WithMany(status => status.Appointments)
                .HasForeignKey(a => a.Id_StatusAP);


            // Configuración de clave primaria para Notifications
            modelBuilder.Entity<Notification>()
                .HasKey(n => n.Id); // Clave primaria

            modelBuilder.Entity<Notification>()
                .Property(n => n.Id)
                .ValueGeneratedOnAdd(); // Auto-incremento

            // Relación Notification -> Appointment (1:1)
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Appointment)
                .WithMany() // No necesitamos la colección en Appointment
                .HasForeignKey(n => n.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade); // Eliminar notificación si se elimina la cita



            modelBuilder.Entity<Note>(entity =>
            {
                entity.HasKey(n => n.Id_Note);  // Clave primaria

                entity.Property(n => n.Reason)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(n => n.NoteDate)
                    .IsRequired();


                entity.Property(n => n.Description)
                    .HasColumnType("text");
            }
            );

            ////de aqui para abajo es el inventario

            // Configuration for UnitOfMeasure
            modelBuilder.Entity<UnitOfMeasure>()
                .HasKey(u => u.UnitOfMeasureID);

            modelBuilder.Entity<UnitOfMeasure>()
                .Property(u => u.UnitOfMeasureID)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<UnitOfMeasure>()
                .Property(u => u.UnitName)
                .IsRequired()
                .HasMaxLength(50);

            // Configuration for Category
            modelBuilder.Entity<Category>()
                .HasKey(c => c.CategoryID);

            modelBuilder.Entity<Category>()
                .Property(c => c.CategoryID)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Category>()
                .Property(c => c.CategoryName)
                .IsRequired()
                .HasMaxLength(50);

            // Configuration for Product
            modelBuilder.Entity<Product>()
                .HasKey(p => p.ProductID);

            modelBuilder.Entity<Product>()
                .Property(p => p.ProductID)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Product>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Product>()
                .Property(p => p.TotalQuantity)
                .IsRequired()
                .HasDefaultValue(0);  // Default to 0 for inventory tracking

            // Relationships for Product
            modelBuilder.Entity<Product>()
                .HasOne(p => p.UnitOfMeasure)
                .WithMany(u => u.Products)
                .HasForeignKey(p => p.UnitOfMeasureID)
                .OnDelete(DeleteBehavior.Restrict);  // Restrict deletion if in use

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuration for Inventory
            modelBuilder.Entity<Inventory>()
                .HasKey(i => i.InventoryID);

            modelBuilder.Entity<Inventory>()
                .Property(i => i.InventoryID)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Inventory>()
                .Property(i => i.Quantity)
                .IsRequired();

            modelBuilder.Entity<Inventory>()
                .Property(i => i.Date)
                .IsRequired();

            modelBuilder.Entity<Inventory>()
                .Property(i => i.MovementType)
                .IsRequired()
                .HasConversion<string>();  // Store enum as string in DB

            // Relationships for Inventory
            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.Product)
                .WithMany(p => p.Inventories)
                .HasForeignKey(i => i.ProductID)
                .OnDelete(DeleteBehavior.Cascade);  // Cascade deletion to clean up associated inventories
        }
    }
}
