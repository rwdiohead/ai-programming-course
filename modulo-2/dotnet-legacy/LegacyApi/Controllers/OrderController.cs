using Microsoft.AspNetCore.Mvc;
using LegacyApi.Domain;
using LegacyApi.Services;

namespace LegacyApi.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
    {
        var result = await _orderService.CreateOrderAsync(request.CustomerEmail, request.Total);

        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(new { 
            message = result.Message, 
            orderId = result.Data?.Id, 
            status = result.Data?.Status.ToString() 
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var result = await _orderService.GetOrderAsync(id);

        if (!result.Success)
            return NotFound(new { message = result.Message });

        return Ok(result.Data);
    }

    [HttpGet("customer/{email}")]
    public async Task<IActionResult> GetCustomerOrders(string email)
    {
        var result = await _orderService.GetCustomerOrdersAsync(email);

        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(result.Data);
    }
}

public class OrderRequest {
    public required string CustomerEmail { get; set; }
    public decimal Total { get; set; }
}