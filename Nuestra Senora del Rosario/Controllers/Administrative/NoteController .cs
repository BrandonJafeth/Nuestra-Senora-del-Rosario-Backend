using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Entities.Administration;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using AutoMapper;
using Infrastructure.Services.Informative.DTOS.CreatesDto;

[ApiController]
[Route("api/[controller]")]
public class NoteController : ControllerBase
{
    private readonly ISvGenericRepository<Note> _noteService;
    private readonly IMapper _mapper;

    public NoteController(ISvGenericRepository<Note> noteService, IMapper mapper)
    {
        _noteService = noteService;
        _mapper = mapper;
    }

    // GET: api/note
    [HttpGet]
    public async Task<IActionResult> GetAllNotes()
    {
        var notes = await _noteService.GetAllAsync();
        return Ok(notes);
    }

    // GET: api/note/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetNoteById(int id)
    {
        var note = await _noteService.GetByIdAsync(id);
        if (note == null)
        {
            return NotFound($"Note with ID {id} not found.");
        }
        return Ok(note);
    }

    // POST: api/note
    [HttpPost]
    public async Task<IActionResult> CreateNote([FromBody] Note note)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Validar si no se pasa manualmente una fecha de creación
        if (note.CreatedAt == default)
        {
            note.CreatedAt = DateTime.UtcNow; // Asignar la fecha actual si no se pasa en el body
        }

        await _noteService.AddAsync(note);
        await _noteService.SaveChangesAsync();

        return CreatedAtAction(nameof(GetNoteById), new { id = note.Id_Note }, note);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNotes(int id, [FromBody] NoteUpdateDTO updateDto)
    {
        // Verificar que el DTO sea válido
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingSection = await _noteService.GetByIdAsync(id);
        if (existingSection == null)
        {
            return NotFound($"Note con ID {id} no fue encontrada.");
        }


        _mapper.Map(updateDto, existingSection);
        await _noteService.SaveChangesAsync();

        return Ok(existingSection);
    }


    // DELETE: api/note/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNote(int id)
    {
        await _noteService.DeleteAsync(id);
        await _noteService.SaveChangesAsync();
        return NoContent();
    }
}
