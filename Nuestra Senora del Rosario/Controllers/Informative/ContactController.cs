using Entities.Informative;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly ISvGenericRepository<Contact> _contactService;

    public ContactController(ISvGenericRepository<Contact> contactService)
    {
        _contactService = contactService;
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

    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchContact(int id, [FromBody] JsonPatchDocument<Contact> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        await _contactService.PatchAsync(id, patchDoc);
        await _contactService.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContact(int id)
    {
        await _contactService.DeleteAsync(id);
        await _contactService.SaveChangesAsync();
        return NoContent();
    }
}
