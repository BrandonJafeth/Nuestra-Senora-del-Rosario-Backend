using AutoMapper;
using Domain.Entities.Informative;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Informative.DTOS.CreatesDto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;

[ApiController]
[Route("api/[controller]")]
public class RegistrationSectionController : ControllerBase
{
    private readonly ISvGenericRepository<RegistrationSection> _registrationSectionService;
    private readonly IMapper _mapper;

    public RegistrationSectionController(ISvGenericRepository<RegistrationSection> registrationSectionService, IMapper mapper)
    {
        _registrationSectionService = registrationSectionService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetRegistrationSections()
    {
        var items = await _registrationSectionService.GetAllAsync();
        return Ok(items);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRegistrationSection(int id, [FromBody] RegistrationSectionUpdateDTO updateDto)
    {
        // Verificar que el DTO sea válido
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingSection = await _registrationSectionService.GetByIdAsync(id);
        if (existingSection == null)
        {
            return NotFound($"RegistrationSection  con ID {id} no fue encontrada.");
        }


        _mapper.Map(updateDto, existingSection);
        await _registrationSectionService.SaveChangesAsync();

        return Ok(existingSection);
    }



}
