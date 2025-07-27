using Moq;
using Newtonsoft.Json;
using Services.Administrator.Configuration;
using Services.AdministratorOne.Abstractions;
using Services.AdministratorOne.Abstractions.Model;
using Services.AdministratorOne.Adapter;
using Services.AdministratorTwo.Abstractions;
using Services.AdministratorTwo.Adapter;
using Services.Applications.Validation;
using Services.Applications.Validation.Parameters;
using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;
using Xunit;

namespace Services.Applications.Tests;

public class ProductOneTests
{
  



    [Fact]
    public async Task Application_for_ProductOne_creates_Investor_in_AdministratorOne()
    {
        var bus = TestHelper.GetDefaultBus();
        var kycService = TestHelper.GetKycServiceAlwaysValid(Guid.Empty);


        var mockAdministrationService1 = new Mock<AdministratorOne.Abstractions.IAdministrationService>();
        mockAdministrationService1.Setup(a => a.CreateInvestor(It.Is<CreateInvestorRequest>(req=> TestHelper.IsEquivalent(req, TestHelper.GetDefaultInvestorRequest())))).Returns(new CreateInvestorResponse()
        {
            AccountId = "SomeAcc",
            InvestorId = "SomeInvestor",
            PaymentId = "SomePayment",
            Reference = "SomeReference"
        }); 
        var administrationService1 = mockAdministrationService1.Object;

        var mockAdministrationService2 = new Mock<AdministratorTwo.Abstractions.IAdministrationService>();
        var administrationService2 = mockAdministrationService2.Object;


        var applicationAdapterManager = TestHelper.InitDefaultApplicationAdapterManager();

        Configurator.Configure(applicationAdapterManager, bus, administrationService1, administrationService2);

        var applicationProcessor = new ApplicationProcessor(applicationAdapterManager, kycService, bus);
        
        await applicationProcessor.Process(TestHelper.GetDefaultApplication());
        mockAdministrationService1.VerifyAll();
    }

