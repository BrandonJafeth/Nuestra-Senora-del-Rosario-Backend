using AutoMapper;
using Domain.Entities.Administration;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOCreate;
using Infrastructure.Services.Administrative.AdministrativeDTO.AdministrativeDTOGet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.GenericService;

namespace Nuestra_Senora_del_Rosario.Controllers.Administrative
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Enfermeria")]  // <-- Aquí especificas el rol
    public class AdministrationRouteController : ControllerBase
    {
        private readonly ISvGenericRepository<AdministrationRoute> _administrationRouteService;
        private readonly IMapper _mapper;

        public AdministrationRouteController(ISvGenericRepository<AdministrationRoute> administrationRouteService, IMapper mapper)
        {
            _administrationRouteService = administrationRouteService;
            _mapper = mapper;
        }

        // GET: api/AdministrationRoute
        [HttpGet]
        public async Task<IActionResult> GetAllAdministrationRoutes()
        {
            var routes = await _administrationRouteService.GetAllAsync();
            var routesDto = _mapper.Map<IEnumerable<AdministrationRouteGetDto>>(routes);
            return Ok(routesDto);
        }

        // GET: api/AdministrationRoute/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdministrationRouteById(int id)
        {
            var route = await _administrationRouteService.GetByIdAsync(id);
            if (route == null)
            {
                return NotFound($"AdministrationRoute with ID {id} not found.");
            }
            var routeDto = _mapper.Map<AdministrationRouteGetDto>(route);
            return Ok(routeDto);
        }

        // POST: api/AdministrationRoute
        [HttpPost]
        public async Task<IActionResult> CreateAdministrationRoute([FromBody] AdministrationRouteCreateDto createRouteDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var route = _mapper.Map<AdministrationRoute>(createRouteDto);
            await _administrationRouteService.AddAsync(route);
            await _administrationRouteService.SaveChangesAsync();

            var routeDto = _mapper.Map<AdministrationRouteGetDto>(route);
            return CreatedAtAction(nameof(GetAdministrationRouteById), new { id = routeDto.Id_AdministrationRoute }, routeDto);
        }

        // PUT: api/AdministrationRoute/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdministrationRoute(int id, [FromBody] AdministrationRouteUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var route = await _administrationRouteService.GetByIdAsync(id);
            if (route == null)
            {
                return NotFound($"AdministrationRoute with ID {id} not found.");
            }

 
            _mapper.Map(updateDto, route);

            await _administrationRouteService.SaveChangesAsync();

            return NoContent();
        }


        // DELETE: api/AdministrationRoute/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdministrationRoute(int id)
        {
            await _administrationRouteService.DeleteAsync(id);
            await _administrationRouteService.SaveChangesAsync();
            return NoContent();
        }
    }
}
