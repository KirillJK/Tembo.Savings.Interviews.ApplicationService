using FluentAssertions;
using Moq;
using Services.AdministratorTwo.Abstractions;
using Services.AdministratorTwo.Adapter;
using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Error = Services.Common.Abstractions.Model.Error;

namespace Services.Applications.Tests
{
    public class ApplicationAdapterProductTwoTests
    {
        private readonly Mock<IBus> _busMock = new();
        private readonly Mock<IAdministrationService> _adminServiceMock = new();
        private readonly ApplicationAdapterProductTwo _adapter;

        public ApplicationAdapterProductTwoTests()
        {
            _adapter = new ApplicationAdapterProductTwo(_busMock.Object, _adminServiceMock.Object);
        }

        [Fact]
        public async Task Process_AllStepsSuccessful_ShouldPublishAllEvents()
        {
            // Arrange
            var application = TestHelper.GetDefaultApplicationProduct2();
            var investorId = Guid.NewGuid();
            var accountId = Guid.NewGuid();

            _adminServiceMock.Setup(x => x.CreateInvestorAsync(application.Applicant))
                .ReturnsAsync(Result<Guid>.Success(investorId));

            _adminServiceMock.Setup(x => x.CreateAccountAsync(investorId, ProductCode.ProductTwo))
                .ReturnsAsync(Result<Guid>.Success(accountId));

            _adminServiceMock.Setup(x => x.ProcessPaymentAsync(accountId, application.Payment))
                .Returns(Task.FromResult(new Result<Guid>(true, null, Guid.Empty)));

            // Act
            await _adapter.Process(application);

            // Assert
            _busMock.Verify(x => x.PublishAsync(It.IsAny<InvestorCreated>()), Times.Once);
            _busMock.Verify(x => x.PublishAsync(It.IsAny<AccountCreated>()), Times.Once);
            _busMock.Verify(x => x.PublishAsync(It.IsAny<ApplicationCompleted>()), Times.Once);
        }

        [Fact]
        public async Task Process_CreateInvestorFails_ShouldThrowException()
        {
            // Arrange
            var application = TestHelper.GetDefaultApplicationProduct2();
            var error = new Error("INVESTOR_ERR", "Investor creation failed", "Admin");

            _adminServiceMock.Setup(x => x.CreateInvestorAsync(application.Applicant))
                .Returns(Task.FromResult(new Result<Guid>(false, error, Guid.Empty)));

            // Act
            Func<Task> act = () => _adapter.Process(application);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage($"*{nameof(_adminServiceMock.Object.CreateInvestorAsync)}*{error.Code}*{error.Description}*");
        }

        [Fact]
        public async Task Process_CreateAccountFails_ShouldThrowException()
        {
            // Arrange
            var application = TestHelper.GetDefaultApplicationProduct2();
            var investorId = Guid.NewGuid();
            var error = new Error("ACCOUNT_ERR", "Account creation failed", "Admin");

            _adminServiceMock.Setup(x => x.CreateInvestorAsync(application.Applicant))
                .ReturnsAsync(Result<Guid>.Success(investorId));

            _adminServiceMock.Setup(x => x.CreateAccountAsync(investorId, ProductCode.ProductTwo))
                .Returns(Task.FromResult(new Result<Guid>(false, error, Guid.Empty)));

            // Act
            Func<Task> act = () => _adapter.Process(application);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage($"*{nameof(_adminServiceMock.Object.CreateAccountAsync)}*{error.Code}*{error.Description}*");
        }

        [Fact]
        public async Task Process_PaymentFails_ShouldThrowException()
        {
            // Arrange
            var application = TestHelper.GetDefaultApplicationProduct2();
            var investorId = Guid.NewGuid();
            var accountId = Guid.NewGuid();
            var error = new Error("PAYMENT_ERR", "Payment failed", "Admin");

            _adminServiceMock.Setup(x => x.CreateInvestorAsync(application.Applicant))
                .ReturnsAsync(Result<Guid>.Success(investorId));

            _adminServiceMock.Setup(x => x.CreateAccountAsync(investorId, ProductCode.ProductTwo))
                .ReturnsAsync(Result<Guid>.Success(accountId));

            _adminServiceMock.Setup(x => x.ProcessPaymentAsync(accountId, application.Payment))
                .Returns(Task.FromResult(new Result<Guid>(false, error, Guid.Empty)));

            // Act
            Func<Task> act = () => _adapter.Process(application);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage($"*{nameof(_adminServiceMock.Object.ProcessPaymentAsync)}*{error.Code}*{error.Description}*");
        }
    }
}
