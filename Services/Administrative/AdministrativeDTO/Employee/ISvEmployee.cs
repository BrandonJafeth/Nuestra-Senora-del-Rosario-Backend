using Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using System.Threading.Tasks;

namespace Services.Administrative.Employees
{
    public interface ISvEmployee
    {
        Task CreateEmployeeAsync(EmployeeCreateDTO employeeCreateDTO); // Nuevo método para crear un empleado
        Task CreateEmployeeAsync(EmployeeCreateDTO employeeCreateDTO, int? roleId); // Método con rol opcional
        Task<EmployeeGetDTO> GetEmployeeByIdAsync(int dni);
        Task<(IEnumerable<EmployeeGetDTO> Employees, int TotalPages)> GetAllEmployeesAsync(int pageNumber, int pageSize);

        Task AssignRoleToEmployeeAsync(EmployeeRoleCreateDTO employeeRoleDto);//nuevo método




        // Nuevos métodos
        Task<IEnumerable<EmployeeWithRolesGetDto>> GetEmployeesWithRolesAsync();
        Task<IEnumerable<EmployeeByRoleGetDto>> GetEmployeesByRoleAsync(string roleName);

        Task<IEnumerable<EmployeeByRoleGetDto>> GetEncargadosAsync();  // Nuevo método
    }
}
