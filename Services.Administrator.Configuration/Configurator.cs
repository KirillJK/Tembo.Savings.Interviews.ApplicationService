
using Services.AdministratorOne.Abstractions;
using Services.AdministratorOne.Adapter;
using Services.AdministratorTwo.Adapter;
using Services.Applications;
using Services.Applications.Validation.Parameters;
using Services.Common.Abstractions.Abstractions;

namespace Services.Administrator.Configuration
{
    public static class Configurator
    {
        public static void Configure(IApplicationAdapterAdministrationManager manager, IBus bus, IAdministrationService administrationServiceOne, Services.AdministratorTwo.Abstractions.IAdministrationService administrationServiceTwo)
        {
            manager.Register(Common.Abstractions.Model.ProductCode.ProductOne, new ApplicationAdapterProductOne(bus, administrationServiceOne), new List<Applications.Validation.IValidationParameters>()
            {
               new AgeValidatorParameters()
               {
                   MinAge = 18,
                   MaxAge = 39
               },
               new MinimumPaymentValidatorParameters()
               {
                   MinimumPayment = (Decimal)0.99
               }
            });

            manager.Register(Common.Abstractions.Model.ProductCode.ProductTwo, new ApplicationAdapterProductTwo(bus, administrationServiceTwo), new List<Applications.Validation.IValidationParameters>()
            {
               new AgeValidatorParameters()
               {
                   MinAge = 18,
               },
               new MinimumPaymentValidatorParameters()
               {
                   MinimumPayment = (Decimal)0.99
               }
            });

        }
    }
}
