using calc_challenge.Models;
using calc_challenge.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace calc_challenge_tests.Services
{
    internal class ConfigurationServiceTest
    {

        [Test]
        // Test that the number of values supplied does not exceed the enforced limit.
        public void GetAllSettings_Test()
        {
            var mockConfigData = new Settings { Delimiters = [","], MaxDigits = 2 };
            Mock<ICalculatorConfigurationService> mockConfigurationService = new Mock<ICalculatorConfigurationService>();
            mockConfigurationService.Setup(ds => ds.GetAllSettings()).Returns(mockConfigData);

            var configurationService = new CalculatorConfigurationService();
            var calcSettings = configurationService.GetAllSettings();

            Assert.Multiple(() =>
            {
                Assert.That(calcSettings?.Delimiters?.First(), Is.EqualTo(mockConfigData.Delimiters.First()));
                Assert.That(calcSettings?.MaxDigits, Is.EqualTo(mockConfigData.MaxDigits));
            });
        }
    }
}
