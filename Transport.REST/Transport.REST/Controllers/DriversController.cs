using Microsoft.AspNetCore.Mvc;
using Transport.REST.Interfaces;
using Transport.REST.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Transport.REST.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // api/drivers
    public class DriversController : ControllerBase
    {
        private readonly ICrudServiceAsync<DriverModel> _driverService;

        public DriversController(ICrudServiceAsync<DriverModel> driverService)
        {
            _driverService = driverService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DriverModel>>> GetDrivers()
        {
            var drivers = await _driverService.ReadAllAsync();
            return Ok(drivers);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DriverModel>> GetDriver(Guid id)
        {
            var driver = await _driverService.ReadAsync(id);
            if (driver == null)
            {
                return NotFound();
            }
            return Ok(driver);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DriverModel>> PostDriver([FromBody] DriverCreateModel driverCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var driverModel = new DriverModel
            {
                FirstName = driverCreateDto.FirstName,
                LastName = driverCreateDto.LastName,
                LicenseNumber = driverCreateDto.LicenseNumber,
                DateOfBirth = driverCreateDto.DateOfBirth
            };

            var success = await _driverService.CreateAsync(driverModel);
            if (!success)
            {
                return BadRequest("Could not create driver.");
            }
            await _driverService.SaveAsync();

            return CreatedAtAction(nameof(GetDriver), new { id = driverModel.Id }, driverModel);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutDriver(Guid id, [FromBody] DriverUpdateModel driverUpdateDto)
        {
            if (id != driverUpdateDto.Id)
            {
                return BadRequest("ID in URL does not match ID in body.");
            }

            var existingDriver = await _driverService.ReadAsync(id);
            if (existingDriver == null)
            {
                return NotFound();
            }

            if (driverUpdateDto.FirstName != null) existingDriver.FirstName = driverUpdateDto.FirstName;
            if (driverUpdateDto.LastName != null) existingDriver.LastName = driverUpdateDto.LastName;
            if (driverUpdateDto.LicenseNumber != null) existingDriver.LicenseNumber = driverUpdateDto.LicenseNumber;
            if (driverUpdateDto.DateOfBirth.HasValue) existingDriver.DateOfBirth = driverUpdateDto.DateOfBirth.Value;

            var success = await _driverService.UpdateAsync(existingDriver);
            if (!success)
            {
                return BadRequest("Could not update driver.");
            }
            await _driverService.SaveAsync();

            return Ok(existingDriver);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDriver(Guid id)
        {
            var driver = await _driverService.ReadAsync(id);
            if (driver == null)
            {
                return NotFound();
            }

            var success = await _driverService.RemoveAsync(driver);
            if (!success)
            {
                return BadRequest("Could not delete driver.");
            }
            await _driverService.SaveAsync();

            return NoContent();
        }
    }
}