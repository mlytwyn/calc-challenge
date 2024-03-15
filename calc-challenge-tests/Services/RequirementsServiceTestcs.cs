using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using calc_challenge.Helpers;
using calc_challenge.Models;
using calc_challenge.Services;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace calc_challenge_tests.Services
{
    internal class RequirementsServiceTests
    {
        readonly Mock<ICalculatorConfigurationService> mockConfigurationService;
        readonly Mock<IStringParser> mockStringParser;
        readonly Settings mockConfigData;

        public RequirementsServiceTests()
        {
            mockConfigurationService = new Mock<ICalculatorConfigurationService>();
            mockStringParser = new Mock<IStringParser>();
            mockConfigData = new Settings { Delimiters = [",", "\\n"], MaxDigits = 0, AllowNegativeDigits = false, MaxNumberSize = 1000 };
        }

        [Test]
        // Test that the calculator functions normally when we don't exceed the maximum number of digits.
        public void CheckLength_UnderLimit()
        {
            var mockConfigData = new Settings { Delimiters = [","], MaxDigits = 2 };

            mockConfigurationService.Setup(ds => ds.GetCalculatorSettings()).Returns(mockConfigData);

            var requirementsService = new RequirementsService(mockConfigurationService.Object, mockStringParser.Object);
            Assert.That(requirementsService.CheckTotalDigits(2, mockConfigData), Is.True);
        }

        [Test]
        // Test that the max limit for numbers is not exceeded if the option is set.
        public void CheckLength_OverLimit()
        {
            var mockConfigData = new Settings { Delimiters = [","], MaxDigits = 2 };
            mockConfigurationService.Setup(ds => ds.GetCalculatorSettings()).Returns(mockConfigData);

            var requirementsService = new RequirementsService(mockConfigurationService.Object, mockStringParser.Object);
            Assert.Throws<Exception>(() => requirementsService.CheckTotalDigits(4, mockConfigData));
        }

        [Test]
        // Test that the number of values supplied does not exceed the enforced limit.
        public void CheckLength_NoLimit()
        {
            var mockConfigData = new Settings { Delimiters = [","], MaxDigits = 0 };
            mockConfigurationService.Setup(ds => ds.GetCalculatorSettings()).Returns(mockConfigData);

            var requirementsService = new RequirementsService(mockConfigurationService.Object, mockStringParser.Object);
            Assert.That(requirementsService.CheckTotalDigits(2, mockConfigData), Is.True);
        }

        [Test]
        // Test that we receive a valid result when calling ParseValues given valid input with multiple delimiters
        public void ParseValues_ValidResult()
        {
            var mockConfigData = new Settings { Delimiters = [",", "\\n"], MaxDigits = 0 };
            mockConfigurationService.Setup(ds => ds.GetCalculatorSettings()).Returns(mockConfigData);

            var requirementsService = new RequirementsService(mockConfigurationService.Object, mockStringParser.Object);
            var result = requirementsService.ParseValues("3,5,6\\n5", mockConfigData);

            Assert.That(result.Count(), Is.EqualTo(4));
        }

        [Test]
        // Test that we throw an exception if a negative value is entered.
        public void ForceNumericValues_Exception()
        {
            List<int> numberList = [1, 3, -5, 6, -8];
            string[] numberArray = ["1", "3", "-5", "6", "-8"];

            mockConfigurationService.Setup(ds => ds.GetCalculatorSettings()).Returns(mockConfigData);
            mockStringParser.Setup(ds => ds.ParseStringForNumbers(numberArray)).Returns(numberList);

            var requirementsService = new RequirementsService(mockConfigurationService.Object, mockStringParser.Object);
            Assert.Throws<Exception>(() => requirementsService.ForceNumericValues(numberArray, mockConfigData));
        }

        [Test]
        // Test that we receive a valid result when parsing valid input when calling ForceNumericValues()
        public void ForceNumericValues_ValidResult()
        {
            List<int> numberList = [1, 3, 5, 6, 8];
            string[] numberArray = ["1", "3", "5", "6", "8"];

            mockConfigurationService.Setup(ds => ds.GetCalculatorSettings()).Returns(mockConfigData);
            mockStringParser.Setup(ds => ds.ParseStringForNumbers(numberArray)).Returns(numberList);

            var requirementsService = new RequirementsService(mockConfigurationService.Object, mockStringParser.Object);
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

            mockConfigurationService.Setup(ds => ds.GetCalculatorSettings()).Returns(mockConfigData);
            mockStringParser.Setup(ds => ds.ParseStringForNumbers(numberArray)).Returns(numberList);

            var requirementsService = new RequirementsService(mockConfigurationService.Object, mockStringParser.Object);
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

            mockConfigurationService.Setup(ds => ds.GetCalculatorSettings()).Returns(mockConfigData);
            mockStringParser.Setup(ds => ds.ParseStringForNumbers(numberArray)).Returns(numberList);

            var requirementsService = new RequirementsService(mockConfigurationService.Object, mockStringParser.Object);
            var requirements = requirementsService.ForceNumericValues(numberArray, mockConfigData);
            var sum = requirements.Sum();

            Assert.Multiple(() =>
            {
                Assert.That(sum, Is.EqualTo(numberList.Sum()));
                Assert.That(requirements.Count, Is.EqualTo(numberList.Count()));
            });
        }

        [Test]
        // Test that if entering a new 1 char delimiter, it is stored correctly.
        public void StoreOptionalDelimiterChar_Valid()
        {
            char delimiter = '#';
            string input = "//#";
            string singleCharacterPattern = @"//(.)";

            Mock<ICalculatorConfigurationService> mockConfigurationService = new Mock<ICalculatorConfigurationService>();
            mockConfigurationService.Setup(ds => ds.GetCalculatorSettings()).Returns(mockConfigData);
            mockStringParser.Setup(ds => ds.RegexMatchSingleChar(input, singleCharacterPattern)).Returns(delimiter.ToString());

            var requirementsService = new RequirementsService(mockConfigurationService.Object, mockStringParser.Object);
            var result = requirementsService.StoreOptionalDelimiter("//#");

            Assert.That(result, Does.Contain(delimiter.ToString()));
        }

        [Test]
        // Test that if entering a new multi character delimiter, they is stored correctly.
        public void StoreOptionalDelimiterString_Valid()
        {
            string delimiter = "#*#";
            string input = "//[#*#]";
            string multipleCharacterPattern = @"\[(.*?)\]";
            List<string> delimiters = new List<string> { delimiter };

            Mock<ICalculatorConfigurationService> mockConfigurationService = new Mock<ICalculatorConfigurationService>();
            mockConfigurationService.Setup(ds => ds.GetCalculatorSettings()).Returns(mockConfigData);
            mockStringParser.Setup(ds => ds.RegexMatchMultipleString(input, multipleCharacterPattern)).Returns(delimiters);

            var requirementsService = new RequirementsService(mockConfigurationService.Object, mockStringParser.Object);
            var result = requirementsService.StoreOptionalDelimiter("//[#*#]");

            Assert.That(result, Does.Contain(delimiter));
        }

        [Test]
        // Test that if entering multiple multi-character delimiter, they is stored correctly.
        public void StoreOptionalDelimitersMultipleString_Valid()
        {
            string delimiterOne = "#*#";
            string delimiterTwo = "*#*";
            string input = "//[#*#][*#*]";
            string multipleCharacterPattern = @"\[(.*?)\]";
            List<string> delimiters = new List<string> { delimiterOne, delimiterTwo };

            Mock<ICalculatorConfigurationService> mockConfigurationService = new Mock<ICalculatorConfigurationService>();
            mockConfigurationService.Setup(ds => ds.GetCalculatorSettings()).Returns(mockConfigData);
            mockStringParser.Setup(ds => ds.RegexMatchMultipleString(input, multipleCharacterPattern)).Returns(delimiters);

            var requirementsService = new RequirementsService(mockConfigurationService.Object, mockStringParser.Object);
            var result = requirementsService.StoreOptionalDelimiter("//[#*#][*#*]");

            Assert.Multiple(() =>
            {
                Assert.That(result, Does.Contain(delimiterOne));
                Assert.That(result, Does.Contain(delimiterTwo));
            });
        }
    }
}
