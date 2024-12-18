using Microsoft.AspNetCore.Mvc;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Services.GenericService;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DataAccess.Entities.Informative;
using Microsoft.EntityFrameworkCore;  // Asegúrate de usar el namespace correcto para Rol

[ApiController]
[Route("api/[controller]")]
public class RolController : ControllerBase
{
    private readonly ISvGenericRepository<Rol> _rolService;
    private readonly IMapper _mapper;

    public RolController(ISvGenericRepository<Rol> rolService, IMapper mapper)
    {
        _rolService = rolService;
        _mapper = mapper;
    }

    // GET: api/rol
    [HttpGet]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _rolService.GetAllAsync();
        var rolDtos = _mapper.Map<IEnumerable<RolGetDTO>>(roles);
        return Ok(rolDtos);
    }

    // GET: api/rol/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRolById(int id)
    {
        var rol = await _rolService.GetByIdAsync(id);
        if (rol == null)
        {
            return NotFound($"Rol with ID {id} not found.");
        }
        var rolDto = _mapper.Map<RolGetDTO>(rol);
        return Ok(rolDto);
    }

    // POST: api/rol
    [HttpPost]
    public async Task<IActionResult> CreateRol([FromBody] RolCreateDTO rolCreateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);  // Devuelve errores de validación
        }

        var rol = _mapper.Map<Rol>(rolCreateDto);  // Mapea de DTO a entidad Rol

        // Verificar el mapeo
        Console.WriteLine($"Mapped Role Name: {rol.Name_Role}");

        await _rolService.AddAsync(rol);
        await _rolService.SaveChangesAsync();

        return Ok(_mapper.Map<RolGetDTO>(rol));
    }


    // PUT: api/rol/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRol(int id, [FromBody] RolCreateDTO rolCreateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);  // Devuelve los errores de validación si los hay
        }

        var existingRol = await _rolService.GetByIdAsync(id);
        if (existingRol == null)
        {
            return NotFound($"Rol with ID {id} not found.");  // Devuelve 404 si no se encuentra el rol
        }

        // Mapeo de los cambios desde el DTO hacia la entidad existente
        _mapper.Map(rolCreateDto, existingRol);

        // Verificación del mapeo
        Console.WriteLine($"Updated Role Name: {existingRol.Name_Role}");

        try
        {
            await _rolService.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, $"Error al actualizar el rol: {ex.InnerException?.Message ?? ex.Message}");
        }

        // Retorna el DTO actualizado
        return Ok(_mapper.Map<RolGetDTO>(existingRol));
    }

    // DELETE: api/rol/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRol(int id)
    {
        var existingRol = await _rolService.GetByIdAsync(id);
        if (existingRol == null)
        {
            return NotFound($"Rol with ID {id} not found.");
        }

        await _rolService.DeleteAsync(id);
        await _rolService.SaveChangesAsync();
        return NoContent();
    }
}
