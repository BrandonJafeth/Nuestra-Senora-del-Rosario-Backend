using DataAccess.Entities.Informative;
using Services.GenericService;
using Services.Informative.DTOS;
using Services.Informative.DTOS.CreatesDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Informative.FormDonationService
{
    public interface ISvFormDonation : ISvGenericRepository<FormDonation>
    {
        // Obtener todas las donaciones con sus detalles (DonationType, MethodDonation, Status)
        Task<(IEnumerable<FormDonationDto> Donations, int TotalPages)> GetFormDonationsWithDetailsAsync(int pageNumber, int pageSize);

        // Obtener una donación por ID con sus detalles
        Task<FormDonationDto> GetFormDonationWithDetailsByIdAsync(int id);

        // Crear una nueva donación con estado por defecto
        Task CreateFormDonationAsync(FormDonationCreateDto formDonationCreateDto);

        // Actualizar el estado de una donación
        Task UpdateFormDonationStatusAsync(int id, int statusId);
    }
}
