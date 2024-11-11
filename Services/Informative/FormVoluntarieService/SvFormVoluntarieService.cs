using Entities.Informative;
using Microsoft.EntityFrameworkCore;
using Services.GenericService;
using Services.Informative.DTOS.CreatesDto;
using Services.Informative.DTOS;
using Services.MyDbContext;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Informative.FormVoluntarieServices
{
    public class SvFormVoluntarieService : SvGenericRepository<FormVoluntarie, MyInformativeContext>, ISvFormVoluntarieService
    {
        public SvFormVoluntarieService(MyInformativeContext context) : base(context)
        {
        }

        // Método para obtener todas las formas de voluntarios con su tipo y estado, optimizado con AsNoTracking
        public async Task<(IEnumerable<FormVoluntarieDto> FormVoluntaries, int TotalPages)> GetAllFormVoluntariesWithTypeAsync(int pageNumber, int pageSize)
        {
            var totalFormVoluntaries = await _context.FormVoluntaries.CountAsync();

            var formVoluntaries = await _context.FormVoluntaries
                .AsNoTracking()
                .Include(f => f.VoluntarieType)
                .Include(f => f.Status)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(f => new FormVoluntarieDto
                {
                    Id_FormVoluntarie = f.Id_FormVoluntarie,
                    Vn_Name = f.Vn_Name,
                    Vn_Lastname1 = f.Vn_Lastname1,
                    Vn_Lastname2 = f.Vn_Lastname2,
                    Vn_Cedula = f.Vn_Cedula,
                    Vn_Phone = f.Vn_Phone,
                    Vn_Email = f.Vn_Email,
                    Delivery_Date = f.Delivery_Date,
                    End_Date = f.End_Date,
                    Name_voluntarieType = f.VoluntarieType.Name_VoluntarieType,
                    Status_Name = f.Status.Status_Name
                })
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalFormVoluntaries / (double)pageSize);
            return (formVoluntaries, totalPages);
        }


        // Método para obtener un formulario de voluntario con su tipo y estado por ID
        public async Task<FormVoluntarieDto> GetFormVoluntarieWithTypeByIdAsync(int id)
        {
            return await _context.FormVoluntaries
                .AsNoTracking()
                .Include(f => f.VoluntarieType)  // Incluye la relación de tipo de voluntariado
                .Include(f => f.Status)  // Incluye la relación de estado
                .Where(f => f.Id_FormVoluntarie == id)
                .Select(f => new FormVoluntarieDto
                {
                    Id_FormVoluntarie = f.Id_FormVoluntarie,
                    Vn_Name = f.Vn_Name,
                    Vn_Lastname1 = f.Vn_Lastname1,
                    Vn_Lastname2 = f.Vn_Lastname2,
                    Vn_Cedula = f.Vn_Cedula,
                    Vn_Phone = f.Vn_Phone,
                    Vn_Email = f.Vn_Email,
                    Delivery_Date = f.Delivery_Date,
                    End_Date = f.End_Date,
                    Name_voluntarieType = f.VoluntarieType.Name_VoluntarieType,
                    Status_Name = f.Status.Status_Name
                })
                .FirstOrDefaultAsync();
        }

        public async Task CreateFormVoluntarieAsync(FormVoluntarieCreateDto formVoluntarieCreateDto)
        {
            // Verificar si el tipo de voluntariado existe
            var voluntarieTypeExists = await _context.VoluntarieTypes
                .AnyAsync(v => v.Id_VoluntarieType == formVoluntarieCreateDto.VoluntarieTypeId);

            if (!voluntarieTypeExists)
            {
                throw new ArgumentException("El tipo de voluntariado proporcionado no existe.");
            }

            // Crear el formulario de voluntariado
            var formVoluntarie = new FormVoluntarie
            {
                Vn_Name = formVoluntarieCreateDto.Vn_Name,
                Vn_Lastname1 = formVoluntarieCreateDto.Vn_Lastname1,
                Vn_Lastname2 = formVoluntarieCreateDto.Vn_Lastname2,
                Vn_Cedula = formVoluntarieCreateDto.Vn_Cedula,
                Vn_Phone = formVoluntarieCreateDto.Vn_Phone,
                Vn_Email = formVoluntarieCreateDto.Vn_Email,
                Delivery_Date = formVoluntarieCreateDto.Delivery_Date,
                End_Date = formVoluntarieCreateDto.End_Date,
                Id_VoluntarieType = formVoluntarieCreateDto.VoluntarieTypeId,
                 Id_Status = 1  
            };

            await _context.FormVoluntaries.AddAsync(formVoluntarie);
            await _context.SaveChangesAsync();
        }

        // Método para actualizar solo el estado de un formulario
        public async Task UpdateFormVoluntarieStatusAsync(int id, int statusId)
        {
            var formVoluntarie = await _context.FormVoluntaries.FindAsync(id);
            if (formVoluntarie == null)
            {
                throw new KeyNotFoundException($"Formulario de voluntario con ID {id} no encontrado.");
            }

            // Verificar si el estado proporcionado existe
            var statusExists = await _context.Statuses.AnyAsync(s => s.Id_Status == statusId);
            if (!statusExists)
            {
                throw new ArgumentException("El estado proporcionado no existe.");
            }

            formVoluntarie.Id_Status = statusId;
            await _context.SaveChangesAsync();
        }


        // Método para eliminar una forma de voluntariado
        public async Task DeleteFormVoluntarieAsync(int id)
        {
            var formVoluntarie = await _context.FormVoluntaries.FindAsync(id);

            if (formVoluntarie == null)
            {
                throw new KeyNotFoundException($"Formulario de voluntario con ID {id} no encontrado.");
            }

            _context.FormVoluntaries.Remove(formVoluntarie);
            await _context.SaveChangesAsync();
        }
    }
}
