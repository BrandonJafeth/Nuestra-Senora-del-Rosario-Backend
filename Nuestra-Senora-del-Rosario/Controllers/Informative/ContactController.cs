using AutoMapper;
using Domain.Entities.Informative;
using Infrastructure.Services.Informative.DTOS.CreatesDto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly ISvGenericRepository<Contact> _contactService;
    private readonly IMapper _mapper;

    public ContactController(ISvGenericRepository<Contact> contactService, IMapper mapper)
    {
        _contactService = contactService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetContacts()
    {
        var items = await _contactService.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetContact(int id)
    {
        var item = await _contactService.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> AddContact(Contact contact)
    {
        await _contactService.AddAsync(contact);
        await _contactService.SaveChangesAsync();
        return CreatedAtAction(nameof(GetContact), new { id = contact.Id_Contact }, contact);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateContact(int id, [FromBody] ContactUpdateDTO updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingSection = await _contactService.GetByIdAsync(id);
        if (existingSection == null)
        {
            return NotFound($"Contact con ID {id} no fue encontrada.");
        }


        _mapper.Map(updateDto, existingSection);
        await _contactService.SaveChangesAsync();

        return Ok(existingSection);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContact(int id)
    {
        await _contactService.DeleteAsync(id);
        await _contactService.SaveChangesAsync();
        return NoContent();
    }
}
