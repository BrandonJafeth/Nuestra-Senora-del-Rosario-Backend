using AutoMapper;

using Services.GenericService;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Services.Administrative.EmployeeRoleService;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.EmployeeService
{
    public class SvEmployee : ISvEmployee
    {
        private readonly ISvGenericRepository<Employee> _employeeRepository;
        private readonly ISvEmployeeRole _employeeRoleService;
        private readonly IMapper _mapper;

        // Constructor
        public SvEmployee(
            ISvGenericRepository<Employee> employeeRepository,
            ISvEmployeeRole employeeRoleService,
            IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _employeeRoleService = employeeRoleService;
            _mapper = mapper;
        }

        // Asignar un rol a un empleado.
        public async Task AssignRoleToEmployeeAsync(EmployeeRoleCreateDTO employeeRoleDto)
        {
            await _employeeRoleService.AssignRoleToEmployeeAsync(employeeRoleDto);
        }

        // Crear un empleado sin rol.
        public async Task CreateEmployeeAsync(EmployeeCreateDTO employeeCreateDTO)
        {
            var employee = _mapper.Map<Employee>(employeeCreateDTO);
            await _employeeRepository.AddAsync(employee);
            await _employeeRepository.SaveChangesAsync();
        }

        // Crear un empleado con un rol opcional.
        public async Task CreateEmployeeAsync(EmployeeCreateDTO employeeCreateDTO, int? roleId)
        {
            var employee = _mapper.Map<Employee>(employeeCreateDTO);
            await _employeeRepository.AddAsync(employee);
            await _employeeRepository.SaveChangesAsync();

            if (roleId.HasValue)
            {
                var roleDto = new EmployeeRoleCreateDTO
                {
                    DniEmployee = employee.Dni,
                    IdRole = roleId.Value
                };
                await _employeeRoleService.AssignRoleToEmployeeAsync(roleDto);
            }
        }

        // Obtener un empleado por DNI.
        public async Task<EmployeeGetDTO> GetEmployeeByIdAsync(int dni)
        {
            var employee = await _employeeRepository
                .Query()
                .Include(e => e.Profession)
                .Include(e => e.TypeOfSalary)
                .FirstOrDefaultAsync(e => e.Dni == dni);

            if (employee == null)
            {
                return null;
            }

            return _mapper.Map<EmployeeGetDTO>(employee);
        }


        // Obtener empleados con sus roles
        public async Task<IEnumerable<EmployeeWithRolesGetDto>> GetEmployeesWithRolesAsync()
        {
            var employees = await _employeeRepository.Query()
                .Include(e => e.EmployeeRoles)
                    .ThenInclude(er => er.Rol)
                .ToListAsync();

            return employees.Select(e => new EmployeeWithRolesGetDto
            {
                Dni = e.Dni,
                FullName = $"{e.First_Name} {e.Last_Name1} {e.Last_Name2}",
                Roles = e.EmployeeRoles.Select(er => er.Rol.Name_Role).ToList()
            });
        }

        // Obtener empleados por un rol específico
        public async Task<IEnumerable<EmployeeByRoleGetDto>> GetEmployeesByRoleAsync(string roleName)
        {
            var employees = await _employeeRepository.Query()
                .Include(e => e.EmployeeRoles)
                    .ThenInclude(er => er.Rol)
                .Where(e => e.EmployeeRoles.Any(er => er.Rol.Name_Role == roleName))
                .ToListAsync();

            return employees.Select(e => new EmployeeByRoleGetDto
            {
                Dni = e.Dni,
                FullName = $"{e.First_Name} {e.Last_Name1} {e.Last_Name2}",
                RoleName = roleName
            });
        }

        public async Task<IEnumerable<EmployeeByRoleGetDto>> GetEncargadosAsync()
        {
            var employees = await _employeeRepository.Query()
                .Include(e => e.EmployeeRoles)
                    .ThenInclude(er => er.Rol)
                .Where(e => e.EmployeeRoles.Any(er => er.Rol.Name_Role == "Encargado"))  // Filtro por rol
                .ToListAsync();

            return employees.Select(e => new EmployeeByRoleGetDto
            {
                Dni = e.Dni,
                FullName = $"{e.First_Name} {e.Last_Name1} {e.Last_Name2}",
                RoleName = "Encargado"
            });
        }


        // Obtener todos los empleados.
        public async Task<(IEnumerable<EmployeeGetDTO> Employees, int TotalPages)> GetAllEmployeesAsync(int pageNumber, int pageSize)
        {
            var totalEmployees = await _employeeRepository.Query().CountAsync();

            var employees = await _employeeRepository
                .Query()
                .Include(e => e.Profession)
                .Include(e => e.TypeOfSalary)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalEmployees / (double)pageSize);

            return (_mapper.Map<IEnumerable<EmployeeGetDTO>>(employees), totalPages);
        }

    }
}
