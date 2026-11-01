using System.Security.Claims;
using EcomBackend.Application.Carts.Commands.AddToCart;
using EcomBackend.Application.Carts.Commands.ClearCart;
using EcomBackend.Application.Carts.Commands.RemoveCartItem;
using EcomBackend.Application.Carts.Commands.UpdateCartItem;
using EcomBackend.Application.Carts.Queries.GetMyCart;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcomBackend.API.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly IMediator _mediator;

    public CartController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyCart()
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new GetMyCartQuery(userId));
        return Ok(result);
    }

    public record AddToCartRequest(int ProductId, int Quantity);

    [HttpPost("items")]
    public async Task<IActionResult> AddItem(AddToCartRequest request)
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new AddToCartCommand(userId, request.ProductId, request.Quantity));
        return Ok(result);
    }

    [HttpDelete("items/{productId:int}")]
    public async Task<IActionResult> RemoveItem(int productId)
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new RemoveCartItemCommand(userId, productId));
        return result == null ? NotFound() : Ok(result);
    }

    public record UpdateCartItemRequest(int Quantity);

    [HttpPut("items/{productId:int}")]
    public async Task<IActionResult> UpdateItem(int productId, UpdateCartItemRequest request)
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new UpdateCartItemCommand(userId, productId, request.Quantity));
        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> Clear()
    {
        var userId = GetUserId();
        await _mediator.Send(new ClearCartCommand(userId));
        return NoContent();
    }

    private int GetUserId()
    {
        var sub = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(sub, out var id))
            throw new UnauthorizedAccessException("Invalid user token.");
        return id;
    }
}
