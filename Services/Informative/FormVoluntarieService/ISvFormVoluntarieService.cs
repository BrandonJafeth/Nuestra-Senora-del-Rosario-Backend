using Entities.Informative;
using Services.GenericService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Informative.FormVoluntarieServices
{
    public interface ISvFormVoluntarieService : ISvGenericRepository<FormVoluntarie>
    {
        Task<IEnumerable<FormVoluntarie>> GetAllFormVoluntariesWithTypeAsync();  // Obtener todas las solicitudes con su tipo de voluntariado
        Task<FormVoluntarie> GetFormVoluntarieWithTypeByIdAsync(int id);  // Obtener una solicitud por su ID con el tipo de voluntariado
    }
}
