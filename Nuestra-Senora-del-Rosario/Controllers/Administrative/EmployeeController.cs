using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Infrastructure.Services.Administrative.AdministrativeDTO.EmployeeService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly ISvEmployee _employeeService;

    public EmployeeController(ISvEmployee employeeService)
    {
        _employeeService = employeeService;
    }

    // Crear un empleado
    [HttpPost]
    public async Task<IActionResult> CreateEmployee([FromBody] EmployeeCreateDTO employeeCreateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _employeeService.CreateEmployeeAsync(employeeCreateDto);
            return Ok(new { message = "Empleado creado exitosamente." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno: {ex.Message}");
        }
    }

    // Obtener un empleado por DNI
    [HttpGet("{dni}")]
    public async Task<IActionResult> GetEmployeeById(int dni)
    {
        try
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(dni);
            return Ok(employee);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno: {ex.Message}");
        }
    }

    // Actualizar un empleado
    [HttpPut("{dni}")]
    public async Task<IActionResult> UpdateEmployee(int dni, [FromBody] EmployeeUpdateDto employeeUpdateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _employeeService.UpdateEmployeeAsync(dni, employeeUpdateDto);
            return Ok(new { message = "Empleado actualizado exitosamente." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno: {ex.Message}");
        }
    }

    // Obtener todos los empleados con paginación
    [HttpGet]
    public async Task<IActionResult> GetAllEmployees(int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            var result = await _employeeService.GetAllEmployeesAsync(pageNumber, pageSize);
            return Ok(new
            {
                employees = result.Employees,
                totalPages = result.TotalPages
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno: {ex.Message}");
        }
    }

    // GET: api/Employee/by-professions?professionIds=1&professionIds=2&professionIds=3
    [HttpGet("by-professions")]
    public async Task<IActionResult> GetEmployeesByProfessions([FromQuery] IEnumerable<int> professionIds)
    {
        try
        {
            var employees = await _employeeService.GetEmployeesByProfessionsAsync(professionIds);
            return Ok(employees);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno: {ex.Message}");
        }
    }


    [HttpGet("filter")]
    public async Task<IActionResult> FilterEmployees(
             [FromQuery] EmployeeFilterDTO filter,
             [FromQuery] int pageNumber = 1,
             [FromQuery] int pageSize = 10)
    {
        var result = await _employeeService.FilterEmployeesAsync(filter, pageNumber, pageSize);
        return Ok(result);
    }

}
