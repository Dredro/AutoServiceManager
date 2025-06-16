using Application.Clients.Commands;
using Application.Clients.Queries;
using Application.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ValidationException = Application.Exceptions.ValidationException; 

namespace API.Controllers; 

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ClientsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new client.
    /// </summary>
    /// <param name="command">Client creation command with personal information.</param>
    /// <returns>The ID of the newly created client.</returns>
    /// <response code="201">Returns the ID of the newly created client.</response>
    /// <response code="409">If a client with the same personal info already exists.</response>
    /// <response code="400">If the request body is invalid.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateClient([FromBody] CreateClientCommand command)
    {
        try
        {
            var clientId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetClient), new { id = clientId }, clientId);
        }
        catch (InvalidOperationException ex) 
        {
            return Conflict(new { message = ex.Message }); 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating client: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while creating the client." });
        }
    }

    /// <summary>
    /// Gets a client by their unique ID.
    /// </summary>
    /// <param name="id">The unique ID of the client.</param>
    /// <returns>The client details.</returns>
    /// <response code="200">Returns the client details.</response>
    /// <response code="404">If the client is not found.</response>
    /// <response code="400">If the ID format is invalid.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetClient(string id)
    {
        try
        {
            var query = new GetClientQuery(id);
            var client = await _mediator.Send(query);
            return Ok(client);
        }
        catch (NotFoundException ex) 
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ValidationException ex) 
        {
            return BadRequest(new { message = ex.Message }); 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting client: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving the client." });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClient(Guid id) 
    {
        try
        {
            if (id == Guid.Empty) 
            {
                return BadRequest(new { message = "Client ID is required and must be a valid GUID." });
            }

            await _mediator.Send(new DeleteClientCommand(id.ToString())); 
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
    [HttpPatch]
    public async Task<IActionResult> UpdateClient([FromBody] EditClientCommand command)
    {
        try
        {
           await _mediator.Send(command);
           return Ok();
        }
        catch (NotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
        
    }
    /// <summary>
    /// Gets a list of all clients.
    /// </summary>
    /// <returns>A list of client details.</returns>
    /// <response code="200">Returns a list of client details.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetClients()
    {
        try
        {
            var query = new GetClientsQuery();
            var clients = await _mediator.Send(query);
            return Ok(clients);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting clients: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving clients." });
        }
    }
}