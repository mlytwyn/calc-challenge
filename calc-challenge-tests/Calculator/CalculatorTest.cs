using calc_challenge.Models;
using calc_challenge.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using calc_challenge.Calculator;

namespace calc_challenge_tests.Calculator
{
    internal class CalculatorTest
    {

        [Test]
        // Test that the number of values supplied does not exceed the enforced limit.
        public void Sum_Test()
        {
            List<int> userInput = [1, 3];
            int sum = 4;

            Mock<IRequirementsService> mockRequirementsService = new Mock<IRequirementsService>();
            mockRequirementsService.Setup(ds => ds.RequirementsCheck("1,3"));

            var calculator = new MainCalculator(mockRequirementsService.Object);

            var calculatedSum = calculator.Sum(userInput);
            Assert.That(sum, Is.EqualTo(calculatedSum));
        }
    }
}
