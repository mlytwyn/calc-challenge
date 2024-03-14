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
        // Test that the calculator functions normally when we don't exceed the maximum number of digits.
        public void CheckLength_UnderLimit()
        {
            var mockConfigData = new Settings { Delimiters = [","], MaxDigits = 2 };
            Mock<ICalculatorConfigurationService> mockConfigurationService = new Mock<ICalculatorConfigurationService>();
            mockConfigurationService.Setup(ds => ds.GetCalculatorSettings()).Returns(mockConfigData);

            var requirementsService = new RequirementsService(mockConfigurationService.Object);
            Assert.That(requirementsService.CheckTotalDigits(2, mockConfigData), Is.True);
        }

        [Test]
        // Test that the max limit for numbers is not exceeded if the option is set.
        public void CheckLength_OverLimit()
        {
            var mockConfigData = new Settings { Delimiters = [","], MaxDigits = 2 };
            Mock<ICalculatorConfigurationService> mockConfigurationService = new Mock<ICalculatorConfigurationService>();
            mockConfigurationService.Setup(ds => ds.GetCalculatorSettings()).Returns(mockConfigData);

            var requirementsService = new RequirementsService(mockConfigurationService.Object);
            Assert.Throws<Exception>(() => requirementsService.CheckTotalDigits(4, mockConfigData));
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
        // Test that we receive a valid result when calling ParseValues given valid input with multiple delimiters
        public void ParseValues_ValidResult()
        {
            var mockConfigData = new Settings { Delimiters = [",", "\\n"], MaxDigits = 0 };
            Mock<ICalculatorConfigurationService> mockConfigurationService = new Mock<ICalculatorConfigurationService>();
            mockConfigurationService.Setup(ds => ds.GetCalculatorSettings()).Returns(mockConfigData);

            var requirementsService = new RequirementsService(mockConfigurationService.Object);
            var result = requirementsService.ParseValues("3,5,6\\5", mockConfigData);

            Assert.That(result.Length, Is.EqualTo(4));
        }

        [Test]
        // Test that we throw an exception if a negative value is entered.
        public void ForceNumericValues_Exception()
        {
            List<int> numberList = [1, 3, -5, 6, -8];
            string[] numberArray = ["1", "3", "-5", "6", "-8"];

            var mockConfigData = new Settings { Delimiters = [",", "\\n"], MaxDigits = 0, AllowNegativeDigits = false };
            Mock<ICalculatorConfigurationService> mockConfigurationService = new Mock<ICalculatorConfigurationService>();
            mockConfigurationService.Setup(ds => ds.GetCalculatorSettings()).Returns(mockConfigData);

            var requirementsService = new RequirementsService(mockConfigurationService.Object);
            Assert.Throws<Exception>(() => requirementsService.ForceNumericValues(numberArray, mockConfigData));
        }

        [Test]
        // Test that we receive a valid result when parsing valid input when calling ForceNumericValues()
        public void ForceNumericValues_ValidResult()
        {
            List<int> numberList = [1, 3, 5, 6, 8];
            string[] numberArray = ["1", "3", "5", "6", "8"];

            var mockConfigData = new Settings { Delimiters = [",", "\\n"], MaxDigits = 0, AllowNegativeDigits = false , MaxNumberSize = 1000};
            Mock<ICalculatorConfigurationService> mockConfigurationService = new Mock<ICalculatorConfigurationService>();
            mockConfigurationService.Setup(ds => ds.GetCalculatorSettings()).Returns(mockConfigData);

            var requirementsService = new RequirementsService(mockConfigurationService.Object);
            var requirements = requirementsService.ForceNumericValues(numberArray, mockConfigData);
            var sum = requirements.Sum();
         
            Assert.Multiple(() =>
            {
                Assert.That(sum, Is.EqualTo(numberList.Sum()));
                Assert.That(requirements.Count, Is.EqualTo(numberList.Count()));
            });
        }

        [Test]
        // Test that invalid characters are correclty removed and replaced with 0 values, not affecting the sum
        public void ForceNumericValues_InvalidCharacter_ValidResult()
        {
            List<int> numberList = [1, 3, 5, 6, 8, 0];
            string[] numberArray = ["1", "3", "5", "6", "8", "a"];

            var mockConfigData = new Settings { Delimiters = [",", "\\n"], MaxDigits = 0, AllowNegativeDigits = false, MaxNumberSize = 1000 };
            Mock<ICalculatorConfigurationService> mockConfigurationService = new Mock<ICalculatorConfigurationService>();
            mockConfigurationService.Setup(ds => ds.GetCalculatorSettings()).Returns(mockConfigData);

            var requirementsService = new RequirementsService(mockConfigurationService.Object);
            var requirements = requirementsService.ForceNumericValues(numberArray, mockConfigData);
            var sum = requirements.Sum();
            var test = numberList.Sum();
            Assert.Multiple(() =>
            {
                Assert.That(sum, Is.EqualTo(numberList.Sum()));
                Assert.That(requirements.Count, Is.EqualTo(numberList.Count()));
            });
        }

        [Test]
        // Test that values over the specific max numerical value are ignored and don't affect the total sum.
        public void ForceNumericValues_OverMaxSize_ValidResult()
        {
            List<int> numberList = [1, 3, 5, 6, 8];
            string[] numberArray = ["1", "3", "5", "6", "8", "8000"];

            var mockConfigData = new Settings { Delimiters = [",", "\\n"], MaxDigits = 0, AllowNegativeDigits = false, MaxNumberSize = 1000 };
            Mock<ICalculatorConfigurationService> mockConfigurationService = new Mock<ICalculatorConfigurationService>();
            mockConfigurationService.Setup(ds => ds.GetCalculatorSettings()).Returns(mockConfigData);

            var requirementsService = new RequirementsService(mockConfigurationService.Object);
            var requirements = requirementsService.ForceNumericValues(numberArray, mockConfigData);
            var sum = requirements.Sum();

            Assert.Multiple(() =>
            {
                Assert.That(sum, Is.EqualTo(numberList.Sum()));
                Assert.That(requirements.Count, Is.EqualTo(numberList.Count()));
            });
        }

    }
}
