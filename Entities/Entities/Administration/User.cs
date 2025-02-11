

public class User
{
    public int Id_User { get; set; }  // Llave primaria auto-incremental
    public int DNI { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }
    public bool Is_Active { get; set; } = true;

    public DateTime? PasswordExpiration { get; set; } // Fecha de caducidad de la contraseña

    public string FullName { get; set; }  // ✅ Nuevo campo agregado

    // Relación M:N con User
    public ICollection<UserRoles> UserRoles { get; set; } = new HashSet<UserRoles>();
}
