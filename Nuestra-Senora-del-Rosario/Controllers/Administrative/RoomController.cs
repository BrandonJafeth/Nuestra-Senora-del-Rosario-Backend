using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Entities.Administration;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Informative.DTOS.CreatesDto;
using AutoMapper;

[ApiController]
[Route("api/[controller]")]
public class RoomController : ControllerBase
{
    private readonly ISvGenericRepository<Room> _roomService;
    private readonly IMapper _mapper;

    public RoomController(ISvGenericRepository<Room> roomService, IMapper mapper)
    {
        _roomService = roomService;
        _mapper = mapper;
    }

    // GET: api/room
    [HttpGet]
    public async Task<IActionResult> GetAllRooms([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var (rooms, totalRecords) = await _roomService.GetPagedAsync(
            pageNumber: pageNumber,
            pageSize: pageSize,
            orderBy: q => q.OrderBy(r => r.RoomNumber)
        );

        var response = new
        {
            Data = rooms,
            TotalRecords = totalRecords,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
        };

        return Ok(response);
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

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRoom(int id, [FromBody] RoomUpdateDTO updateDto)
    {
        // Verificar que el DTO sea válido
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingSection = await _roomService.GetByIdAsync(id);
        if (existingSection == null)
        {
            return NotFound($"Room con ID {id} no fue encontrada.");
        }


        _mapper.Map(updateDto, existingSection);
        await _roomService.SaveChangesAsync();

        return Ok(existingSection);
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
