
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Informative.DTOS;
using Infrastructure.Services.Informative.DTOS.CreatesDto;

namespace Infrastructure.Services.Informative.ApplicationFormService
{
    public interface ISvApplicationForm
    {
        Task<(IEnumerable<ApplicationFormDto> Forms, int TotalPages)> GetAllFormsAsync(int pageNumber, int pageSize);
        Task<ApplicationFormDto> GetFormByIdAsync(int id);        // Devuelve un DTO específico por ID
        Task<int> AddFormAsync(ApplicationFormCreateDto formCreateDto); // Ahora devuelve el ID generado
        Task DeleteAsync(int id);                                 // Eliminar una solicitud

        Task UpdateFormStatusAsync(int id, int statusId);

        Task<ApplicationFormDto> UpdateFormAsync(int id, ApplicationFormUpdateDto updateDto);
    }
}
