using FluentValidation;
using Services.Applications.Validation;
using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;

namespace Services.Applications
{
    internal class ApplicationAdapterManager : IApplicationAdapterAdministrationManager, IApplicationAdapterManager
    {
        Dictionary<ProductCode, ProductPack> _index = new Dictionary<ProductCode, ProductPack>();
        IValidationFactory _validationFactory;

        public ApplicationAdapterManager(IValidationFactory validationFactory)
        {
            _validationFactory = validationFactory;
        }

        public ProductPack Get(ProductCode code)
        {
            if (!_index.ContainsKey(code)) return null;
            return _index[code];
        }


        public IApplicationAdapterAdministrationManager Register(ProductCode productCode, IApplicationAdapter applicationAdapter, List<IValidationParameters> parameters)
        {
            var validationWrapper = _validationFactory.Create(parameters);
            _index[productCode] = new ProductPack()
            {
                Validation = validationWrapper,
                Adapter = applicationAdapter
            };
            return this;
        }
    }



    

    
}
