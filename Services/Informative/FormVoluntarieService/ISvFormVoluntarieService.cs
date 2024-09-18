using Entities.Informative;
using Services.GenericService;
using Services.Informative.DTOS;
using Services.Informative.DTOS.CreatesDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Informative.FormVoluntarieServices
{
    public interface ISvFormVoluntarieService : ISvGenericRepository<FormVoluntarie>
    {
        Task<IEnumerable<FormVoluntarieDto>> GetAllFormVoluntariesWithTypeAsync();

        // Obtener una solicitud por su ID con el tipo de voluntariado y estado
        Task<FormVoluntarieDto> GetFormVoluntarieWithTypeByIdAsync(int id);

        // Crear una nueva solicitud de voluntariado
        Task CreateFormVoluntarieAsync(FormVoluntarieCreateDto formVoluntarieCreateDto);

        // Actualizar una solicitud de voluntariado
        // Actualizar solo el estado de una solicitud de voluntariado
        Task UpdateFormVoluntarieStatusAsync(int id, int statusId);

        // Eliminar una solicitud de voluntariado
        Task DeleteFormVoluntarieAsync(int id);
    }
}
