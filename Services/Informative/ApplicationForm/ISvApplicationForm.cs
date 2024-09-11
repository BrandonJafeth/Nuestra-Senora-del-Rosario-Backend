using Entities.Informative;
using System.Collections.Generic;
using System.Threading.Tasks;
using Services.Informative.DTOS.CreatesDto; // Asegúrate de incluir el DTO

namespace Services.Informative.ApplicationFormService
{
    public interface ISvApplicationForm
    {
        Task<IEnumerable<ApplicationForm>> GetAllFormsAsync();    // Obtener todas las solicitudes
        Task<ApplicationForm> GetFormByIdAsync(int id);           // Obtener una solicitud por su ID
        Task AddFormAsync(ApplicationFormCreateDto formCreateDto); // Agregar una nueva solicitud usando el DTO
       
        Task DeleteAsync(int id);                                 // Eliminar una solicitud
    }
}
