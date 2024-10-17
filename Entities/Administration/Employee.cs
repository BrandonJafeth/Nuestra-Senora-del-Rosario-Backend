using Entities.Administration;

public class Employee
{
    public int Dni { get; set; }  // Usamos el DNI como clave primaria
    public string First_Name { get; set; }
    public string Last_Name1 { get; set; }
    public string Last_Name2 { get; set; }
    public string Phone_Number { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
    public string Emergency_Phone { get; set; }

    public int Id_TypeOfSalary { get; set; }
    public TypeOfSalary TypeOfSalary { get; set; }

    public int Id_Profession { get; set; }
    public Profession Profession { get; set; }

    // Relación M:N con Rol
    public ICollection<EmployeeRole> EmployeeRoles { get; set; }

    public ICollection<PaymentReceipt> PaymentReceipts { get; set; }  // Relación 1:N con PaymentReceipt

    public ICollection<Appointment> CompanionAppointments { get; set; }

}
