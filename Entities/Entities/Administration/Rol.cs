public class Rol
{
    public int Id_Role { get; set; }
    public string Name_Role { get; set; }

    // Relación M:N con Employee
    public ICollection<EmployeeRole> EmployeeRoles { get; set; }
}
