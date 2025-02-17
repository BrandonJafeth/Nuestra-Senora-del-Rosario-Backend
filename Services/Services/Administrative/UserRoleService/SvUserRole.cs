using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Services.GenericService;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.EmployeeRoleService
{
    public class SvUserRole : ISvUserRole
    {
        private readonly ISvGenericRepository<UserRoles> _userRoleRepository;
        private readonly ISvGenericRepository<User> _userRepository;

        public SvUserRole(
            ISvGenericRepository<UserRoles> userRoleRepository,
            ISvGenericRepository<User> userRepository)
        {
            _userRoleRepository = userRoleRepository;
            _userRepository = userRepository;
        }

        // 📌 Asignar un rol a un usuario
        public async Task AssignRoleToUserAsync(UserRoleCreateDTO userRoleDto)
        {
            try
            {
                // Verificar si el usuario existe
                var userExists = await _userRepository.ExistsAsync(u => u.Id_User == userRoleDto.Id_User);
                if (!userExists)
                {
                    throw new KeyNotFoundException($"El usuario con ID {userRoleDto.Id_User} no existe.");
                }

                // Verificar si el rol ya está asignado
                var roleExists = await RoleAlreadyAssignedAsync(userRoleDto.Id_User, userRoleDto.Id_Role);
                if (roleExists)
                {
                    throw new InvalidOperationException("El usuario ya tiene asignado este rol.");
                }

                // Crear la relación usuario-rol
                var userRole = new UserRoles
                {
                    Id_User = userRoleDto.Id_User,
                    Id_Role = userRoleDto.Id_Role
                };

                await _userRoleRepository.AddAsync(userRole);
                await _userRoleRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al asignar el rol al usuario.", ex);
            }
        }

        // 📌 Verificar si un usuario ya tiene un rol asignado
        public async Task<bool> RoleAlreadyAssignedAsync(int userId, int roleId)
        {
            return await _userRoleRepository.ExistsAsync(
                ur => ur.Id_User == userId && ur.Id_Role == roleId);
        }
    }
}
