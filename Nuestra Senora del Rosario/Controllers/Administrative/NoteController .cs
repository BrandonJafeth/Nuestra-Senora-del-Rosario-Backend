using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Entities.Administration;

[ApiController]
[Route("api/[controller]")]
public class NoteController : ControllerBase
{
    private readonly ISvGenericRepository<Note> _noteService;

    public NoteController(ISvGenericRepository<Note> noteService)
    {
        _noteService = noteService;
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

    // PATCH: api/note/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchNote(int id, [FromBody] JsonPatchDocument<Note> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest("Invalid patch document.");
        }

        try
        {
            await _noteService.PatchAsync(id, patchDoc);
            await _noteService.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
        }
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
