using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Parts.Commands;
using Application.Parts.Queries;
using Application.Parts.DTOs;
using Application.Exceptions; 
using System; 

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SparePartsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SparePartsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new spare part.
    /// </summary>
    /// <param name="command">Spare part creation command with details.</param>
    /// <returns>The ID of the newly created spare part.</returns>
    /// <response code="201">Returns the ID of the newly created spare part.</response>
    /// <response code="400">If the request body is invalid or contains bad data.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateSparePart([FromBody] CreateSparePartCommand command)
    {
        try
        {
            var partId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetSparePart), new { id = partId }, partId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating spare part: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while creating the spare part." });
        }
    }

    /// <summary>
    /// Gets a spare part by its unique ID.
    /// </summary>
    /// <param name="id">The unique ID of the spare part.</param>
    /// <returns>The spare part details.</returns>
    /// <response code="200">Returns the spare part details.</response>
    /// <response code="404">If the spare part is not found.</response>
    /// <response code="400">If the ID format is invalid.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSparePart(string id)
    {
        try
        {
            var query = new GetSparePartQuery(id);
            var partDto = await _mediator.Send(query);
            return Ok(partDto);
        }
        catch (NotFoundException ex) 
        {
            return NotFound(new { message = ex.Message }); // 404 Not Found
        }
        catch (FormatException) 
        {
            return BadRequest(new { message = "Invalid ID format. ID must be a valid GUID." }); // 400 Bad Request
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting spare part: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving the spare part." });
        }
    }

    /// <summary>
    /// Gets a list of all spare parts.
    /// </summary>
    /// <returns>A list of spare part details.</returns>
    /// <response code="200">Returns a list of spare part details.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSpareParts()
    {
        try
        {
            var query = new GetSparePartsQuery();
            var partDtos = await _mediator.Send(query);
            return Ok(partDtos);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting all spare parts: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving spare parts." });
        }
    }
}