using Moq;
using Services.Administrator.Configuration;
using Services.AdministratorOne.Abstractions.Model;
using Services.AdministratorOne.Adapter;
using Services.Applications.Validation;
using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Services.Applications.Tests
{
    public class ProductTwoTests
    {
        [Fact]
        public async Task Application_for_ProductOne_creates_Investor_in_AdministratorTwo()
        {
            Guid investorId = Guid.NewGuid();
            Guid accountId = Guid.NewGuid();
            Guid paymentId = Guid.NewGuid();
            var bus = TestHelper.GetDefaultBus();
            var kycService = TestHelper.GetKycServiceAlwaysValid(Guid.Empty);


            var mockAdministrationService1 = new Mock<AdministratorOne.Abstractions.IAdministrationService>();
            var administrationService1 = mockAdministrationService1.Object;

            var mockAdministrationService2 = new Mock<AdministratorTwo.Abstractions.IAdministrationService>();
            mockAdministrationService2.Setup(a => a.CreateInvestorAsync(It.IsAny<User>())).Returns(Task.FromResult(new Result<Guid>(true, null, investorId)));
            mockAdministrationService2.Setup(a => a.CreateAccountAsync(It.IsAny<Guid>(), ProductCode.ProductTwo)).Returns(Task.FromResult(new Result<Guid>(true, null, accountId)));
            mockAdministrationService2.Setup(a => a.ProcessPaymentAsync(It.IsAny<Guid>(), It.IsAny<Payment>())).Returns(Task.FromResult(new Result<Guid>(true, null, paymentId)));
            var administrationService2 = mockAdministrationService2.Object;


            var applicationAdapterManager = TestHelper.InitDefaultApplicationAdapterManager();

            Configurator.Configure(applicationAdapterManager, bus, administrationService1, administrationService2);

            var applicationProcessor = new ApplicationProcessor(applicationAdapterManager, kycService, bus);

            var defaultApplication = TestHelper.GetDefaultApplicationProduct2(); 
            await applicationProcessor.Process(defaultApplication);
            mockAdministrationService1.VerifyAll();
        }


        [Fact]
        public async Task Application_for_ProductOne_creates_Investor_in_AdministratorTwo_But_ProductNotFound()
        {
            Guid investorId = Guid.NewGuid();
            Guid accountId = Guid.NewGuid();
            Guid paymentId = Guid.NewGuid();
            var bus = TestHelper.GetDefaultBus();
            var kycService = TestHelper.GetKycServiceAlwaysValid(Guid.Empty);

            var mockAdministrationService1 = new Mock<AdministratorOne.Abstractions.IAdministrationService>();
            var administrationService1 = mockAdministrationService1.Object;

            var mockAdministrationService2 = new Mock<AdministratorTwo.Abstractions.IAdministrationService>();
            mockAdministrationService2.Setup(a => a.CreateInvestorAsync(It.IsAny<User>())).Returns(Task.FromResult(new Result<Guid>(true, null, investorId)));
            mockAdministrationService2.Setup(a => a.CreateAccountAsync(It.IsAny<Guid>(), ProductCode.ProductTwo)).Returns(Task.FromResult(new Result<Guid>(true, null, accountId)));
            mockAdministrationService2.Setup(a => a.ProcessPaymentAsync(It.IsAny<Guid>(), It.IsAny<Payment>())).Returns(Task.FromResult(new Result<Guid>(true, null, paymentId)));
            var administrationService2 = mockAdministrationService2.Object;


            var applicationAdapterManager = TestHelper.InitDefaultApplicationAdapterManager();

            applicationAdapterManager.Register(ProductCode.ProductOne, new ApplicationAdapterProductOne(bus, administrationService1), new List<IValidationParameters>());

            var applicationProcessor = new ApplicationProcessor(applicationAdapterManager, kycService, bus);

            var defaultApplication = TestHelper.GetDefaultApplicationProduct2();

            var expectedMessage = "Product code not found";

            var exception = await Assert.ThrowsAsync<Exception>(() =>
                applicationProcessor.Process(defaultApplication));
            Assert.Equal(expectedMessage, exception.Message);
        }

        [Fact]
        public async Task Application_for_ProductTwo_creates_Investor_in_AdministratorOne_Bus_Method_Should_be_invoked()
        {
            var defaultApplication = TestHelper.GetDefaultApplicationProduct2();

            Guid guid = defaultApplication.Id;
            Guid investorId = Guid.NewGuid();
            Guid accountId = Guid.NewGuid();
            Guid paymentId = Guid.NewGuid();

            var mockBus = new Mock<IBus>();
            mockBus.Setup(a => a.PublishAsync(It.Is<InvestorCreated>(req => req.InvestorId == investorId.ToString())));
            mockBus.Setup(a => a.PublishAsync(It.Is<AccountCreated>(req => req.AccountId == accountId.ToString())));
            mockBus.Setup(a => a.PublishAsync(It.Is<ApplicationCompleted>(req => req.ApplicationId == guid)));
            var bus = mockBus.Object;

            var kycService = TestHelper.GetKycServiceAlwaysValid(Guid.Empty);

            var mockAdministrationService1 = new Mock<AdministratorOne.Abstractions.IAdministrationService>();
            var administrationService1 = mockAdministrationService1.Object;

            var mockAdministrationService2 = new Mock<AdministratorTwo.Abstractions.IAdministrationService>();
            mockAdministrationService2.Setup(a => a.CreateInvestorAsync(It.IsAny<User>())).Returns(Task.FromResult(new Result<Guid>(true, null, investorId)));
            mockAdministrationService2.Setup(a => a.CreateAccountAsync(It.IsAny<Guid>(), ProductCode.ProductTwo)).Returns(Task.FromResult(new Result<Guid>(true, null, accountId)));
            mockAdministrationService2.Setup(a => a.ProcessPaymentAsync(It.IsAny<Guid>(), It.IsAny<Payment>())).Returns(Task.FromResult(new Result<Guid>(true, null, paymentId)));
            var administrationService2 = mockAdministrationService2.Object;


            var applicationAdapterManager = TestHelper.InitDefaultApplicationAdapterManager();

            Configurator.Configure(applicationAdapterManager, bus, administrationService1, administrationService2);

            var applicationProcessor = new ApplicationProcessor(applicationAdapterManager, kycService, bus);


            await applicationProcessor.Process(defaultApplication);

            mockBus.VerifyAll();

        }


        [Fact]
        public async Task Application_for_ProductOne_creates_Investor_in_AdministratorTwoValidationNotPassed()
        {
            Guid investorId = Guid.NewGuid();
            Guid accountId = Guid.NewGuid();
            Guid paymentId = Guid.NewGuid();
            var bus = TestHelper.GetDefaultBus();
            var kycService = TestHelper.GetKycServiceAlwaysValid(Guid.Empty);


            var mockAdministrationService1 = new Mock<AdministratorOne.Abstractions.IAdministrationService>();
            var administrationService1 = mockAdministrationService1.Object;

            var mockAdministrationService2 = new Mock<AdministratorTwo.Abstractions.IAdministrationService>();
            mockAdministrationService2.Setup(a => a.CreateInvestorAsync(It.IsAny<User>())).Returns(Task.FromResult(new Result<Guid>(true, null, investorId)));
            mockAdministrationService2.Setup(a => a.CreateAccountAsync(It.IsAny<Guid>(), ProductCode.ProductTwo)).Returns(Task.FromResult(new Result<Guid>(true, null, accountId)));
            mockAdministrationService2.Setup(a => a.ProcessPaymentAsync(It.IsAny<Guid>(), It.IsAny<Payment>())).Returns(Task.FromResult(new Result<Guid>(true, null, paymentId)));
            var administrationService2 = mockAdministrationService2.Object;


            var applicationAdapterManager = TestHelper.InitDefaultApplicationAdapterManager();

            Configurator.Configure(applicationAdapterManager, bus, administrationService1, administrationService2);

            var applicationProcessor = new ApplicationProcessor(applicationAdapterManager, kycService, bus);

            var defaultApplication = new Application()
            {
                Applicant = new User()
                {
                    DateOfBirth = DateOnly.FromDateTime(DateTime.Now)
                },
                Payment = new Payment(new BankAccount(), new Money("GBP", 1000))
            };

            var exception = await Assert.ThrowsAsync<AggregateException>(() =>
                applicationProcessor.Process(defaultApplication));
        }

        [Fact]
        public async Task Application_for_ProductOne_creates_Investor_in_AdministratorTwoValidationNotPassed_NotEnoughMoney()
        {
            Guid investorId = Guid.NewGuid();
            Guid accountId = Guid.NewGuid();
            Guid paymentId = Guid.NewGuid();
            var bus = TestHelper.GetDefaultBus();
            var kycService = TestHelper.GetKycServiceAlwaysValid(Guid.Empty);


            var mockAdministrationService1 = new Mock<AdministratorOne.Abstractions.IAdministrationService>();
            var administrationService1 = mockAdministrationService1.Object;

            var mockAdministrationService2 = new Mock<AdministratorTwo.Abstractions.IAdministrationService>();
            mockAdministrationService2.Setup(a => a.CreateInvestorAsync(It.IsAny<User>())).Returns(Task.FromResult(new Result<Guid>(true, null, investorId)));
            mockAdministrationService2.Setup(a => a.CreateAccountAsync(It.IsAny<Guid>(), ProductCode.ProductTwo)).Returns(Task.FromResult(new Result<Guid>(true, null, accountId)));
            mockAdministrationService2.Setup(a => a.ProcessPaymentAsync(It.IsAny<Guid>(), It.IsAny<Payment>())).Returns(Task.FromResult(new Result<Guid>(true, null, paymentId)));
            var administrationService2 = mockAdministrationService2.Object;


            var applicationAdapterManager = TestHelper.InitDefaultApplicationAdapterManager();

            Configurator.Configure(applicationAdapterManager, bus, administrationService1, administrationService2);

            var applicationProcessor = new ApplicationProcessor(applicationAdapterManager, kycService, bus);

            var defaultApplication = new Application()
            {
                Applicant = new User()
                {
                    DateOfBirth = DateOnly.FromDateTime(DateTime.Now).AddYears(-30)
                },
                Payment = new Payment(new BankAccount(), new Money("GBP", (decimal)0.2))
            };

            var exception = await Assert.ThrowsAsync<AggregateException>(() =>
                applicationProcessor.Process(defaultApplication));
        }
    }



}
