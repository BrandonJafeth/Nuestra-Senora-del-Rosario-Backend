﻿using Entities.Informative;
using System.Collections.Generic;
using System.Threading.Tasks;
using Services.Informative.DTOS.CreatesDto; // Asegúrate de incluir el DTO
using Services.Informative.DTOS; // Incluye el DTO de ApplicationForm

namespace Services.Informative.ApplicationFormService
{
    public interface ISvApplicationForm
    {
        Task<(IEnumerable<ApplicationFormDto> Forms, int TotalPages)> GetAllFormsAsync(int pageNumber, int pageSize);
        Task<ApplicationFormDto> GetFormByIdAsync(int id);        // Devuelve un DTO específico por ID
        Task AddFormAsync(ApplicationFormCreateDto formCreateDto); // Agregar una nueva solicitud usando el DTO
        Task DeleteAsync(int id);                                 // Eliminar una solicitud

        Task UpdateFormStatusAsync(int id, int statusId);
    }
}
