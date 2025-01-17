using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Services.Administrative.Appointments
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
