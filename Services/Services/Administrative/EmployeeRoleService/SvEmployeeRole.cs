using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Services.GenericService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.EmployeeRoleService
{
    public class SvEmployeeRole : ISvEmployeeRole
    {
        private readonly ISvGenericRepository<EmployeeRole> _employeeRoleRepository;
        private readonly ISvGenericRepository<Employee> _employeeRepository;

        public SvEmployeeRole(
            ISvGenericRepository<EmployeeRole> employeeRoleRepository,
            ISvGenericRepository<Employee> employeeRepository)
        {
            _employeeRoleRepository = employeeRoleRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task AssignRoleToEmployeeAsync(EmployeeRoleCreateDTO employeeRoleDto)
        {
            var employeeExists = await _employeeRepository.ExistsAsync(e => e.Dni == employeeRoleDto.DniEmployee);
            if (!employeeExists)
            {
                throw new InvalidOperationException($"El empleado con DNI {employeeRoleDto.DniEmployee} no existe.");
            }

            var roleExists = await RoleAlreadyAssignedAsync(employeeRoleDto.DniEmployee, employeeRoleDto.IdRole);
            if (roleExists)
            {
                throw new InvalidOperationException("El empleado ya tiene asignado este rol.");
            }

            var employeeRole = new EmployeeRole
            {
                Dni_Employee = employeeRoleDto.DniEmployee,
                Id_Role = employeeRoleDto.IdRole
            };

            await _employeeRoleRepository.AddAsync(employeeRole);
            await _employeeRoleRepository.SaveChangesAsync();
        }

        // Nuevo método: Verificar si un rol ya está asignado
        public async Task<bool> RoleAlreadyAssignedAsync(int dniEmployee, int roleId)
        {
            return await _employeeRoleRepository.ExistsAsync(
                er => er.Dni_Employee == dniEmployee && er.Id_Role == roleId);
        }
    }

}
