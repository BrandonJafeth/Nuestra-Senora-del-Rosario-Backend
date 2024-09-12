using Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Administrative.Users
{
    public interface ISvUser
    {
        Task CreateUserFromEmployeeAsync(int dniEmployee, int idRole);  // Método actualizado para incluir idRole
        Task<UserGetDTO> GetUserByIdAsync(int id);
        Task<IEnumerable<UserGetDTO>> GetAllUsersAsync();
        Task<string> LoginAsync(UserLoginDTO loginDTO); // Método para login
    }
}
