using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PermissionManager.API.Utils;
using PermissionManager.Application.Commands.GetPermission;
using PermissionManager.Application.Commands.GetPermissions;
using PermissionManager.Application.Commands.ModifyPermission;
using PermissionManager.Application.Commands.RequestPermission;
using PermissionManager.Application.DTOs;
using Swashbuckle.AspNetCore.Annotations;

namespace PermissionManager.API.Controllers;

/// <summary>
/// A controller that handles <see cref="PermissionManager.Domain.Entities.Permission"/>
/// </summary>
[ApiController]
[Route("api/permissions")]
[Produces("application/json")]
[Consumes("application/json")]
public class PermissionsController(
    IMediator mediator, 
    IMapper mapper, 
    IValidator<RequestPermissionDto> requestPermissionDtoValidator, 
    IValidator<ModifyPermissionDto> modifyPermissionDtoValidator) : ControllerBase
{
    /// <summary>
    /// Creates a new <see cref="PermissionManager.Domain.Entities.Permission"/> 
    /// </summary>
    [HttpPost]
    [SwaggerOperation("Request a new permission for an employee")]
    [SwaggerResponse(StatusCodes.Status201Created, "The permission was created successfully.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The body contains incorrect, malformed or non existing parameters.")]
    public async Task<IActionResult> RequestAsync([FromBody] RequestPermissionDto body, CancellationToken ct)
    {
        var validationResult = await requestPermissionDtoValidator.ValidateAsync(body, ct);
        if (!validationResult.IsValid)
            return validationResult.ToBadRequestResult(ModelState, HttpContext.TraceIdentifier);
        
        var cmd = mapper.Map<RequestPermissionCommand>(body);
        var id = await mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(GetByIdAsync), new { id }, null);
    }

    /// <summary>
    /// Updates an existing <see cref="PermissionManager.Domain.Entities.Permission"/>
    /// </summary>
    [HttpPut("{id:int}")]
    [SwaggerOperation("Modifies the existing permission for an employee")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "The permission was modified successfully.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The requested permission was not found.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The body contains incorrect, malformed or non existing parameters.")]
    public async Task<IActionResult> ModifyAsync(int id, [FromBody] ModifyPermissionDto body, CancellationToken ct)
    {
        var validationResult = await modifyPermissionDtoValidator.ValidateAsync(body, ct);
        if (!validationResult.IsValid)
            return validationResult.ToBadRequestResult(ModelState, HttpContext.TraceIdentifier);
        
        var cmd = mapper.Map<ModifyPermissionCommand>(body);
        cmd = cmd with { Id = id };

        try
        {
            await mediator.Send(cmd, ct);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Returns a paginated list with all the <see cref="PermissionManager.Domain.Entities.Permission"/>
    /// that matches the query
    /// </summary>
    [HttpGet]
    [SwaggerOperation("Retrieves all the assigned permissions")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns a paginated result with all the assigned permission.")]
    public async Task<IActionResult> GetAllAsync(
        [FromQuery(Name="permission_type_id")] 
        [SwaggerSchema("Filter by permission type")]
        int? permissionTypeId,
        
        [FromQuery(Name = "page_size")]
        [SwaggerSchema("Specifies the amount of results to return per page")]
        int pageSize = 50, 
        
        [FromQuery(Name = "page")] 
        [SwaggerSchema("Specifies the page to return")]
        int page = 1,
        CancellationToken ct = default)
    {
        var list = await mediator.Send(new GetPermissionsQuery
        {
            PageSize = pageSize,
            Page = page,
            PermissionTypeId = permissionTypeId,
        }, ct);
        
        return Ok(new PagedResponse<PermissionDto>
        {
            Data = list,
            HasNextPage = list.HasNextPage,
        });
    }

    /// <summary>
    /// Retrieves a single <see cref="PermissionManager.Domain.Entities.Permission"/> searching by its id
    /// </summary>
    [HttpGet("{id:int}")]
    [SwaggerOperation("Retrieves an assigned permission by id")]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns the requested permission.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The requested permission was not found.")]
    public async Task<IActionResult> GetByIdAsync(int id, CancellationToken ct)
    {
        var permission = await mediator.Send(new GetPermissionQuery{ Id = id }, ct);
        if(permission == null) return NotFound();

        return Ok(permission);
    }
}