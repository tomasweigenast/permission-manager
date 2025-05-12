using FluentValidation;
using PermissionManager.Application.DTOs;

namespace PermissionManager.Application.Validators;

/// <summary>
/// An <see cref="AbstractValidator{T}"/> which validates a <see cref="ModifyPermissionDto"/> record
/// </summary>
public class ModifyPermissionDtoValidator : AbstractValidator<ModifyPermissionDto>
{
    public ModifyPermissionDtoValidator()
    {
        RuleFor(p => p.EmployeeName)
            .Length(1, 100)
                .When(x => !string.IsNullOrWhiteSpace(x.EmployeeName))
                .WithMessage("{PropertyName} must have between 1 and 100 characters.");
        
        RuleFor(p => p.EmployeeSurname)
            .Length(1, 100)
            .When(x => !string.IsNullOrWhiteSpace(x.EmployeeSurname))
            .WithMessage("{PropertyName} must have between 1 and 100 characters.");
        
        RuleFor(p => p.PermissionTypeId)
            .GreaterThan(0)
                .When(x => x.PermissionTypeId.HasValue)
                .WithMessage("{PropertyName} must be greater than zero.");
    }
}