using Microsoft.AspNetCore.Mvc;
using Transport.REST.Interfaces;
using Transport.REST.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;


namespace Transport.REST.Controllers
{
    [ApiController] 
    [Route("api/[controller]")] 
    public class BusesController : ControllerBase
    {

        private readonly ICrudServiceAsync<BusModel> _busService;

        public BusesController(ICrudServiceAsync<BusModel> busService)
        {
            _busService = busService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)] 
        public async Task<ActionResult<IEnumerable<BusModel>>> GetBuses()
        {
            var buses = await _busService.ReadAllAsync();
            return Ok(buses);
        }

        [HttpGet("paged")] 
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<BusModel>>> GetBusesPaged([FromQuery] int page = 1, [FromQuery] int amount = 10)
        {
            if (page < 1 || amount < 1)
            {

                return BadRequest("Page and amount must be positive integers.");
            }
            var buses = await _busService.ReadAllAsync(page, amount);
            return Ok(buses); 
        }


        [HttpGet("{id}")] 
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] 
        public async Task<ActionResult<BusModel>> GetBus(Guid id)
        {
            var bus = await _busService.ReadAsync(id);
            if (bus == null)
            {
                return NotFound(); 
            }
            return Ok(bus); 
        }

     
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)] 
        [ProducesResponseType(StatusCodes.Status400BadRequest)] 
        public async Task<ActionResult<BusModel>> PostBus([FromBody] BusCreateModel busCreateDto)
        {
 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }

            var busModel = new BusModel
            {

                PlateNumber = busCreateDto.PlateNumber,
                Model = busCreateDto.Model,
                Capacity = busCreateDto.Capacity,
                DriverId = busCreateDto.DriverId,
                RouteId = busCreateDto.RouteId
            };

            var success = await _busService.CreateAsync(busModel);
            if (!success)
            {

                return BadRequest("Could not create bus. Possible duplicate ID or internal error.");
            }
            await _busService.SaveAsync();

            return CreatedAtAction(nameof(GetBus), new { id = busModel.Id }, busModel);
        }

       
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)] 
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutBus(Guid id, [FromBody] BusUpdateModel busUpdateDto)
        {

            if (id != busUpdateDto.Id)
            {
                return BadRequest("ID in URL does not match ID in body.");
            }

            var existingBus = await _busService.ReadAsync(id);
            if (existingBus == null)
            {
                return NotFound(); 
            }


            if (busUpdateDto.PlateNumber != null) existingBus.PlateNumber = busUpdateDto.PlateNumber;
            if (busUpdateDto.Model != null) existingBus.Model = busUpdateDto.Model;
            if (busUpdateDto.Capacity.HasValue) existingBus.Capacity = busUpdateDto.Capacity.Value;
            if (busUpdateDto.DriverId.HasValue) existingBus.DriverId = busUpdateDto.DriverId.Value;
            if (busUpdateDto.RouteId.HasValue) existingBus.RouteId = busUpdateDto.RouteId.Value;

            var success = await _busService.UpdateAsync(existingBus);
            if (!success)
            {
                return BadRequest("Could not update bus due to an internal service error.");
            }
            await _busService.SaveAsync();

            return Ok(existingBus); 
        }

      
        
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] 
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBus(Guid id)
        {
            var bus = await _busService.ReadAsync(id);
            if (bus == null)
            {
                return NotFound(); 
            }

            var success = await _busService.RemoveAsync(bus);
            if (!success)
            {
                return BadRequest("Could not delete bus due to an internal service error.");
            }
            await _busService.SaveAsync();

            return NoContent(); 
        }
    }
}