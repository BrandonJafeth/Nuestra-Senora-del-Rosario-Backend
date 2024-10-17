using AutoMapper;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Services.GenericService;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Services.Administrative.EmployeeRoleServices;

namespace Services.Administrative.Employees
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

        // Obtener todos los empleados.
        public async Task<IEnumerable<EmployeeGetDTO>> GetAllEmployeesAsync()
        {
            var employees = await _employeeRepository
                .Query()
                .Include(e => e.Profession)
                .Include(e => e.TypeOfSalary)
                .ToListAsync();

            return _mapper.Map<IEnumerable<EmployeeGetDTO>>(employees);
        }
    }
}
