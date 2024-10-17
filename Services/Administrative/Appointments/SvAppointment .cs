using AutoMapper;
using Entities.Administration;
using Microsoft.EntityFrameworkCore;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Services.GenericService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Administrative.AppointmentService
{
    public class SvAppointment : ISvAppointment
    {
        private readonly ISvGenericRepository<Appointment> _appointmentRepository;
        private readonly ISvGenericRepository<Resident> _residentRepository;
        private readonly ISvGenericRepository<Employee> _employeeRepository;
        private readonly ISvGenericRepository<AppointmentStatus> _appointmentStatusRepository; // Agregado
        private readonly IMapper _mapper;

        public SvAppointment(
            ISvGenericRepository<Appointment> appointmentRepository,
            ISvGenericRepository<Resident> residentRepository,
            ISvGenericRepository<Employee> employeeRepository,
            ISvGenericRepository<AppointmentStatus> appointmentStatusRepository, // Agregado
            IMapper mapper)
        {
            _appointmentRepository = appointmentRepository;
            _residentRepository = residentRepository;
            _employeeRepository = employeeRepository;
            _appointmentStatusRepository = appointmentStatusRepository;  // Asignación
            _mapper = mapper;
        }

        // Crear cita
        public async Task CreateAppointmentAsync(AppointmentPostDto appointmentDto)
        {
            // Validar si el residente existe
            var resident = await _residentRepository.GetByIdAsync(appointmentDto.Id_Resident);
            if (resident == null)
                throw new KeyNotFoundException($"No se encontró el residente con ID {appointmentDto.Id_Resident}");

            // Validar si el acompañante existe
            var companion = await _employeeRepository.GetByIdAsync(appointmentDto.Id_Companion);
            if (companion == null)
                throw new KeyNotFoundException($"No se encontró el acompañante con DNI {appointmentDto.Id_Companion}");

            // Validar que no exista una cita duplicada para la misma fecha y hora
            bool appointmentExists = await _appointmentRepository
                .Query()
                .AnyAsync(a => a.Id_Resident == appointmentDto.Id_Resident &&
                               a.Date == appointmentDto.Date &&
                               a.Time == appointmentDto.Time);

            if (appointmentExists)
                throw new InvalidOperationException("El residente ya tiene una cita en la misma fecha y hora.");

            // Buscar el estado "Pendiente" en la base de datos
            var pendingStatus = await _appointmentStatusRepository
                .Query()
                .FirstOrDefaultAsync(s => s.Name_StatusAP == "Pendiente");

            if (pendingStatus == null)
                throw new InvalidOperationException("No se encontró el estado 'Pendiente' en la base de datos.");

            // Crear la cita y asignar el estado "Pendiente"
            var appointment = _mapper.Map<Appointment>(appointmentDto);
            appointment.Id_StatusAP = pendingStatus.Id_StatusAP;  // Asignar el estado "Pendiente"

            // Guardar la cita en la base de datos
            await _appointmentRepository.AddAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();
        }


        // Obtener cita por ID
        public async Task<AppointmentGetDto> GetAppointmentByIdAsync(int id)
        {
            var appointment = await _appointmentRepository
                .Query()
                .Include(a => a.Resident)
                .Include(a => a.Companion)
                .Include(a => a.Specialty)
                .Include(a => a.HealthcareCenter)
                .Include(a => a.AppointmentStatus)
                .FirstOrDefaultAsync(a => a.Id_Appointment == id);

            if (appointment == null)
                throw new KeyNotFoundException($"No se encontró la cita con ID {id}");

            return _mapper.Map<AppointmentGetDto>(appointment);
        }

        // Obtener todas las citas
        public async Task<IEnumerable<AppointmentGetDto>> GetAllAppointmentsAsync()
        {
            var appointments = await _appointmentRepository
                .Query()
                .Include(a => a.Resident)
                .Include(a => a.Companion)
                .Include(a => a.Specialty)
                .Include(a => a.HealthcareCenter)
                .Include(a => a.AppointmentStatus)
                .ToListAsync();

            return _mapper.Map<IEnumerable<AppointmentGetDto>>(appointments);
        }

        // Actualizar cita
        public async Task UpdateAppointmentAsync(AppointmentUpdateDto appointmentDto)
        {
            var existingAppointment = await _appointmentRepository.GetByIdAsync(appointmentDto.Id_Appointment);
            if (existingAppointment == null)
                throw new KeyNotFoundException($"No se encontró la cita con ID {appointmentDto.Id_Appointment}");

            // Validar conflicto de horario en la actualización
            if (appointmentDto.Date.HasValue && appointmentDto.Time.HasValue)
            {
                bool appointmentExists = await _appointmentRepository
                    .Query()
                    .AnyAsync(a => a.Id_Resident == existingAppointment.Id_Resident &&
                                   a.Date == appointmentDto.Date.Value &&
                                   a.Time == appointmentDto.Time.Value &&
                                   a.Id_Appointment != appointmentDto.Id_Appointment);  // Excluir la cita actual

                if (appointmentExists)
                    throw new InvalidOperationException("El residente ya tiene otra cita en la misma fecha y hora.");
            }

            // Actualizar los campos
            _mapper.Map(appointmentDto, existingAppointment);
            await _appointmentRepository.SaveChangesAsync();
        }


        public async Task PatchAppointmentAsync(int id, AppointmentUpdateDto appointmentDto)
        {
            var existingAppointment = await _appointmentRepository.GetByIdAsync(id);
            if (existingAppointment == null)
                throw new KeyNotFoundException($"No se encontró la cita con ID {id}");

            // Actualizar solo los campos proporcionados en el DTO
            if (appointmentDto.Date.HasValue)
                existingAppointment.Date = appointmentDto.Date.Value;

            if (appointmentDto.Time.HasValue)
                existingAppointment.Time = appointmentDto.Time.Value;

            if (appointmentDto.Id_Companion.HasValue)
            {
                var companion = await _employeeRepository.GetByIdAsync(appointmentDto.Id_Companion.Value);
                if (companion == null)
                    throw new KeyNotFoundException($"No se encontró el acompañante con DNI {appointmentDto.Id_Companion}");

                existingAppointment.Id_Companion = appointmentDto.Id_Companion.Value;
            }

            if (appointmentDto.Id_StatusAP.HasValue)
                existingAppointment.Id_StatusAP = appointmentDto.Id_StatusAP.Value;

            if (appointmentDto.Notes != null)
                existingAppointment.Notes = appointmentDto.Notes;

            // Guardar los cambios
            await _appointmentRepository.SaveChangesAsync();
        }

        // Eliminar cita
        public async Task DeleteAppointmentAsync(int id)
        {
            var existingAppointment = await _appointmentRepository.GetByIdAsync(id);
            if (existingAppointment == null)
                throw new KeyNotFoundException($"No se encontró la cita con ID {id}");

            await _appointmentRepository.DeleteAsync(id);
            await _appointmentRepository.SaveChangesAsync();
        }
    }
}
