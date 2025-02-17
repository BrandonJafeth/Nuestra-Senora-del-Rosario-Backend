using Services.GenericService;
using System.Threading.Tasks;
using Domain.Entities.Informative;

namespace Infrastructure.Services.Administrative.FormVoluntarieService
{
    public interface IAdministrativeFormVoluntarieService: ISvGenericRepository<FormVoluntarie>
    {
        Task<IEnumerable<FormVoluntarie>> GetAllFormVoluntariesWithTypeAsync();
        Task<FormVoluntarie> GetFormVoluntarieWithTypeByIdAsync(int id);
    }
}
