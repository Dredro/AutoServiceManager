using Application.Exceptions;
using Application.Services.Commands;
using Application.Services.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ServicesController> _logger;

    public ServicesController(IMediator mediator, ILogger<ServicesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateService([FromBody] CreateServiceCommand command)
    {
        try
        {
            var serviceId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetService), new { id = serviceId }, serviceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating service: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while creating the service." });
        }
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> EditService([FromBody] EditServiceCommand command)
    {
        try
        {
            var serviceId = await _mediator.Send(command);
            return StatusCode(StatusCodes.Status202Accepted,
                new { message = $"Service {serviceId} updated successfully." });
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error when editing service: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Service not found during edit: {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error editing service: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while editing the service." });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetService(string id)
    {
        try
        {
            var query = new GetServiceQuery(id);
            var serviceDto = await _mediator.Send(query);
            return Ok(serviceDto);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Service not found: {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (FormatException)
        {
            _logger.LogWarning("Invalid ID format for GetService: {Id}", id);
            return BadRequest(new { message = "Invalid ID format. ID must be a valid GUID." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting service: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while retrieving the service." });
        }
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetServices()
    {
        try
        {
            var query = new GetServicesQuery();
            var servicesDto = await _mediator.Send(query);
            return Ok(servicesDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting services: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while retrieving services." });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteService(Guid id) 
    {
        try
        {
            if (id == Guid.Empty) 
            {
                return BadRequest(new { message = "Service ID is required and must be a valid GUID." });
            }

            await _mediator.Send(new DeleteServiceCommand(id.ToString())); 
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
        }
    }
}