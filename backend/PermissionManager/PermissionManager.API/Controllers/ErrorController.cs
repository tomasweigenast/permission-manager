using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace PermissionManager.API.Controllers;

[ApiController]
[Microsoft.AspNetCore.Components.Route("[controller]")]
public class ErrorController : ControllerBase
{
    [Route("/error")]
    [SwaggerIgnore]
    public IActionResult Error() => Problem(
        detail: null,
        statusCode: 500
    );
}