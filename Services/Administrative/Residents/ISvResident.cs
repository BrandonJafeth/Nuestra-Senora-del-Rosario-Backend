using Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Administrative.Residents
{
    public interface ISvResident
    {
        Task<(IEnumerable<ResidentGetDto> Residents, int TotalPages)> GetAllResidentsAsync(int pageNumber, int pageSize);
        Task<ResidentGetDto> GetResidentByIdAsync(int id);
        Task AddResidentAsync(ResidentCreateDto residentDto);
        Task AddResidentFromApplicantAsync(ResidentFromApplicantDto dto);  // Nuevo método
        Task UpdateResidentAsync(int id, ResidentCreateDto residentDto);
        Task PatchResidentAsync(int id, ResidentPatchDto patchDto);
        Task DeleteResidentAsync(int id);
    }
}
