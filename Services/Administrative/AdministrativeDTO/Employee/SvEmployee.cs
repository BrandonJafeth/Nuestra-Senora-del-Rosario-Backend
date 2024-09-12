using AutoMapper;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Services.GenericService;
using System.Threading.Tasks;

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
            var employee = await _employeeRepository.GetByIdAsync(dni);
            return _mapper.Map<EmployeeGetDTO>(employee);
        }

        public async Task<IEnumerable<EmployeeGetDTO>> GetAllEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<EmployeeGetDTO>>(employees);
        }
    }
}
