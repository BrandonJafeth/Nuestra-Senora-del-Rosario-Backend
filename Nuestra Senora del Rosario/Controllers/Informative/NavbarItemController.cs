
using AutoMapper;
using Entities.Informative;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.DTOS;
using Services.Informative.GenericRepository;
using Services.Informative.NavbarItemServices;

[ApiController]
[Route("api/[controller]")]
public class NavbarItemController : ControllerBase
{
    private readonly ISvNavbarItemService _navbarItemService;
    private readonly IMapper _mapper;

    public NavbarItemController(ISvNavbarItemService navbarItemService, IMapper mapper)
    {
        _navbarItemService = navbarItemService;
        _mapper = mapper;
    }


    [HttpGet]
    public async Task<IActionResult> GetNavbarItems()
    {
       
        var navbarItems = await _navbarItemService.GetAllWithChildrenAsync();
        var navbarItemsDto = _mapper.Map<IEnumerable<NavbarItemDto>>(navbarItems);
        return Ok(navbarItemsDto);
    }

   
    [HttpGet("{id}")]
    public async Task<IActionResult> GetNavbarItem(int id)
    {
        var navbarItem = await _navbarItemService.GetNavbarItemWithChildrenAsync(id);
        if (navbarItem == null)
        {
            return NotFound();
        }

        var navbarItemDto = _mapper.Map<NavbarItemDto>(navbarItem);
        return Ok(navbarItemDto);
    }



    [HttpPost]
    public async Task<IActionResult> AddNavbarItem(NavbarItem navbarItem)
    {
        await _navbarItemService.AddAsync(navbarItem);
        await _navbarItemService.SaveChangesAsync();
        return CreatedAtAction(nameof(GetNavbarItem), new { id = navbarItem.Id_Nav_It }, navbarItem);
    }


    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchNavbarItem(int id, [FromBody] JsonPatchDocument<NavbarItem> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        await _navbarItemService.PatchAsync(id, patchDoc);
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
