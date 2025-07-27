
using System;
using FluentValidation.TestHelper;
using Xunit;
using FluentAssertions;
using Services.Applications.Validation.Validators;
using Services.Applications.Validation.Parameters;
using Services.Common.Abstractions.Model;
namespace Services.Applications.Tests
{


    public class AgeValidatorTests
    {
        [Theory]
        [InlineData(2008, 1, 1, 18, 65, false)] // Too young
        [InlineData(1940, 1, 1, 18, 65, false)] // Too old
        [InlineData(1990, 1, 1, 18, 65, true)]  // Valid age
        [InlineData(1990, 1, 1, 18, null, true)]  // Valid age
        [InlineData(2022, 1, 1, 18, null, false)]  // Valid age
        [InlineData(1990, 1, 1, null, null, true)] // No limits
        public void Validate_AgeRange_ShouldBehaveAsExpected(
            int year, int month, int day,
            int? minAge, int? maxAge,
            bool expectedIsValid)
        {
            // Arrange
            var parameters = new AgeValidatorParameters
            {
                MinAge = minAge,
                MaxAge = maxAge
            };

            var validator = new AgeValidator(parameters);

            var user = new User
            {
                DateOfBirth = new DateOnly(year, month, day)
            };

            // Act
            var result = validator.TestValidate(user);

            // Assert
            result.IsValid.Should().Be(expectedIsValid);
        }

        [Fact]
        public void Should_Set_Custom_ErrorMessage()
        {
            // Arrange
            var parameters = new AgeValidatorParameters
            {
                MinAge = 21,
                MaxAge = 30
            };

            var validator = new AgeValidator(parameters);
            var user = new User
            {
                DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-20)) // 20 years old
            };

            // Act
            var result = validator.Validate(user);

            // Assert
            result.Errors.Should().ContainSingle()
                .Which.ErrorMessage.Should().Be("User age must be between 21 and 30 years.");
        }
    }
}
