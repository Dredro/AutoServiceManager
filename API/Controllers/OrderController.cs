using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Orders.Commands;
using Application.Orders.Queries;
using Application.Exceptions; 


namespace API.Controllers;
[ApiController]
[Route("api/[controller]")] 
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrdersController> _logger; 

    public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new order.
    /// </summary>
    /// <param name="command">Order creation command with client, vehicle, services, and spare parts details.</param>
    /// <returns>The ID of the newly created order.</returns>
    /// <response code="201">Returns the ID of the newly created order.</response>
    /// <response code="400">If the request body is invalid, or contains invalid GUID formats, or duplicate spare parts.</response>
    /// <response code="404">If the client, vehicle, services, or spare parts referenced are not found.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
    {
        try
        {
            var orderId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetOrders), new { clientId = command.ClientId }, orderId);
           
        }
        catch (BadRequestException ex) 
        {
            _logger.LogWarning(ex, "Bad request for creating order: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message }); 
        }
        catch (NotFoundException ex) 
        {
            _logger.LogWarning(ex, "Dependent entity not found when creating order: {Message}", ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while creating the order." });
        }
    }

    /// <summary>
    /// Gets a list of orders based on optional filtering criteria.
    /// </summary>
    /// <param name="query">Optional parameters for filtering orders (ClientId, VehicleId, IsPaid, FinalizationDate).</param>
    /// <returns>A list of order details.</returns>
    /// <response code="200">Returns a list of order details (can be empty).</response>
    /// <response code="400">If any ID format in the query parameters is invalid.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOrders([FromQuery] GetOrderQuery query)
    {
        try
        {
            var orders = await _mediator.Send(query);
            return Ok(orders);
        }
        catch (BadRequestException ex) 
        {
            _logger.LogWarning(ex, "Bad request for getting orders: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message }); 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting orders: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while retrieving orders." });
        }
    }
}