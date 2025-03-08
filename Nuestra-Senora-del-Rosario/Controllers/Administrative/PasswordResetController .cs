using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.PasswordResetServices;
using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

namespace Nuestra_Senora_del_Rosario.Controllers.Administrative
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordResetController : ControllerBase
    {
        private readonly ISvPasswordResetService _passwordResetService;


        public PasswordResetController(ISvPasswordResetService passwordResetService)
        {
            _passwordResetService = passwordResetService;
            //_passwordResetService = passwordResetService;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _passwordResetService.RequestPasswordResetAsync(request.Email);
            if (!result)
            {
                return NotFound("Employee with this email does not exist.");
            }

            return Ok("Password reset link has been sent.");
        }

        [HttpPost("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] PasswordResetDTO passwordUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Valida el token
            var isValidToken = await _passwordResetService.ValidatePasswordResetTokenAsync(passwordUpdateDto.Token);
            if (!isValidToken)
            {
                return BadRequest("Token inválido o expirado.");
            }

            // Lógica para verificar que la nueva contraseña y su confirmación sean iguales
            if (passwordUpdateDto.NewPassword != passwordUpdateDto.ConfirmPassword)
            {
                return BadRequest("Las contraseñas no coinciden.");
            }

            // Restablecer la contraseña en el servicio
            var result = await _passwordResetService.ResetPasswordAsync(passwordUpdateDto.Token, passwordUpdateDto.NewPassword, passwordUpdateDto.ConfirmPassword);

            if (!result)
            {
                return BadRequest("No se pudo actualizar la contraseña.");
            }

            return Ok("Contraseña actualizada correctamente.");
        }

    }
}