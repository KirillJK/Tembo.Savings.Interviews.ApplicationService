using Services.Applications.Validation;
using Services.Common.Abstractions.Abstractions;

namespace Services.Applications
{
    public class ProductPack
    {
        public required IApplicationAdapter Adapter { get; set; }
        public required IValidatorWrapper Validation { get; set; }
    }

        
}
