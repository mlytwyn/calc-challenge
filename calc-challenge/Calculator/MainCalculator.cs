using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using calc_challenge.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace calc_challenge.Calculator
{
    public class MainCalculator(IRequirementsService requirementsService) : ICalculator
    {
        // Using DI for the requirements & configuration services.
        private readonly IRequirementsService _requirementsService = requirementsService;

        // Core loop of the calculator function, record input and send to requirements service to be parsed.
        // End result is sent back to calculator for numeric operations.
        public void CalculatorMain()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Enter a series of numbers to find the sum.");
                Console.WriteLine($"The numbers may be delimited by a comma or newline character.");
                Console.WriteLine($"You may also define custom delimiters (See ReadMe)");
                string? userInput = Console.ReadLine();

                if (userInput != null)
                    try
                    {
                        Console.WriteLine("Total: " + Sum(_requirementsService.RequirementsCheck(userInput)));
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"{ex.Message}");
                    }
                    
                else Console.WriteLine("Invalid Value Entered");
            }
        }

        // Add the end result of numbers together to fund the sum.
        public int Sum(List<int> numbers)
        {
            int sum = 0;
            foreach (int number in numbers)
            {
                sum += number;
            }

            return sum;
        }
    }

    public interface ICalculator
    {
        void CalculatorMain();
        int Sum(List<int> numbers);
    }
}
