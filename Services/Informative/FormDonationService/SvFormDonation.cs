using Entities.Informative;
using Microsoft.EntityFrameworkCore;
using Services.GenericService;
using Services.Informative.DTOS.CreatesDto;
using Services.Informative.DTOS;
using Services.MyDbContext;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Informative.FormDonationService
{
    public class SvFormDonationService : SvGenericRepository<FormDonation, MyInformativeContext>, ISvFormDonation
    {
        public SvFormDonationService(MyInformativeContext context) : base(context)
        {
        }

        // Obtener todas las donaciones con sus detalles (DonationType, MethodDonation, Status)
        public async Task<IEnumerable<FormDonationDto>> GetFormDonationsWithDetailsAsync()
        {
            return await _context.FormDonations
                .AsNoTracking() // Evitamos el tracking para mejorar rendimiento
                .Include(fd => fd.DonationType) // Incluir DonationType
                .Include(fd => fd.MethodDonation) // Incluir MethodDonation
                .Include(fd => fd.Status) // Incluir Status para evitar el error
                .Select(fd => new FormDonationDto
                {
                    Id_FormDonation = fd.Id_FormDonation,
                    Dn_Name = fd.Dn_Name,
                    Dn_Lastname1 = fd.Dn_Lastname1,
                    Dn_Lastname2 = fd.Dn_Lastname2,
                    Dn_Cedula = fd.Dn_Cedula,
                    Dn_Email = fd.Dn_Email,
                    Dn_Phone = fd.Dn_Phone,
                    Delivery_date = fd.Delivery_date,
                    DonationType = fd.DonationType.Name_DonationType, // Mapeamos solo los nombres necesarios
                    MethodDonation = fd.MethodDonation.Name_MethodDonation,
                    Status_Name = fd.Status.Status_Name // Mapeamos el nombre del estado
                })
                .ToListAsync();
        }

        // Obtener una donación por ID con detalles, incluyendo Status
        public async Task<FormDonationDto> GetFormDonationWithDetailsByIdAsync(int id)
        {
            return await _context.FormDonations
                .AsNoTracking() // Evitamos tracking para consultas de solo lectura
                .Include(fd => fd.DonationType) // Incluir DonationType
                .Include(fd => fd.MethodDonation) // Incluir MethodDonation
                .Include(fd => fd.Status) // Incluir Status para evitar el error
                .Where(fd => fd.Id_FormDonation == id)
                .Select(fd => new FormDonationDto
                {
                    Id_FormDonation = fd.Id_FormDonation,
                    Dn_Name = fd.Dn_Name,
                    Dn_Lastname1 = fd.Dn_Lastname1,
                    Dn_Lastname2 = fd.Dn_Lastname2,
                    Dn_Cedula = fd.Dn_Cedula,
                    Dn_Email = fd.Dn_Email,
                    Dn_Phone = fd.Dn_Phone,
                    Delivery_date = fd.Delivery_date,
                    DonationType = fd.DonationType.Name_DonationType,
                    MethodDonation = fd.MethodDonation.Name_MethodDonation,
                    Status_Name = fd.Status.Status_Name // Estado actual
                })
                .FirstOrDefaultAsync();
        }

        // Crear una nueva donación con el estado por defecto "Pendiente"
        public async Task CreateFormDonationAsync(FormDonationCreateDto formDonationCreateDto)
        {
            var formDonation = new FormDonation
            {
                Dn_Name = formDonationCreateDto.Dn_Name,
                Dn_Lastname1 = formDonationCreateDto.Dn_Lastname1,
                Dn_Lastname2 = formDonationCreateDto.Dn_Lastname2,
                Dn_Cedula = formDonationCreateDto.Dn_Cedula,
                Dn_Email = formDonationCreateDto.Dn_Email,
                Dn_Phone = formDonationCreateDto.Dn_Phone,
                Delivery_date = formDonationCreateDto.Delivery_date,
                Id_DonationType = formDonationCreateDto.Id_DonationType,
                Id_MethodDonation = formDonationCreateDto.Id_MethodDonation,
                Id_Status = 1 // Estado por defecto "Pendiente"
            };

            await _context.FormDonations.AddAsync(formDonation);
            await _context.SaveChangesAsync();
        }

        // Actualizar solo el estado de una donación
        public async Task UpdateFormDonationStatusAsync(int id, int statusId)
        {
            var formDonation = await _context.FormDonations.FindAsync(id);
            if (formDonation == null)
            {
                throw new KeyNotFoundException($"Formulario de donación con ID {id} no encontrado.");
            }

            // Verificar si el estado proporcionado existe
            var statusExists = await _context.Statuses.AnyAsync(s => s.Id_Status == statusId);
            if (!statusExists)
            {
                throw new ArgumentException("El estado proporcionado no existe.");
            }

            formDonation.Id_Status = statusId;
            await _context.SaveChangesAsync();
        }
    }
}
