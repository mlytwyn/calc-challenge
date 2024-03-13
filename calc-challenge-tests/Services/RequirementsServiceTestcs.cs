using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using calc_challenge.Models;
using calc_challenge.Services;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace calc_challenge_tests.Services
{
    internal class RequirementsServiceTests
    {
        [Test]
        // Test that the number of values supplied does not exceed the enforced limit.
        public void CheckLength_UnderLimit()
        {
            var mockConfigData = new Settings { Delimiters = [","], MaxDigits = 2 };
            Mock<ICalculatorConfigurationService> mockConfigurationService = new Mock<ICalculatorConfigurationService>();
            mockConfigurationService.Setup(ds => ds.GetCalculatorSettings()).Returns(mockConfigData);

            var requirementsService = new RequirementsService(mockConfigurationService.Object);
            Assert.That(requirementsService.CheckTotalDigits(2, mockConfigData), Is.True);
        }

        [Test]
        // Test that the number of values supplied does not exceed the enforced limit.
        public void CheckLength_NoLimit()
        {
            var mockConfigData = new Settings { Delimiters = [","], MaxDigits = 0 };
            Mock<ICalculatorConfigurationService> mockConfigurationService = new Mock<ICalculatorConfigurationService>();
            mockConfigurationService.Setup(ds => ds.GetCalculatorSettings()).Returns(mockConfigData);

            var requirementsService = new RequirementsService(mockConfigurationService.Object);
            Assert.That(requirementsService.CheckTotalDigits(2, mockConfigData), Is.True);
        }

        [Test]
        // Test that the number of values supplied does not exceed the enforced limit.
        public void ParseValues_ValidResult()
        {
            var mockConfigData = new Settings { Delimiters = [",", "\\n"], MaxDigits = 0 };
            Mock<ICalculatorConfigurationService> mockConfigurationService = new Mock<ICalculatorConfigurationService>();
            mockConfigurationService.Setup(ds => ds.GetCalculatorSettings()).Returns(mockConfigData);

            var requirementsService = new RequirementsService(mockConfigurationService.Object);
            var result = requirementsService.ParseValues("3,5,6\\5", mockConfigData);

            Assert.Multiple(() =>
            {
                Assert.That(result.Length, Is.EqualTo(4));
            });
        }

    }
}
