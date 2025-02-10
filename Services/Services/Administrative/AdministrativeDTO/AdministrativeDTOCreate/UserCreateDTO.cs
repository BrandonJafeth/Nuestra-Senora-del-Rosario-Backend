



public class UserCreateDto
{
    public int Dni { get; set; } // DNI del usuario
    public string Email { get; set; } // Correo del usuario
    public string Password { get; set; } // Contraseña del usuario
    public bool IsActive { get; set; } = true; // Estado del usuario

}

