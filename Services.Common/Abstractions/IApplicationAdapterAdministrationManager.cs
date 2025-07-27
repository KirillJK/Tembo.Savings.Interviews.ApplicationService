
using Services.Applications.Validation;
using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;

namespace Services.Applications
{
    public interface IApplicationAdapterAdministrationManager
    {
        IApplicationAdapterAdministrationManager Register(ProductCode productCode, IApplicationAdapter applicationAdapter, List<IValidationParameters> parameters);
    }  
}
