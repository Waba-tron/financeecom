using EcomBackend.Application.ProductTypes.Commands.CreateProductType;
using EcomBackend.Application.ProductTypes.Dtos;
using EcomBackend.Application.ProductTypes.Queries.GetAllProductTypes;
using EcomBackend.Domain.Common.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcomBackend.API.Controllers;

[ApiController]
[Route("api/product-types")]
public class ProductTypesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductTypesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllProductTypesQuery());
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = RoleConstants.Admin)]
    public async Task<IActionResult> Create(CreateProductTypeCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = RoleConstants.Admin)]
    public async Task<IActionResult> Update(int id, UpdateProductTypeDto updateProductTypeDto)
    {
        var command = new UpdateProductTypeCommand(
            id,
            updateProductTypeDto.Name,
            updateProductTypeDto.Description,
            updateProductTypeDto.IsActive
        );

        var result = await _mediator.Send(command);

        return result == null ? NotFound() : Ok(result);
    }
}
