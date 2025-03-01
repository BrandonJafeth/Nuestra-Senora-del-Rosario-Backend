using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.Users
{
    public interface ISvUser
    {
        // Crear un usuario desde un empleado
        Task CreateUserFromEmployeeAsync(int dniEmployee, int idRole);

        // Crear un usuario directamente
        Task CreateUserAsync(UserCreateDto userCreateDto);

        // Obtener un usuario por ID
        Task<UserGetDto> GetUserByIdAsync(int id);

        // Obtener todos los usuarios
        Task<IEnumerable<UserGetDto>> GetAllUsersAsync();

        // Asignar un rol a un usuario
        Task AssignRoleToUserAsync(UserRoleCreateDTO userRoleCreateDto);

        // Autenticación (login)
        Task<string> LoginAsync(UserLoginDTO loginDto);


        Task<UserGetMeDto> GetAuthenticatedUserAsync(int userId);
        Task UpdateUserProfileAsync(int userId, UserUpdateProfileDto userUpdateProfileDto);
        Task ChangeAuthenticatedUserPasswordAsync(int userId, UserChangePasswordDto userChangePasswordDto);
        Task UpdateUserStatusAsync(int userId, bool isActive, int performedByUserId);

 


    }
}
