
using Entities.Informative;
using Services.GenericService;
using System.Threading.Tasks;

namespace Services.Administrative.FormVoluntarieService
{
    public interface IAdministrativeFormVoluntarieService : ISvGenericRepository<FormVoluntarie>
    {
        Task<IEnumerable<FormVoluntarie>> GetAllFormVoluntariesWithTypeAsync();  // Obtener todas las solicitudes con su tipo de voluntariado
        Task<FormVoluntarie> GetFormVoluntarieWithTypeByIdAsync(int id);
    }
}
