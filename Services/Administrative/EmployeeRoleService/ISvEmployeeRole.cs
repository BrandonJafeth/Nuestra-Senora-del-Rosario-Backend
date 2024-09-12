using Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Administrative.EmployeeRoleServices
{
    public interface ISvEmployeeRole
    {
        Task AssignRoleToEmployeeAsync(EmployeeRoleCreateDTO employeeRoleDto);
    }
}
