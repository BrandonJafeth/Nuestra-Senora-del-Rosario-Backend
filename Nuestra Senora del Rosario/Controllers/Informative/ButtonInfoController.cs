using Entities.Informative;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.Informative.GenericRepository;

[ApiController]
[Route("api/[controller]")]
public class ButtonInfoController : ControllerBase
{
    private readonly ISvGenericRepository<ButtonInfo> _buttonInfoService;

    public ButtonInfoController(ISvGenericRepository<ButtonInfo> buttonInfoService)
    {
        _buttonInfoService = buttonInfoService;
    }

    [HttpGet]
    public async Task<IActionResult> GetButtonInfos()
    {
        var items = await _buttonInfoService.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetButtonInfo(int id)
    {
        var item = await _buttonInfoService.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> AddButtonInfo(ButtonInfo buttonInfo)
    {
        await _buttonInfoService.AddAsync(buttonInfo);
        await _buttonInfoService.SaveChangesAsync();
        return CreatedAtAction(nameof(GetButtonInfo), new { id = buttonInfo.Id_ButtonInfo }, buttonInfo);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchButtonInfo(int id, [FromBody] JsonPatchDocument<ButtonInfo> patchDoc)
    {
        if (patchDoc == null)
        {
            return BadRequest();
        }

        await _buttonInfoService.PatchAsync(id, patchDoc);
        await _buttonInfoService.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteButtonInfo(int id)
    {
        await _buttonInfoService.DeleteAsync(id);
        await _buttonInfoService.SaveChangesAsync();
        return NoContent();
    }
}
