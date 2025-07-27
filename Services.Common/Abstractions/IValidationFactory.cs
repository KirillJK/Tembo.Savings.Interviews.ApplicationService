namespace Services.Applications.Validation
{
    public interface IValidationFactory
    {
        IValidatorWrapper Create(IEnumerable<IValidationParameters> validationParameters);
    }
}
