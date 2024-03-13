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
        public void CheckLength_Test()
        {
            var mockConfigData = new Settings { Delimiters = [","], MaxDigits = 2 };
            Mock<ICalculatorConfigurationService> mockConfigurationService = new Mock<ICalculatorConfigurationService>();
            mockConfigurationService.Setup(ds => ds.GetAllSettings()).Returns(mockConfigData);

            var requirementsService = new RequirementsService(mockConfigurationService.Object);
            var digits = mockConfigurationService.Object.GetMaxDigits();
            Assert.That(requirementsService.CheckLength(2, mockConfigData), Is.True);
        }
    }
}
