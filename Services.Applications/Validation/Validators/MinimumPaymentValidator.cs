using FluentValidation;
using Services.Applications.Validation.Parameters;
using Services.Common.Abstractions.Model;

namespace Services.Applications.Validation.Validators
{
    internal class MinimumPaymentValidator : AbstractValidator<Payment>
    {
        public MinimumPaymentValidator(MinimumPaymentValidatorParameters parameters)
        {
            RuleFor(p => p.Amount.Amount)
                .Must((payment, amount) =>
                {
                    if (parameters.MinimumPayment.HasValue && amount < parameters.MinimumPayment.Value)
                        return false;

                    return true;
                })
                .WithMessage($"Payment amount must be at least {parameters.MinimumPayment}");

        }


    }

}
