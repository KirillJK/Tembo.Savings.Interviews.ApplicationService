namespace Services.Applications.Validation.Parameters
{
    public class AgeValidatorParameters : IValidationParameters
    {
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }

    }
}
