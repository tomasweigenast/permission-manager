using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PermissionManager.Core.Utils;

namespace PermissionManager.API.Utils;

public static class ValidationResultExtensions
{
    private class CustomProblemDetails : ProblemDetails
    {
        public required Dictionary<string, string[]> Errors { get; init; }
        
        public required string TraceId { get; init; }
    }
    
    public static IActionResult ToBadRequestResult(this ValidationResult result, ModelStateDictionary modelState, string traceId) 
    {
        foreach (var error in result.Errors)
            modelState.AddModelError(error.PropertyName.ToSnakeCase(), error.ErrorMessage);

        return new BadRequestObjectResult(new CustomProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Title = "One or more validation errors occurred.",
            Status = 400,
            Errors = modelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? []),
            TraceId = traceId,
        });
    }
}