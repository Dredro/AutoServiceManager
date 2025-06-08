using Application.Exceptions;
using Application.Workers.Commands;
using Application.Workers.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<WorkersController> _logger;


    public WorkersController(IMediator mediator, ILogger<WorkersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateWorker([FromBody] CreateWorkerCommand command)
    {
        try
        {
            var workerId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetWorker), new { id = workerId }, workerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating worker: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while creating the worker." });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetWorker(string id)
    {
        try
        {
            var query = new GetWorkerQuery(id);
            var workerDto = await _mediator.Send(query);
            return Ok(workerDto);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Worker not found: {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (FormatException)
        {
            _logger.LogWarning("Invalid ID format for GetWorker: {Id}", id);
            return BadRequest(new { message = "Invalid ID format. ID must be a valid GUID." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting worker: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while retrieving the worker." });
        }
    }
}