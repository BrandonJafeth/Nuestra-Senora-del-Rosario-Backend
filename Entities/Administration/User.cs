

public class User
{
    public int Id_User { get; set; }  // Llave primaria auto-incremental
    public int Dni_Employee { get; set; }  // Relación 1:1 con Employee
    public Employee Employee { get; set; }
    
    public string Password { get; set; }
    public bool Is_Active { get; set; } = true;
}
