using FluentValidation;
using PermissionManager.Application.DTOs;
using PermissionManager.Domain.Interfaces;

namespace PermissionManager.Application.Validators;

/// <summary>
/// An <see cref="AbstractValidator{T}"/> which validates a <see cref="RequestPermissionDto"/> record
/// </summary>
public class RequestPermissionDtoValidator : AbstractValidator<RequestPermissionDto>
{
    public RequestPermissionDtoValidator(IPermissionTypeRepository permissionTypeRepository)
    {
        RuleFor(p => p.EmployeeName)
            .NotEmpty().WithMessage("{PropertyName} cannot be empty.")
            .Length(1, 100).WithMessage("{PropertyName} must have between 1 and 100 characters.");
        
        RuleFor(p => p.EmployeeSurname)
            .NotEmpty().WithMessage("{PropertyName} cannot be empty.")
            .Length(1, 100).WithMessage("{PropertyName} must have between 1 and 100 characters.");
        
        RuleFor(p => p.PermissionTypeId)
            .GreaterThan(0)
            .WithMessage("{PropertyName} must be greater than zero.")
            .MustAsync(async (id, ct) => await permissionTypeRepository.GetByIdAsync(id, ct) != null)
            .WithMessage(id => $"{{PropertyName}} = {id.PermissionTypeId} does not exist.");
    }
}