using Microsoft.AspNetCore.Mvc;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Services.Administrative.PasswordResetServices;

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
    }
}
