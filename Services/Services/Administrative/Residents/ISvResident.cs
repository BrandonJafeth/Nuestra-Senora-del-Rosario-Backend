using Domain.Entities.Administration;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.Residents
{
    public interface ISvResident
    {
        Task<(IEnumerable<ResidentGetDto> Residents, int TotalPages)> GetAllResidentsAsync(int pageNumber, int pageSize);

        Task<IEnumerable<ResidentGetDto>> GetAllResidentsAsync();
        Task<ResidentGetDto> GetResidentByIdAsync(int id);
        Task<Resident> GetResidentByCedulaAsync(string cedula);

        Task<ResidentMinimalInfoDto> GetResidentMinimalInfoAsync(int id);
        Task AddResidentAsync(ResidentCreateDto residentDto);
        Task AddResidentFromApplicantAsync(ResidentFromApplicantDto dto);  
        Task UpdateResidentAsync(int id, ResidentCreateDto residentDto);
        Task PatchResidentAsync(int id, ResidentPatchDto patchDto);
        Task DeleteResidentAsync(int id);
    }
}
