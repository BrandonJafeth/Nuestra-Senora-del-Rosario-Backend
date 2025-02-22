using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.MedicationSpecifics
{
    public interface ISvMedicationSpecific
    {
        Task<IEnumerable<MedicationSpecificGetDto>> GetAllAsync();
        Task<MedicationSpecificGetDto> GetByIdAsync(int id);
        Task<MedicationSpecificGetDto> CreateAsync(MedicationSpecificCreateDto dto);
        Task UpdateAsync(int id, MedicationSpecificUpdateDto dto);
        Task DeleteAsync(int id);
    }
}
