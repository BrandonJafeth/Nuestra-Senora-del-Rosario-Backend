using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.ResidentMedications
{
    public interface ISvResidentMedication
    {
        Task<IEnumerable<ResidentMedicationGetDto>> GetAllAsync();
        Task<ResidentMedicationGetDto> GetByIdAsync(int id);
        Task<ResidentMedicationGetDto> CreateAsync(ResidentMedicationCreateDto dto);
        Task UpdateAsync(int id, ResidentMedicationUpdateDto dto);
        Task DeleteAsync(int id);
    }
}
