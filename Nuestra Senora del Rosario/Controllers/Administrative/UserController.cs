using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Infrastructure.Services.Administrative.Users;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ISvUser _userService;

    public UserController(ISvUser userService)
    {
        _userService = userService;
    }

    // Obtener un usuario por su ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    // Obtener todos los usuarios
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    // Crear un usuario a partir de un empleado
    [HttpPost("create-from-employee/{dniEmployee}/{idRole}")]
    public async Task<IActionResult> CreateUserFromEmployee(int dniEmployee, int idRole)
    {
        try
        {
            await _userService.CreateUserFromEmployeeAsync(dniEmployee, idRole);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // Nuevo método para realizar el login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDTO loginDTO)
    {
        var token = await _userService.LoginAsync(loginDTO);

        if (token == null)
        {
            return Unauthorized("Invalid login attempt");
        }

        // Retornar el token si las credenciales son válidas
        return Ok(new { token });
    }
}
