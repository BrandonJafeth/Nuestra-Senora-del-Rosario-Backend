using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.ResidentPathologies
{
    public interface ISvResidentPathology
    {
        Task<IEnumerable<ResidentPathologyGetDto>> GetAllAsync();
        Task<ResidentPathologyGetDto> GetByIdAsync(int id);
        Task<ResidentPathologyGetDto> CreateAsync(ResidentPathologyCreateDto dto);
        Task UpdateAsync(int id, ResidentPathologyUpdateDto dto);
        Task DeleteAsync(int id);
    }
}
