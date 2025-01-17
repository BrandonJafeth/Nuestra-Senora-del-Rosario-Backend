using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.EmployeeRoleService
{
    public interface ISvEmployeeRole
    {
        Task AssignRoleToEmployeeAsync(EmployeeRoleCreateDTO employeeRoleDto);
        Task<bool> RoleAlreadyAssignedAsync(int dniEmployee, int roleId);//nuevo método
    }
}
