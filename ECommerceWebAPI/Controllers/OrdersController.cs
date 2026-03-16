using ECommerceWebAPI.DTOs;
using ECommerceWebAPI.Entities;
using ECommerceWebAPI.Enums;
using ECommerceWebAPI.Exceptions;
using ECommerceWebAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWebAPI.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _service;
        private readonly IOrderStatusService _statusService;


        public OrdersController(IOrderService service, IOrderStatusService statusService)
        {
            _service = service;
            _statusService = statusService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderRequest request)
        {
            try
            {
                var orderId = await _service.CreateOrderAsync(request);

                return CreatedAtAction(
                    nameof(GetOrderById),
                    new { id = orderId },
                    new { OrderId = orderId });
            }
            catch (NotFoundException ex)
            {
                return NotFound(ToProblemDetails(ex.Message, StatusCodes.Status404NotFound));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ToProblemDetails(ex.Message, StatusCodes.Status400BadRequest));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ToProblemDetails(ex.Message, StatusCodes.Status400BadRequest));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrderById(int id)
        {
            try
            {
                var order = await _service.GetOrderByIdAsync(id);
                return Ok(order);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ToProblemDetails(ex.Message, StatusCodes.Status404NotFound));
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ToProblemDetails("An unexpected error occurred.", StatusCodes.Status500InternalServerError, ex));
            }
        }


        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] OrderStatus status)
        {
            try
            {
                await _statusService.UpdateOrderStatusAsync(id, status);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ToProblemDetails(ex.Message, StatusCodes.Status404NotFound));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ToProblemDetails(ex.Message, StatusCodes.Status400BadRequest));
            }
        }

        private static ProblemDetails ToProblemDetails(string detail, int status, Exception? exception = null)
        {
            var pd = new ProblemDetails
            {
                Status = status,
                Title = status switch
                {
                    StatusCodes.Status400BadRequest => "Bad request",
                    StatusCodes.Status404NotFound => "Not found",
                    StatusCodes.Status500InternalServerError => "Internal server error",
                    _ => "Error"
                },
                Detail = detail,
            };

            if (exception != null)
                pd.Extensions["traceId"] = exception.GetType().Name;

            return pd;
        }
    }
}
