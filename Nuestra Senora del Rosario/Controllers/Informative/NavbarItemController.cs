using AutoMapper;
using Entities.Informative;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.Informative.DTOS;
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

    // GET: api/NavbarItem
    [HttpGet]
    public async Task<IActionResult> GetNavbarItems()
    {
        var navbarItems = await _navbarItemService.GetAllWithChildrenAsync();
        var navbarItemsDto = _mapper.Map<IEnumerable<NavbarItemDto>>(navbarItems);
        return Ok(navbarItemsDto);
    }

    // GET: api/NavbarItem/{id}
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

    // POST: api/NavbarItem
    [HttpPost]
    public async Task<IActionResult> AddNavbarItem(NavbarItemDto navbarItemDto)
    {
        // Mapeamos el DTO a la entidad
        var navbarItem = _mapper.Map<NavbarItem>(navbarItemDto);

        await _navbarItemService.AddAsync(navbarItem);
        await _navbarItemService.SaveChangesAsync();

        var createdNavbarItemDto = _mapper.Map<NavbarItemDto>(navbarItem);
        return CreatedAtAction(nameof(GetNavbarItem), new { id = createdNavbarItemDto.Id_Nav_It }, createdNavbarItemDto);
    }

    // PATCH: api/NavbarItem/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchNavbarItem(int id, [FromBody] JsonPatchDocument<NavbarItemDto> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        var navbarItem = await _navbarItemService.GetByIdAsync(id);
        if (navbarItem == null)
        {
            return NotFound();
        }

        // Aplica el parche al DTO y luego mapea de nuevo a la entidad
        var navbarItemDto = _mapper.Map<NavbarItemDto>(navbarItem);
        patchDoc.ApplyTo(navbarItemDto, ModelState);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _mapper.Map(navbarItemDto, navbarItem);

        await _navbarItemService.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/NavbarItem/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNavbarItem(int id)
    {
        await _navbarItemService.DeleteAsync(id);
        await _navbarItemService.SaveChangesAsync();
        return NoContent();
    }
}
