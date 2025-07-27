using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;

namespace Services.Applications;

public class ApplicationProcessor : IApplicationProcessor
{
    private IApplicationAdapterManager _applicationAdapterManager;
    private IKycService _kycService;
    private IBus _bus;

    public ApplicationProcessor(IApplicationAdapterManager applicationAdapterManager, IKycService kycService, IBus bus)
    {
        _applicationAdapterManager = applicationAdapterManager;
        _kycService = kycService;
        _bus = bus;
    }

    public async Task Process(Application application)
    {
        var report = await _kycService.GetKycReportAsync(application.Applicant);

        if (report.IsSuccess && report.Value.IsVerified)
        {
            var adapter = _applicationAdapterManager.Get(application.ProductCode);
            if (adapter == null)
            {
                throw new Exception("Product code not found");
            }
            adapter.Validation.Validate(application);
            await adapter.Adapter.Process(application);
        }
        else
        {
            await _bus.PublishAsync(new KycFailed(application.Applicant.Id, report.Value.Id));
            throw new Exception("KYC validation failed");
        }
    }
}