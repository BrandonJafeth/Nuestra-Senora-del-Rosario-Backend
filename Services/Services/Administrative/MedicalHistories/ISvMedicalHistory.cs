using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.MedicalHistories
{
    public interface ISvMedicalHistory
    {
        Task<MedicalHistoryGetDto> GetByIdAsync(int id);
        Task<IEnumerable<MedicalHistoryGetDto>> GetByResidentIdAsync(int residentId);
        Task<MedicalHistoryGetDto> CreateAsync(MedicalHistoryCreateDto dto);
        Task UpdateAsync(int id, MedicalHistoryUpdateDto dto);
        Task DeleteAsync(int id);
    }
}
