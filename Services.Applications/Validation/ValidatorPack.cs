using FluentValidation;
using Services.Common.Abstractions.Model;

namespace Services.Applications.Validation
{
    internal class ValidatorPack
    {
        public Func<Application, object> Getter { get; set; }
        public IValidator Validator { get; set; }
    }
}