    [Fact]
    public async Task Application_for_ProductOne_does_not_create_Investor_in_AdministratorOne_when_kyc_is_not_passed()
    {

        var bus = TestHelper.GetDefaultBus();
        var kycService = TestHelper.GetKycServiceAlwaysInValid(Guid.Empty);


        var mockAdministrationService1 = new Mock<AdministratorOne.Abstractions.IAdministrationService>();
        var administrationService1 = mockAdministrationService1.Object;

        var mockAdministrationService2 = new Mock<AdministratorTwo.Abstractions.IAdministrationService>();
        var administrationService2 = mockAdministrationService2.Object;


        var applicationAdapterManager = TestHelper.InitDefaultApplicationAdapterManager();

        Configurator.Configure(applicationAdapterManager, bus, administrationService1, administrationService2);

        var applicationProcessor = new ApplicationProcessor(applicationAdapterManager, kycService, bus);

        var expectedMessage = "KYC validation failed";

        var exception = await Assert.ThrowsAsync<Exception>(() =>
            applicationProcessor.Process(TestHelper.GetDefaultApplication()));
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public async Task Application_for_ProductOne_does_not_create_Investor_in_AdministratorOne_when_It_should_create_anotherProduct()
    {

        var bus = TestHelper.GetDefaultBus();
        var kycService = TestHelper.GetKycServiceAlwaysValid(Guid.Empty);


        var mockAdministrationService1 = new Mock<AdministratorOne.Abstractions.IAdministrationService>();
        mockAdministrationService1.Setup(a => a.CreateInvestor(It.Is<CreateInvestorRequest>(req => TestHelper.IsEquivalent(req, TestHelper.GetDefaultInvestorRequest())))).Returns(new CreateInvestorResponse()
        {
            AccountId = "SomeAcc",
            InvestorId = "SomeInvestor",
            PaymentId = "SomePayment",
            Reference = "SomeReference"
        });
        var administrationService1 = mockAdministrationService1.Object;

        var mockAdministrationService2 = new Mock<AdministratorTwo.Abstractions.IAdministrationService>();
        var administrationService2 = mockAdministrationService2.Object;


        var applicationAdapterManager = TestHelper.InitDefaultApplicationAdapterManager();

        applicationAdapterManager.Register(ProductCode.ProductTwo, new ApplicationAdapterProductOne(bus, administrationService1), new List<IValidationParameters>());
       

        var applicationProcessor = new ApplicationProcessor(applicationAdapterManager, kycService, bus);

        var expectedMessage = "Product code not found";

        var exception = await Assert.ThrowsAsync<Exception>(() =>
            applicationProcessor.Process(TestHelper.GetDefaultApplication()));
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public async Task Application_for_ProductOne_creates_Investor_in_AdministratorOne_Bus_Method_Should_be_invoked()
    {
        var defaultApplication = TestHelper.GetDefaultApplication();
        Guid guid = defaultApplication.Id;

        var mockBus = new Mock<IBus>();
        mockBus.Setup(a => a.PublishAsync(It.Is<InvestorCreated>(req => req.InvestorId == "SomeInvestor")));
        mockBus.Setup(a => a.PublishAsync(It.Is<AccountCreated>(req => req.AccountId == "SomeAcc")));
        mockBus.Setup(a => a.PublishAsync(It.Is<ApplicationCompleted>(req => req.ApplicationId == guid)));
        var bus = mockBus.Object;

        var kycService = TestHelper.GetKycServiceAlwaysValid(Guid.Empty);


        var mockAdministrationService1 = new Mock<AdministratorOne.Abstractions.IAdministrationService>();
        mockAdministrationService1.Setup(a => a.CreateInvestor(It.Is<CreateInvestorRequest>(req => TestHelper.IsEquivalent(req, TestHelper.GetDefaultInvestorRequest())))).Returns(new CreateInvestorResponse()
        {
            AccountId = "SomeAcc",
            InvestorId = "SomeInvestor",
            PaymentId = "SomePayment",
            Reference = "SomeReference"

        });
        var administrationService1 = mockAdministrationService1.Object;

        var mockAdministrationService2 = new Mock<AdministratorTwo.Abstractions.IAdministrationService>();
        var administrationService2 = mockAdministrationService2.Object;


        var applicationAdapterManager = TestHelper.InitDefaultApplicationAdapterManager();

        Configurator.Configure(applicationAdapterManager, bus, administrationService1, administrationService2);

        var applicationProcessor = new ApplicationProcessor(applicationAdapterManager, kycService, bus);

       
        await applicationProcessor.Process(TestHelper.GetDefaultApplication());
        mockBus.VerifyAll();
    }

    [Fact]
    public async Task Application_for_ProductOne_creates_Investor_in_AdministratorOne_Bus_Method_Kyc_Failed_Should_Be_Invoked()
    {
        var defaultApplication = TestHelper.GetDefaultApplication();
        Guid guid = defaultApplication.Id;

        var mockBus = new Mock<IBus>();
        mockBus.Setup(a => a.PublishAsync(It.Is<KycFailed>(req => req.ReportId == guid)));
        var bus = mockBus.Object;

        var kycService = TestHelper.GetKycServiceAlwaysInValid(guid);


        var mockAdministrationService1 = new Mock<AdministratorOne.Abstractions.IAdministrationService>();
        mockAdministrationService1.Setup(a => a.CreateInvestor(It.Is<CreateInvestorRequest>(req => TestHelper.IsEquivalent(req, TestHelper.GetDefaultInvestorRequest()))));
        var administrationService1 = mockAdministrationService1.Object;

        var mockAdministrationService2 = new Mock<AdministratorTwo.Abstractions.IAdministrationService>();
        var administrationService2 = mockAdministrationService2.Object;


        var applicationAdapterManager = TestHelper.InitDefaultApplicationAdapterManager();

        Configurator.Configure(applicationAdapterManager, bus, administrationService1, administrationService2);

        var applicationProcessor = new ApplicationProcessor(applicationAdapterManager, kycService, bus);


        try
        {
            await applicationProcessor.Process(TestHelper.GetDefaultApplication());
        }
        catch (Exception)
        {

        }
       
        mockBus.VerifyAll();
    }

    [Fact]
    public async Task Application_for_ProductOne_creates_Investor_in_AdministratorOne_ButValidationIsNotPassed()
    {
        var bus = TestHelper.GetDefaultBus();
        var kycService = TestHelper.GetKycServiceAlwaysValid(Guid.Empty);


        var mockAdministrationService1 = new Mock<AdministratorOne.Abstractions.IAdministrationService>();
        mockAdministrationService1.Setup(a => a.CreateInvestor(It.Is<CreateInvestorRequest>(req => TestHelper.IsEquivalent(req, TestHelper.GetDefaultInvestorRequest())))).Returns(new CreateInvestorResponse()
        {
            AccountId = "SomeAcc",
            InvestorId = "SomeInvestor",
            PaymentId = "SomePayment",
            Reference = "SomeReference"
        });
        var administrationService1 = mockAdministrationService1.Object;

        var mockAdministrationService2 = new Mock<AdministratorTwo.Abstractions.IAdministrationService>();
        var administrationService2 = mockAdministrationService2.Object;


        var applicationAdapterManager = TestHelper.InitDefaultApplicationAdapterManager();

        Configurator.Configure(applicationAdapterManager, bus, administrationService1, administrationService2);

        var applicationProcessor = new ApplicationProcessor(applicationAdapterManager, kycService, bus);
        var application = new Application()
        {
            Applicant = new User()
            {
                DateOfBirth = DateOnly.FromDateTime(DateTime.Now)
            },
            Payment = new Payment(new BankAccount(), new Money("GBP", 20))

        };

        var exception = await Assert.ThrowsAsync<AggregateException>(() =>
            applicationProcessor.Process(application));
        Assert.Equal("User age must be between 18 and 39 years.", exception.InnerExceptions.First().Message);
    }

    [Fact]
    public async Task Application_for_ProductOne_creates_Investor_in_AdministratorOne_ButValidationIsNotPassedNotEnoughMoney()
    {
        var bus = TestHelper.GetDefaultBus();
        var kycService = TestHelper.GetKycServiceAlwaysValid(Guid.Empty);


        var mockAdministrationService1 = new Mock<AdministratorOne.Abstractions.IAdministrationService>();
        mockAdministrationService1.Setup(a => a.CreateInvestor(It.Is<CreateInvestorRequest>(req => TestHelper.IsEquivalent(req, TestHelper.GetDefaultInvestorRequest())))).Returns(new CreateInvestorResponse()
        {
            AccountId = "SomeAcc",
            InvestorId = "SomeInvestor",
            PaymentId = "SomePayment",
            Reference = "SomeReference"
        });
        var administrationService1 = mockAdministrationService1.Object;

        var mockAdministrationService2 = new Mock<AdministratorTwo.Abstractions.IAdministrationService>();
        var administrationService2 = mockAdministrationService2.Object;


        var applicationAdapterManager = TestHelper.InitDefaultApplicationAdapterManager();

        Configurator.Configure(applicationAdapterManager, bus, administrationService1, administrationService2);

        var applicationProcessor = new ApplicationProcessor(applicationAdapterManager, kycService, bus);
        var application = new Application()
        {
            Applicant = new User()
            {
                DateOfBirth = DateOnly.FromDateTime(DateTime.Now).AddYears(-30)
            },
            Payment = new Payment(new BankAccount(), new Money("GBP", (decimal)0.5))

        };

        var exception = await Assert.ThrowsAsync<AggregateException>(() =>
            applicationProcessor.Process(application));
        Assert.Equal("Payment amount must be at least 0.99", exception.InnerExceptions.First().Message);
    }





}