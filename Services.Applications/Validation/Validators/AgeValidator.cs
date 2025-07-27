using FluentValidation;
using Services.Applications.Validation.Parameters;
using Services.Common.Abstractions.Model;

namespace Services.Applications.Validation.Validators
{
    internal class AgeValidator : AbstractValidator<User>
    {
        public AgeValidator(AgeValidatorParameters parameters)
        {
            RuleFor(user => user.DateOfBirth)
                .Must(dateOfBirth =>
                {
                    var today = DateOnly.FromDateTime(DateTime.Today);
                    var age = today.Year - dateOfBirth.Year;
                    if (dateOfBirth > today.AddYears(-age)) age--;

                    if (parameters.MinAge.HasValue && age < parameters.MinAge.Value)
                        return false;

                    if (parameters.MaxAge.HasValue && age > parameters.MaxAge.Value)
                        return false;

                    return true;
                })
                .WithMessage(user =>
                {
                    var min = parameters.MinAge?.ToString() ?? "_";
                    var max = parameters.MaxAge?.ToString() ?? "_";
                    return $"User age must be between {min} and {max} years.";
                });
        }
    }

}
