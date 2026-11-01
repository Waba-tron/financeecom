using EcomBackend.Application.Orders.Commands.UpdateOrderStatus;
using EcomBackend.Application.Orders.Queries;
using EcomBackend.Domain.Common.Constants;
using EcomBackend.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcomBackend.API.Controllers;

[ApiController]
[Route("api/admin/orders")]
[Authorize(Roles = RoleConstants.Admin)]
public class AdminOrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminOrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        var result = await _mediator.Send(new GetAllOrdersQuery());
        return Ok(result);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
    {
        var command = new UpdateOrderStatusCommand(id, request.Status);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}

public record UpdateOrderStatusRequest(OrderStatus Status);
