using FluentValidation;
using Services.Common.Abstractions.Model;

namespace Services.Applications.Validation
{
    internal class ChainValidatorWrapper : IValidatorWrapper
    {
        private List<ValidatorPack> _validators;

        public ChainValidatorWrapper(List<ValidatorPack> validators)
        {
            _validators = validators;
        }

        public void Validate(Application application)
        {
            List<string> allErrors = new List<string>();
            foreach (var validator in _validators)
            {
                var obj = validator.Getter(application);
                var result = validator.Validator.Validate(new ValidationContext<object>(obj));
                allErrors.AddRange(result.Errors.Select(a=>a.ErrorMessage));
            }
            if (allErrors.Any()) throw new AggregateException(allErrors.Select(a => new Exception(a)));
        }
    }
}
