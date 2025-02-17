﻿using Domain.Entities.Administration;
using Domain.Entities.Informative;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.AppDbContext
{
    public partial class AppDbContext
    {
        private void ConfigureAdministrativeEntities(ModelBuilder modelBuilder)
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
                .HasKey(e => e.Id_Employee);

            modelBuilder.Entity<Employee>()
            .Property(e => e.Id_Employee)
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<Employee>()
            .Property(e => e.Dni)
            .IsRequired();


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
            .Property(u => u.Id_User)
            .ValueGeneratedOnAdd();

            // Configuración para UserRoles (relación M:N entre User y Rol)
            modelBuilder.Entity<UserRoles>()
                .HasKey(ur => new { ur.Id_User, ur.Id_Role });

            modelBuilder.Entity<UserRoles>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.Id_User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRoles>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.Id_Role)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración para ApplicationStatus
            modelBuilder.Entity<ApplicationStatus>()
                .HasKey(s => s.Id_Status);
            modelBuilder.Entity<ApplicationStatus>()
                .Property(s => s.Id_Status)
                .ValueGeneratedOnAdd();



            // Configuración para Guardian
            modelBuilder.Entity<Guardian>()
                .HasKey(g => g.Id_Guardian);
            modelBuilder.Entity<Guardian>()
                .Property(g => g.Id_Guardian)
                .ValueGeneratedOnAdd();

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
                .HasForeignKey(pr => pr.Id_Employee)
                .HasConstraintName("fk_paymentreceipts_employee")
                .OnDelete(DeleteBehavior.Cascade);  // Eliminar los recibos si se elimina el empleado

            // Índice en EmployeeDni y PaymentDate para consultas más rápidas
            modelBuilder.Entity<PaymentReceipt>()
                .HasIndex(pr => new { pr.Id_Employee, pr.PaymentDate })
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

            // Configuración adicional para PasswordResetToken
            modelBuilder.Entity<PasswordResetToken>()
                .HasKey(p => p.Id);



            modelBuilder.Entity<Room>().HasKey(r => r.Id_Room);
            modelBuilder.Entity<Resident>().HasKey(r => r.Id_Resident);
            modelBuilder.Entity<DependencyLevel>().HasKey(dl => dl.Id_DependencyLevel);
            modelBuilder.Entity<DependencyHistory>().HasKey(dh => dh.Id_History);

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
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ResidentMedication>()
          .HasKey(rm => rm.Id_ResidentMedication);


            modelBuilder.Entity<MedicationSpecific>()
                .HasKey(e => e.Id_MedicamentSpecific);

            modelBuilder.Entity<MedicationSpecific>(entity =>
            {
                entity.ToTable("MedicationSpecifics");

                entity.Property(e => e.Name_MedicamentSpecific)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.SpecialInstructions)
                    .HasMaxLength(1000);

                entity.Property(e => e.AdministrationSchedule)
                    .HasMaxLength(500);

                entity.HasOne(e => e.UnitOfMeasure)
                    .WithMany(u => u.MedicationsSpecific)
                    .HasForeignKey(e => e.UnitOfMeasureID);

                entity.HasOne(e => e.AdministrationRoute)
                    .WithMany(a => a.MedicationSpecifics)
                    .HasForeignKey(e => e.Id_AdministrationRoute);
            });

            modelBuilder.Entity<AdministrationRoute>()
                .HasKey(e => e.Id_AdministrationRoute);

            modelBuilder.Entity<AdministrationRoute>(entity =>
            {
                entity.ToTable("AdministrationRoutes");

                entity.Property(e => e.RouteName)
                    .IsRequired()
                    .HasMaxLength(100);
            });



            modelBuilder.Entity<ResidentMedication>(entity =>
            {
                entity.ToTable("ResidentMedications");
                entity.Property(e => e.StartDate)
                    .IsRequired();
                entity.Property(e => e.EndDate)
                    .IsRequired();
                entity.Property(e => e.PrescribedDose)
                    .IsRequired();
                entity.HasOne(e => e.Resident)
                    .WithMany(r => r.ResidentMedications)
                    .HasForeignKey(e => e.Id_Resident);
                entity.HasOne(e => e.MedicationSpecific)
                    .WithMany(m => m.ResidentMedications)
                    .HasForeignKey(e => e.Id_MedicamentSpecific);
            });


             modelBuilder.Entity<Pathology>()
                .HasKey(p => p.Id_Pathology);

            modelBuilder.Entity<ResidentPathology>()
                .HasKey(rp => rp.Id_ResidentPathology);
            modelBuilder.Entity<ResidentMedication>()
                .HasKey(rm => rm.Id_ResidentMedication);

        }
    }
}
