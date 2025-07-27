using Services.Common.Abstractions.Model;

namespace Services.Applications.Validation
{
    public interface IValidatorWrapper
    {
        void Validate(Application application);
    }
}
