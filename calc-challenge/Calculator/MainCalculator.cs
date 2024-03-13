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

        // Core loop of the calculator function, take input and route appropriately
        public void Input()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Enter two numbers separated by a comma to find the sum.");
                string? userInput = Console.ReadLine();

                if (userInput != null)
                    Console.WriteLine("Total: " + Sum(_requirementsService.RequirementsCheck(userInput)));
                else Console.WriteLine("Invalid Value Entered");
            }
        }

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
        void Input();
        int Sum(List<int> numbers);
    }
}
