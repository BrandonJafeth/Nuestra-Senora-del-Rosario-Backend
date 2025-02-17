public class Rol
{
    public int Id_Role { get; set; }
    public string Name_Role { get; set; }

    // Relación M:N con Roles
    public ICollection<UserRoles> UserRoles { get; set; } = new HashSet<UserRoles>();
}
