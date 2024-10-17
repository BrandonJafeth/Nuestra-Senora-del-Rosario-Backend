using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Services.Administrative.Employees;
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

    // Crear empleado sin rol
    [HttpPost]
    public async Task<IActionResult> CreateEmployee([FromBody] EmployeeCreateDTO employeeCreateDTO)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _employeeService.CreateEmployeeAsync(employeeCreateDTO);
            return Ok(new { message = "Empleado creado exitosamente." });
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
        }
    }

    [HttpPost("with-role")]
    public async Task<IActionResult> CreateEmployeeWithRole(
    [FromBody] EmployeeCreateDTO employeeCreateDTO,
    [FromQuery] int? roleId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _employeeService.CreateEmployeeAsync(employeeCreateDTO, roleId);
            return Ok(new { message = "Empleado creado con rol asignado." });
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
        }
    }

    // Obtener un empleado por su DNI
    [HttpGet("{dni}")]
    public async Task<IActionResult> GetEmployeeById(int dni)
    {
        var employee = await _employeeService.GetEmployeeByIdAsync(dni);
        if (employee == null)
        {
            return NotFound();
        }
        return Ok(employee);
    }

    // Obtener todos los empleados
    [HttpGet]
    public async Task<IActionResult> GetAllEmployees()
    {
        var employees = await _employeeService.GetAllEmployeesAsync();
        return Ok(employees);
    }
}
