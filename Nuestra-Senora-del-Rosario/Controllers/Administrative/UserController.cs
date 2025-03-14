﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Infrastructure.Services.Administrative.Users;
using Infrastructure.Services.Administrative.EmployeeRoleService;
using Infrastructure.Services.Administrative.PasswordResetServices;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using System;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly ISvUser _userService;
    private readonly ISvUserRole _userRoleService;
    private readonly ISvPasswordResetService _passwordResetService; // Servicio de restablecimiento

    public UserController(
        ISvUser userService,
        ISvUserRole userRoleService,
        ISvPasswordResetService passwordResetService)
    {
        _userService = userService;
        _userRoleService = userRoleService;
        _passwordResetService = passwordResetService;
    }

    /// <summary>
    /// 📌 Obtener un usuario por su ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            return Ok(user);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno", error = ex.Message });
        }
    }

    /// <summary>
    /// 📌 Obtener todos los usuarios
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(new { count = users.Count(), users });
    }

    /// <summary>
    /// 📌 Crear un usuario desde un empleado
    /// </summary>
    [HttpPost("create-from-employee")]
    public async Task<IActionResult> CreateUserFromEmployee([FromQuery] int dniEmployee, [FromQuery] int idRole)
    {
        try
        {
            await _userService.CreateUserFromEmployeeAsync(dniEmployee, idRole);
            return Ok(new { message = "Usuario creado con éxito desde el empleado." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno", error = ex.Message });
        }
    }

    /// <summary>
    /// 📌 Crear un usuario nuevo manualmente
    /// </summary>
    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateDto userCreateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _userService.CreateUserAsync(userCreateDto);
            return Ok(new { message = "Usuario creado exitosamente." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno", error = ex.Message });
        }
    }

    /// <summary>
    /// 📌 Asignar un rol a un usuario
    /// </summary>
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRoleToUser([FromBody] UserRoleCreateDTO userRoleDto)
    {
        if (userRoleDto == null)
        {
            return BadRequest(new { message = "Datos inválidos." });
        }

        try
        {
            bool roleAlreadyAssigned = await _userRoleService.RoleAlreadyAssignedAsync(userRoleDto.Id_User, userRoleDto.Id_Role);
            if (roleAlreadyAssigned)
            {
                return Conflict(new { message = "El usuario ya tiene este rol asignado." });
            }

            await _userRoleService.AssignRoleToUserAsync(userRoleDto);
            return Ok(new { message = "Rol asignado exitosamente." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno", error = ex.Message });
        }
    }

    /// <summary>
    /// 📌 Solicitar restablecimiento de contraseña
    /// </summary>
    [HttpPost("password-reset/request")]
    public async Task<IActionResult> RequestPasswordReset([FromQuery] string email)
    {
        var result = await _passwordResetService.RequestPasswordResetAsync(email);

        if (!result)
        {
            return NotFound(new { message = "No se encontró un usuario con el correo proporcionado." });
        }

        return Ok(new { message = "Se ha enviado un correo para restablecer la contraseña." });
    }

    /// <summary>
    /// 📌 Restablecer la contraseña
    /// </summary>
    [HttpPost("password-reset/confirm")]
    public async Task<IActionResult> ResetPassword([FromBody] PasswordResetDTO resetDto)
    {
        try
        {
            var success = await _passwordResetService.ResetPasswordAsync(resetDto.Token, resetDto.NewPassword, resetDto.ConfirmPassword);

            if (!success)
            {
                return BadRequest(new { message = "No se pudo restablecer la contraseña." });
            }

            return Ok(new { message = "Contraseña restablecida con éxito." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // 🔹 Obtener el usuario autenticado
    [HttpGet("me")]
    public async Task<IActionResult> GetAuthenticatedUser()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized("No se pudo recuperar el ID del usuario autenticado.");
        }

        var user = await _userService.GetAuthenticatedUserAsync(userId);
        return Ok(user);

    }

    [HttpPut("update-profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UserUpdateProfileDto userUpdateProfileDto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized("No se pudo recuperar el ID del usuario autenticado.");
        }

        await _userService.UpdateUserProfileAsync(userId, userUpdateProfileDto);
        return NoContent();
    }


    [HttpPost("change-password-authenticated")]
    public async Task<IActionResult> ChangePassword([FromBody] UserChangePasswordDto userChangePasswordDto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized("No se pudo recuperar el ID del usuario autenticado.");
        }

        await _userService.ChangeAuthenticatedUserPasswordAsync(userId, userChangePasswordDto);
        return NoContent();
    }


    // 🔹 Activar/Desactivar usuario (requiere permisos de administrador)
    // PUT: api/user/{id}/status
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateUserStatus(int id, [FromBody] UserStatusUpdateDto userStatusUpdateDto)
    {
        var performedByUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        await _userService.UpdateUserStatusAsync(
            userId: id,
            isActive: userStatusUpdateDto.IsActive,
            performedByUserId: performedByUserId
        );

        return NoContent();
    }

    // GET: api/user/paginated?pageNumber=1&pageSize=10
    [HttpGet("paginated")]
    public async Task<IActionResult> GetAllUsersPaginated([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var (users, totalPages) = await _userService.GetAllUserWithPaginationAsync(pageNumber, pageSize);

            return Ok(new
            {
                users,
                totalPages
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    /// <summary>
    /// 📌 Login de usuario
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDTO loginDTO)
    {
        try
        {
            var token = await _userService.LoginAsync(loginDTO);
            return Ok(new { token });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno", error = ex.Message });
        }
    }
}
