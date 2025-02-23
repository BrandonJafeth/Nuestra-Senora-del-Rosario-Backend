using AutoMapper;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Services.GenericService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.AdministrativeDTO.EmployeeService
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

        // Crear un empleado
        public async Task CreateEmployeeAsync(EmployeeCreateDTO employeeCreateDto)
        {
            var employee = _mapper.Map<Employee>(employeeCreateDto);
            await _employeeRepository.AddAsync(employee);
            await _employeeRepository.SaveChangesAsync();
        }

        // Obtener un empleado por DNI
        public async Task<EmployeeGetDTO> GetEmployeeByIdAsync(int dni)
        {
            var employee = await _employeeRepository.Query()
                .Include(e => e.Profession)
                .Include(e => e.TypeOfSalary)
                .FirstOrDefaultAsync(e => e.Dni == dni);

            if (employee == null)
                throw new KeyNotFoundException($"Employee with DNI {dni} not found.");

            return _mapper.Map<EmployeeGetDTO>(employee);
        }

        // Actualizar un empleado
        public async Task UpdateEmployeeAsync(int dni, EmployeeUpdateDto employeeUpdateDto)
        {
            var employee = await _employeeRepository.Query()
                .FirstOrDefaultAsync(e => e.Dni == dni);

            if (employee == null)
                throw new KeyNotFoundException($"Employee with DNI {dni} not found.");

            // Actualizar campos modificables
            _mapper.Map(employeeUpdateDto, employee);

            _employeeRepository.Update(employee);
            await _employeeRepository.SaveChangesAsync();
        }

        // Obtener todos los empleados con paginación
        public async Task<(IEnumerable<EmployeeGetDTO> Employees, int TotalPages)> GetAllEmployeesAsync(int pageNumber, int pageSize)
        {
            var totalEmployees = await _employeeRepository.Query().CountAsync();

            var employees = await _employeeRepository.Query()
                .Include(e => e.Profession)
                .Include(e => e.TypeOfSalary)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalEmployees / (double)pageSize);

            return (_mapper.Map<IEnumerable<EmployeeGetDTO>>(employees), totalPages);
        }

        public async Task<IEnumerable<EmployeeByProfessionDTO>> GetEmployeesByProfessionsAsync(IEnumerable<int> professionIds)
        {
            var employees = await _employeeRepository.Query()
                .Where(e => professionIds.Contains(e.Id_Profession))
                .Select(e => new EmployeeByProfessionDTO
                {
                    Id_Employee = e.Id_Employee,
                    Dni = e.Dni,
                    FullName = $"{e.First_Name} {e.Last_Name1} {e.Last_Name2}",
                    Email = e.Email,
                    Profession = e.Profession.Name_Profession
                })
                .ToListAsync();

            return employees;
        }

    }
}
