using Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Services.Administrative.EmployeeRoleServices;
using Services.GenericService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Administrative.EmployeeRoleServices
{
    public class SvEmployeeRole : ISvEmployeeRole
    {
        private readonly ISvGenericRepository<EmployeeRole> _employeeRoleRepository;

        public SvEmployeeRole(ISvGenericRepository<EmployeeRole> employeeRoleRepository)
        {
            _employeeRoleRepository = employeeRoleRepository;
        }

        public async Task AssignRoleToEmployeeAsync(EmployeeRoleCreateDTO employeeRoleDto)
        {
            var employeeRole = new EmployeeRole
            {
                Dni_Employee = employeeRoleDto.DniEmployee,
                Id_Role = employeeRoleDto.IdRole
            };

            await _employeeRoleRepository.AddAsync(employeeRole);
            await _employeeRoleRepository.SaveChangesAsync();
        }
    }
}
