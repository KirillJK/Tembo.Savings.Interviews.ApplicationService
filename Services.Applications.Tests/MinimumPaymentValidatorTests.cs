
using FluentValidation.TestHelper;
using Services.Applications.Validation;
using Services.Applications.Validation.Parameters;
using Services.Applications.Validation.Validators;
using Services.Common.Abstractions.Model;
using Xunit;

namespace Services.Applications.Tests
{


    public class MinimumPaymentValidatorTests
    {
        [Fact]
        public void Should_Have_Error_When_Amount_Is_Less_Than_Minimum()
        {
            // Arrange
            var parameters = new MinimumPaymentValidatorParameters { MinimumPayment = 100 };
            var validator = new MinimumPaymentValidator(parameters);

            var payment = new Payment(
                new BankAccount { SortCode = "12-34-56", AccountNumber = "12345678" },
                new Money("GBP", 50m)
            );

            // Act & Assert
            var result = validator.TestValidate(payment);
            result.ShouldHaveValidationErrorFor(p => p.Amount.Amount)
                  .WithErrorMessage("Payment amount must be at least 100");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Amount_Is_Greater_Than_Minimum()
        {
            var parameters = new MinimumPaymentValidatorParameters { MinimumPayment = 100 };
            var validator = new MinimumPaymentValidator(parameters);

            var payment = new Payment(
                new BankAccount { SortCode = "12-34-56", AccountNumber = "12345678" },
                new Money("GBP", 150m)
            );

            var result = validator.TestValidate(payment);
            result.ShouldNotHaveValidationErrorFor(p => p.Amount.Amount);
        }

        [Fact]
        public void Should_Not_Have_Error_When_MinimumPayment_Is_Null()
        {
            var parameters = new MinimumPaymentValidatorParameters { MinimumPayment = null };
            var validator = new MinimumPaymentValidator(parameters);

            var payment = new Payment(
                new BankAccount { SortCode = "12-34-56", AccountNumber = "12345678" },
                new Money("GBP", 0m)
            );

            var result = validator.TestValidate(payment);
            result.ShouldNotHaveValidationErrorFor(p => p.Amount.Amount);
        }

    }
}
