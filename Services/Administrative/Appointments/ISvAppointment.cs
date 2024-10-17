using Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Administrative.AppointmentService
{
    public interface ISvAppointment
    {
        Task CreateAppointmentAsync(AppointmentPostDto appointmentDto);
        Task UpdateAppointmentAsync(AppointmentUpdateDto appointmentDto);
        Task PatchAppointmentAsync(int id, AppointmentUpdateDto appointmentDto);
        Task<AppointmentGetDto> GetAppointmentByIdAsync(int id);
        Task<IEnumerable<AppointmentGetDto>> GetAllAppointmentsAsync();
        Task DeleteAppointmentAsync(int id);
    }
}
