using Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Administrative.Users
{
    public  interface ISvUser
    {
        Task CreateUserFromEmployeeAsync(int dniEmployee);
        Task<UserGetDTO> GetUserByIdAsync(int id);
        Task<IEnumerable<UserGetDTO>> GetAllUsersAsync();
    }
}
