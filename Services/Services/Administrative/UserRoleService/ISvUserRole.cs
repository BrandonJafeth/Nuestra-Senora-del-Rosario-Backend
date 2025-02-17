using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.EmployeeRoleService
{
    public interface ISvUserRole
    {
     
        Task AssignRoleToUserAsync(UserRoleCreateDTO userRoleDto);

     
        Task<bool> RoleAlreadyAssignedAsync(int userId, int roleId);
    }
}
