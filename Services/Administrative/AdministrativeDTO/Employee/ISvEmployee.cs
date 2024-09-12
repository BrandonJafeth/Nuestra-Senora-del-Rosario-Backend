using Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using System.Threading.Tasks;

namespace Services.Administrative.Employees
{
    public interface ISvEmployee
    {
        Task CreateEmployeeAsync(EmployeeCreateDTO employeeCreateDTO); // Nuevo método para crear un empleado
        Task<EmployeeGetDTO> GetEmployeeByIdAsync(int dni);
        Task<IEnumerable<EmployeeGetDTO>> GetAllEmployeesAsync();
    }
}
