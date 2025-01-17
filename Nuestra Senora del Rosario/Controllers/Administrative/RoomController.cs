using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Entities.Administration;

[ApiController]
[Route("api/[controller]")]
public class RoomController : ControllerBase
{
    private readonly ISvGenericRepository<Room> _roomService;

    public RoomController(ISvGenericRepository<Room> roomService)
    {
        _roomService = roomService;
    }

    // GET: api/room
    [HttpGet]
    public async Task<IActionResult> GetAllRooms()
    {
        var rooms = await _roomService.GetAllAsync();
        return Ok(rooms);
    }

    // GET: api/room/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoomById(int id)
    {
        var room = await _roomService.GetByIdAsync(id);
        if (room == null)
        {
            return NotFound($"Room with ID {id} not found.");
        }
        return Ok(room);
    }

    // POST: api/room
    [HttpPost]
    public async Task<IActionResult> CreateRoom([FromBody] Room room)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _roomService.AddAsync(room);
        await _roomService.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRoomById), new { id = room.Id_Room }, room);
    }

    // PATCH: api/room/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchRoom(int id, [FromBody] JsonPatchDocument<Room> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest("Invalid patch document.");
        }

        try
        {
            await _roomService.PatchAsync(id, patchDoc);
            await _roomService.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
        }
    }

    // DELETE: api/room/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRoom(int id)
    {
        await _roomService.DeleteAsync(id);
        await _roomService.SaveChangesAsync();
        return NoContent();
    }
}
