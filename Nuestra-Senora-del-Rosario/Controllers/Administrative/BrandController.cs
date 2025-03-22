using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Entities.Administration;              // Aquí está la entidad Brand
using Services.GenericService;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Microsoft.AspNetCore.Authorization;                    // Servicio genérico
// using [TuNamespace].BrandDTO;                  // Ajusta el namespace de tus DTOs

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]  // <-- Aquí especificas el rol
public class BrandController : ControllerBase
{
    private readonly ISvGenericRepository<Brand> _brandService;
    private readonly IMapper _mapper;

    public BrandController(ISvGenericRepository<Brand> brandService, IMapper mapper)
    {
        _brandService = brandService;
        _mapper = mapper;
    }

    // GET: api/brand
    [HttpGet]
    public async Task<IActionResult> GetAllBrands([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var (brands, totalRecords) = await _brandService.GetPagedAsync(
            pageNumber: pageNumber,
            pageSize: pageSize,
            orderBy: q => q.OrderBy(b => b.BrandName)
        );

        var brandDtos = _mapper.Map<IEnumerable<BrandReadDto>>(brands);

        var response = new
        {
            Data = brandDtos,
            TotalRecords = totalRecords,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize)
        };

        return Ok(response);
    }


    // GET: api/brand/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBrandById(int id)
    {
        var brand = await _brandService.GetByIdAsync(id);
        if (brand == null)
        {
            return NotFound($"Brand with ID {id} not found.");
        }

        var brandDto = _mapper.Map<BrandReadDto>(brand);
        return Ok(brandDto);
    }

    // POST: api/brand
    [HttpPost]
    public async Task<IActionResult> CreateBrand([FromBody] BrandCreateDto brandCreateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var brand = _mapper.Map<Brand>(brandCreateDto);
        await _brandService.AddAsync(brand);
        await _brandService.SaveChangesAsync();

        var brandDto = _mapper.Map<BrandReadDto>(brand);
        return CreatedAtAction(nameof(GetBrandById), new { id = brand.IdBrand }, brandDto);
    }

    // PUT: api/brand/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBrand(int id, [FromBody] BrandCreateDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingBrand = await _brandService.GetByIdAsync(id);
        if (existingBrand == null)
        {
            return NotFound($"Brand with ID {id} not found.");
        }

        _mapper.Map(updateDto, existingBrand);
        await _brandService.SaveChangesAsync();

        return Ok(existingBrand);
    }

    // DELETE: api/brand/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBrand(int id)
    {
        var brand = await _brandService.GetByIdAsync(id);
        if (brand == null)
        {
            return NotFound($"Brand with ID {id} not found.");
        }

        await _brandService.DeleteAsync(id);
        await _brandService.SaveChangesAsync();
        return NoContent();
    }
}
