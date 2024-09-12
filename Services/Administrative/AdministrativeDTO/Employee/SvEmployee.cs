using AutoMapper;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Services.GenericService;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Services.Administrative.Employees
{
    public class SvEmployee : ISvEmployee
    {
        private readonly ISvGenericRepository<Employee> _employeeRepository;
        private readonly IMapper _mapper;

        public SvEmployee(ISvGenericRepository<Employee> employeeRepository, IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        // Método para crear un empleado
        public async Task CreateEmployeeAsync(EmployeeCreateDTO employeeCreateDTO)
        {
            var employee = _mapper.Map<Employee>(employeeCreateDTO);
            await _employeeRepository.AddAsync(employee);
            await _employeeRepository.SaveChangesAsync();
        }

        public async Task<EmployeeGetDTO> GetEmployeeByIdAsync(int dni)
        {
            var employee = await _employeeRepository
                .Query()  // Usamos el método "Query" que devuelve IQueryable para poder hacer Include
                .Include(e => e.Profession)
                .Include(e => e.TypeOfSalary)
                .FirstOrDefaultAsync(e => e.Dni == dni);  // Filtrar por DNI

            if (employee == null)
            {
                return null;
            }

            // Mapeamos la entidad a DTO
            return _mapper.Map<EmployeeGetDTO>(employee);
        }

        public async Task<IEnumerable<EmployeeGetDTO>> GetAllEmployeesAsync()
        {
            var employees = await _employeeRepository
                .Query()  // Usamos el método "Query" que devuelve IQueryable para poder hacer Include
                .Include(e => e.Profession)
                .Include(e => e.TypeOfSalary)
                .ToListAsync();

            return _mapper.Map<IEnumerable<EmployeeGetDTO>>(employees);
        }

    }
}
