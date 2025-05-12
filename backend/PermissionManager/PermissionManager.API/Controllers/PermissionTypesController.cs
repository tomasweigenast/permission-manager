using MediatR;
using Microsoft.AspNetCore.Mvc;
using PermissionManager.Application.Commands.GetPermissionTypes;
using Swashbuckle.AspNetCore.Annotations;

namespace PermissionManager.API.Controllers;

[ApiController]
[Route("api/permission_types")]
[Produces("application/json")]
[Consumes("application/json")]
public class PermissionTypesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation("Retrieves all the available permission types")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns a list with all the permission types.")]
    public async Task<IActionResult> GetAllAsync(CancellationToken ct)
    {
        var list = await mediator.Send(new GetPermissionTypesQuery(), ct);
        return Ok(list);
    }
}