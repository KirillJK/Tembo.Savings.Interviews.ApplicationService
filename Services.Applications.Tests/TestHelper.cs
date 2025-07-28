using Moq;
using Newtonsoft.Json;
using Services.AdministratorOne.Abstractions.Model;
using Services.Applications.Validation;
using Services.Common.Abstractions.Abstractions;
using Services.Common.Abstractions.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Applications.Tests
{
    internal static class TestHelper
    {
        public static ApplicationAdapterManager InitDefaultApplicationAdapterManager()
        {
            IValidationFactory validationFactory = new ValidationFactory();
            ApplicationAdapterManager applicationAdapterManager = new ApplicationAdapterManager(validationFactory);
            return applicationAdapterManager;
        }


        public static IBus GetDefaultBus()
        {
            var mockBus = new Mock<IBus>();
            mockBus.Setup(a => a.PublishAsync(It.IsAny<DomainEvent>()));
            return mockBus.Object;
        }

        public static IKycService GetKycServiceAlwaysValid(Guid reportId)
        {
            Mock<IKycService> mockKycService = new Mock<IKycService>();
            Result<KycReport> result = new Result<KycReport>(true, null, new KycReport(reportId, true));
            mockKycService.Setup(a => a.GetKycReportAsync(It.IsAny<User>())).Returns(Task.FromResult(result));
            return mockKycService.Object;
        }

        public static IKycService GetKycServiceAlwaysInValid(Guid reportId)
        {
            Mock<IKycService> mockKycService = new Mock<IKycService>();
            Result<KycReport> result = new Result<KycReport>(true, null, new KycReport(reportId, false));
            mockKycService.Setup(a => a.GetKycReportAsync(It.IsAny<User>())).Returns(Task.FromResult(result));
            return mockKycService.Object;
        }


        public static JsonSerializerSettings GetDefaultJsonSettings()
        {
            var settings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateParseHandling = DateParseHandling.DateTime
            };
            return settings;
        }

        public static Application GetDefaultApplication()
        {
            return JsonConvert.DeserializeObject<Application>(File.ReadAllText("TestData/application.json"), GetDefaultJsonSettings());
        }

        public static Application GetDefaultApplicationProduct2()
        {
            return JsonConvert.DeserializeObject<Application>(File.ReadAllText("TestData/application2.json"), GetDefaultJsonSettings());
        }

        public static CreateInvestorRequest GetDefaultInvestorRequest()
        {
            return JsonConvert.DeserializeObject<CreateInvestorRequest>(File.ReadAllText("TestData/createinvestorrequest.json"), GetDefaultJsonSettings());
        }
        
        public static User GetUser()
        {
            return JsonConvert.DeserializeObject<User>(File.ReadAllText("TestData/user.json"), GetDefaultJsonSettings());
        }

        public static bool IsEquivalent(CreateInvestorRequest one, CreateInvestorRequest two)
        {
            var result =
                  one.FirstName == two.FirstName &&
                  one.LastName == two.LastName &&
                  one.DateOfBirth == two.DateOfBirth &&
                  one.Nino == two.Nino &&
                  one.Addressline1 == two.Addressline1 &&
                  one.Addressline2 == two.Addressline2 &&
                  one.Addressline3 == two.Addressline3 &&
                  one.Addressline4 == two.Addressline4 &&
                  one.PostCode == two.PostCode &&
                  one.Product == two.Product &&
                  one.SortCode == two.SortCode &&
                  one.AccountNumber == two.AccountNumber &&
                  one.InitialPayment == two.InitialPayment;
            return result;
        }

        public static bool IsEquivalent(User one, User two)
        {
            return one.Id == two.Id &&
                   one.IsVerified == two.IsVerified &&
                   one.Forename == two.Forename &&
                   one.Surname == two.Surname &&
                   one.DateOfBirth == two.DateOfBirth &&
                   one.Nino == two.Nino;
        }
        

    }
}
