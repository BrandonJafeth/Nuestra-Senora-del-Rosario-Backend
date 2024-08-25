
using Entities.Informative;
using Microsoft.AspNetCore.Mvc;
using Services.Informative.GenericRepository;

[ApiController]
[Route("api/[controller]")]
public class NavbarItemController : ControllerBase
{
    private readonly ISvGenericRepository<NavbarItem> _navbarItemService;

    public NavbarItemController(ISvGenericRepository<NavbarItem> navbarItemService)
    {
        _navbarItemService = navbarItemService;
    }

    [HttpGet]
    public async Task<IActionResult> GetNavbarItems()
    {
        var items = await _navbarItemService.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetNavbarItem(int id)
    {
        var item = await _navbarItemService.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> AddNavbarItem(NavbarItem navbarItem)
    {
        await _navbarItemService.AddAsync(navbarItem);
        await _navbarItemService.SaveChangesAsync();
        return CreatedAtAction(nameof(GetNavbarItem), new { id = navbarItem.Id_Nav_It }, navbarItem);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNavbarItem(int id, NavbarItem navbarItem)
    {
        if (id != navbarItem.Id_Nav_It)
        {
            return BadRequest();
        }

        await _navbarItemService.UpdateAsync(navbarItem);
        await _navbarItemService.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNavbarItem(int id)
    {
        await _navbarItemService.DeleteAsync(id);
        await _navbarItemService.SaveChangesAsync();
        return NoContent();
    }
}
