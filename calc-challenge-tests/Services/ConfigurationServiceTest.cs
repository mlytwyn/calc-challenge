﻿using calc_challenge.Models;
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
        // Test that we receive the correct settings back when calling them from the Calculator Configuration Service.
        public void GetAllSettings_Test()
        {
            var mockConfigData = new Settings { Delimiters = [","], MaxDigits = 0, AllowNegativeDigits = false, MaxNumberSize = 1000 };
            Mock<ICalculatorConfigurationService> mockConfigurationService = new Mock<ICalculatorConfigurationService>();
            mockConfigurationService.Setup(ds => ds.GetCalculatorSettings()).Returns(mockConfigData);

            var configurationService = new CalculatorConfigurationService();
            var calcSettings = configurationService.GetCalculatorSettings();

            Assert.Multiple(() =>
            {
                Assert.That(calcSettings?.Delimiters?.First(), Is.EqualTo(mockConfigData.Delimiters.First()));
                Assert.That(calcSettings?.MaxDigits, Is.EqualTo(mockConfigData.MaxDigits));
                Assert.That(calcSettings?.AllowNegativeDigits, Is.EqualTo(mockConfigData.AllowNegativeDigits));
                Assert.That(calcSettings?.MaxNumberSize, Is.EqualTo(mockConfigData.MaxNumberSize));
            });
        }
    }
}
