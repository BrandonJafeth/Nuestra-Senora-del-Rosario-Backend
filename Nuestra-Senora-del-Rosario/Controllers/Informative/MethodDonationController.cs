﻿using AutoMapper;
using Domain.Entities.Informative;
using Infrastructure.Services.Informative.DTOS;
using Infrastructure.Services.Informative.DTOS.CreatesDto;
using Infrastructure.Services.Informative.MethodDonationService;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class MethodDonationController : ControllerBase
{
    private readonly ISvMethodDonation _methodDonationService;
    private readonly IMapper _mapper;

    public MethodDonationController(ISvMethodDonation methodDonationService, IMapper mapper)
    {
        _methodDonationService = methodDonationService;
        _mapper = mapper;
    }

    // GET: api/MethodDonation
    [HttpGet]
    public async Task<IActionResult> GetMethodDonations()
    {
        var methodDonations = await _methodDonationService.GetMethodDonationsWithTypesAsync();
        var methodDonationDtos = _mapper.Map<IEnumerable<MethodDonationDto>>(methodDonations);
        return Ok(methodDonationDtos);
    }

    // GET: api/MethodDonation/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMethodDonation(int id)
    {
        var methodDonation = await _methodDonationService.GetByIdAsync(id);
        if (methodDonation == null)
        {
            return NotFound();
        }

        var methodDonationDto = _mapper.Map<MethodDonationDto>(methodDonation);
        return Ok(methodDonationDto);
    }

    // POST: api/MethodDonation
    [HttpPost]
    public async Task<IActionResult> AddMethodDonation(MethodDonationDto methodDonationDto)
    {
        var methodDonation = _mapper.Map<MethodDonation>(methodDonationDto);
        await _methodDonationService.AddAsync(methodDonation);
        await _methodDonationService.SaveChangesAsync();
        return CreatedAtAction(nameof(GetMethodDonation), new { id = methodDonation.Id_MethodDonation }, methodDonationDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMethodDonation(int id, [FromBody] MethodDonationUpdateDTO updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingSection = await _methodDonationService.GetByIdAsync(id);
        if (existingSection == null)
        {
            return NotFound($"MethodDonation con ID {id} no fue encontrada.");
        }

        _mapper.Map(updateDto, existingSection);
        await _methodDonationService.SaveChangesAsync();
        return Ok(existingSection);
    }

    // DELETE: api/MethodDonation/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMethodDonation(int id)
    {
        await _methodDonationService.DeleteAsync(id);
        await _methodDonationService.SaveChangesAsync();
        return NoContent();
    }
}
