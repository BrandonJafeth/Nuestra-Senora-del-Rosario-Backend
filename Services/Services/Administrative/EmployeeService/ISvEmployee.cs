using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.EmployeeService
{
    public interface ISvEmployee
    {
        Task CreateEmployeeAsync(EmployeeCreateDTO employeeCreateDto);

        // Obtener un empleado por DNI
        Task<EmployeeGetDTO> GetEmployeeByIdAsync(int dni);

        // Obtener todos los empleados con paginación
        Task<(IEnumerable<EmployeeGetDTO> Employees, int TotalPages)> GetAllEmployeesAsync(int pageNumber, int pageSize);


        Task<(IEnumerable<EmployeeFilterDTO> Employees, int TotalPages)> FilterEmployeesAsync(EmployeeFilterDTO filter, int pageNumber, int pageSize);

        Task UpdateEmployeeAsync(int dni, EmployeeUpdateDto employeeUpdateDto);

        Task<IEnumerable<EmployeeByProfessionDTO>> GetEmployeesByProfessionsAsync(IEnumerable<int> professionIds);


    }
}
