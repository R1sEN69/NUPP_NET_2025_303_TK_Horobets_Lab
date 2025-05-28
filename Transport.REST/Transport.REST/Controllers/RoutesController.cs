using Microsoft.AspNetCore.Mvc;
using Transport.REST.Interfaces;
using Transport.REST.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Transport.REST.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // api/routes
    public class RoutesController : ControllerBase
    {
        private readonly ICrudServiceAsync<RouteModel> _routeService;

        public RoutesController(ICrudServiceAsync<RouteModel> routeService)
        {
            _routeService = routeService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RouteModel>>> GetRoutes()
        {
            var routes = await _routeService.ReadAllAsync();
            return Ok(routes);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RouteModel>> GetRoute(Guid id)
        {
            var route = await _routeService.ReadAsync(id);
            if (route == null)
            {
                return NotFound();
            }
            return Ok(route);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RouteModel>> PostRoute([FromBody] RouteCreateModel routeCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var routeModel = new RouteModel
            {
                Name = routeCreateDto.Name,
                StartPoint = routeCreateDto.StartPoint,
                EndPoint = routeCreateDto.EndPoint,
                DistanceKm = routeCreateDto.DistanceKm
            };

            var success = await _routeService.CreateAsync(routeModel);
            if (!success)
            {
                return BadRequest("Could not create route.");
            }
            await _routeService.SaveAsync();

            return CreatedAtAction(nameof(GetRoute), new { id = routeModel.Id }, routeModel);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutRoute(Guid id, [FromBody] RouteUpdateModel routeUpdateDto)
        {
            if (id != routeUpdateDto.Id)
            {
                return BadRequest("ID in URL does not match ID in body.");
            }

            var existingRoute = await _routeService.ReadAsync(id);
            if (existingRoute == null)
            {
                return NotFound();
            }

            if (routeUpdateDto.Name != null) existingRoute.Name = routeUpdateDto.Name;
            if (routeUpdateDto.StartPoint != null) existingRoute.StartPoint = routeUpdateDto.StartPoint;
            if (routeUpdateDto.EndPoint != null) existingRoute.EndPoint = routeUpdateDto.EndPoint;
            if (routeUpdateDto.DistanceKm.HasValue) existingRoute.DistanceKm = routeUpdateDto.DistanceKm.Value;

            var success = await _routeService.UpdateAsync(existingRoute);
            if (!success)
            {
                return BadRequest("Could not update route.");
            }
            await _routeService.SaveAsync();

            return Ok(existingRoute);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRoute(Guid id)
        {
            var route = await _routeService.ReadAsync(id);
            if (route == null)
            {
                return NotFound();
            }

            var success = await _routeService.RemoveAsync(route);
            if (!success)
            {
                return BadRequest("Could not delete route.");
            }
            await _routeService.SaveAsync();

            return NoContent();
        }
    }
}