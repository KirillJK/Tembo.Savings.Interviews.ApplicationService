using Services.AdministratorTwo.Abstractions;
using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;

namespace Services.AdministratorTwo.Adapter
{
    public class ApplicationAdapterProductTwo : IApplicationAdapter
    {
        private IBus _bus;
        private IAdministrationService _administrationService;
        public ApplicationAdapterProductTwo(IBus bus, IAdministrationService administrationService){
            _bus = bus;
            _administrationService = administrationService;
        }
        public async Task Process(Application application)
        {
            var investorResult = await _administrationService.CreateInvestorAsync(application.Applicant);
            if (!investorResult.IsSuccess) ThrowError(investorResult.Error, nameof(_administrationService.CreateInvestorAsync));
            await _bus.PublishAsync(new InvestorCreated(application.Applicant.Id, investorResult.Value.ToString()));
            
            var accountResult = await _administrationService.CreateAccountAsync(investorResult.Value, ProductCode.ProductTwo);
            if (!accountResult.IsSuccess) ThrowError(accountResult.Error, nameof(_administrationService.CreateAccountAsync));
            await _bus.PublishAsync(new AccountCreated(investorResult.Value.ToString(), ProductCode.ProductTwo, accountResult.Value.ToString()));
            
            var paymentResult = await _administrationService.ProcessPaymentAsync(accountResult.Value, application.Payment);
            if (!paymentResult.IsSuccess) ThrowError(paymentResult.Error, nameof(_administrationService.ProcessPaymentAsync));
            await _bus.PublishAsync(new ApplicationCompleted(application.Id));
        }

        private void ThrowError(Error error, string methodName)
        {
            throw new Exception($"Error on {methodName}, ErrorCode = {error.Code}, Message = {error.Description}, System = {error.System}");
        }
    }
}
