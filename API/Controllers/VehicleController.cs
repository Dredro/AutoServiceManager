using Application.Exceptions;
using Application.Vehicles.Commands;
using Application.Vehicles.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<VehiclesController> _logger;


    public VehiclesController(IMediator mediator, ILogger<VehiclesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateVehicle([FromBody] CreateVehicleCommand command)
    {
        try
        {
            var vehicleId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetVehicle), new { vehicleId = vehicleId }, vehicleId);
        }
        catch (BadRequestException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Client not found during vehicle creation: {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating vehicle: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while creating the vehicle." });
        }
    }

    [HttpGet("{vehicleId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetVehicle(string vehicleId)
    {
        try
        {
            var query = new GetVehicleQuery(vehicleId);
            var vehicleDto = await _mediator.Send(query);
            return Ok(vehicleDto);
        }
        catch (BadRequestException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Vehicle not found: {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (FormatException)
        {
            _logger.LogWarning("Invalid VehicleId format for GetVehicle: {VehicleId}", vehicleId);
            return BadRequest(new { message = "Invalid Vehicle ID format. ID must be a valid GUID." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting vehicle: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while retrieving the vehicle." });
        }
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllVehicles()
    {
        try
        {
            var query = new GetAllVehiclesQuery();
            var vehicleDtos = await _mediator.Send(query);
            return Ok(vehicleDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all vehicles: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while retrieving vehicles." });
        }
    }
}