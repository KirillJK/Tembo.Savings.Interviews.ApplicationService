using Services.AdministratorOne.Abstractions;
using Services.AdministratorOne.Abstractions.Model;
using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.AdministratorOne.Adapter
{
    public class ApplicationAdapterProductOne : IApplicationAdapter
    {
        private IBus _bus;
        private IAdministrationService _administrationService;

        public ApplicationAdapterProductOne(IBus bus, IAdministrationService administrationService)
        {
            _bus = bus;
            _administrationService = administrationService;
        }

        public async Task Process(Application application)
        {
                var investor = _administrationService.CreateInvestor(new AdministratorOne.Abstractions.Model.CreateInvestorRequest()
                {
                    FirstName = application.Applicant.Forename,
                    LastName = application.Applicant.Surname,
                    Reference = application.Id.ToString(),
                    DateOfBirth = application.Applicant.DateOfBirth.ToString("yyyy-MM-dd"),
                    Nino = application.Applicant.Nino,
                    Addressline1 = application.Applicant.Addresses[0].Addressline1,
                    Addressline2 = application.Applicant.Addresses[0].Addressline2,
                    Addressline3 = application.Applicant.Addresses[0].Addressline3,
                    PostCode = application.Applicant.Addresses[0].PostCode,
                    Product = application.ProductCode.ToString(),
                    SortCode = application.Payment.BankAccount.SortCode,
                    AccountNumber = application.Payment.BankAccount.AccountNumber,
                    InitialPayment = (int)application.Payment.Amount.Amount
                });
                await _bus.PublishAsync(new InvestorCreated(application.Applicant.Id, investor.InvestorId));
                await _bus.PublishAsync(new AccountCreated(investor.InvestorId, ProductCode.ProductOne, investor.AccountId));
                await _bus.PublishAsync(new ApplicationCompleted(application.Id));

    
             
        }
    }



    

    
}
