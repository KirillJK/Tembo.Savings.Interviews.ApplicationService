using FluentValidation;
using Services.Applications.Validation.Parameters;
using Services.Applications.Validation.Validators;

namespace Services.Applications.Validation
{
    public class ValidationFactory : IValidationFactory
    {
        public IValidatorWrapper Create(IEnumerable<IValidationParameters> validationParameters)
        {
            List<ValidatorPack> list = new List<ValidatorPack>();
            var validators = validationParameters.Select(Get).Where(a => a != null);
            IValidatorWrapper validatorWrapper = new ChainValidatorWrapper(validators.ToList());
            return validatorWrapper;
        }

        private ValidatorPack Get(IValidationParameters parameters)
        {
            if (parameters is AgeValidatorParameters ageValidatorParameters)
            {
                return new ValidatorPack()
                {
                    Validator = new AgeValidator(ageValidatorParameters),
                    Getter = (a) => a.Applicant
                };
            }

            if (parameters is MinimumPaymentValidatorParameters minimumPaymentValidatorParameters)
            {
                return new ValidatorPack()
                {
                    Validator = new MinimumPaymentValidator(minimumPaymentValidatorParameters),
                    Getter = (a) => a.Payment
                };
            }
            return null;
        }
    }
}
