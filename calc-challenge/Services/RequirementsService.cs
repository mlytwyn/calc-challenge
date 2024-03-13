using calc_challenge.Models;
using System;

namespace calc_challenge.Services
{
    public class RequirementsService(ICalculatorConfigurationService calcConfiguration) : IRequirementsService
    {
        readonly ICalculatorConfigurationService _calcConfiguration = calcConfiguration;

        // The service uses config data stored in config.json to perform the following
        // 1) Determine the delimiter used and parse the numbers into an array.
        // 2) Enforce the maximum amount of digits allowed if configured.
        public List<int> RequirementsCheck(string input)
        {
            List<int> parsedInputNumbersList = [];
            string[] parsedInputNumbersArray = [];
            Settings settings = _calcConfiguration.GetCalculatorSettings();

            try
            {
                parsedInputNumbersArray = ParseValues(input, settings);
                CheckTotalDigits(parsedInputNumbersArray.Length, settings);
                CheckValues(parsedInputNumbersList, parsedInputNumbersArray);
                return parsedInputNumbersList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return parsedInputNumbersList;
            }
        }

        // Check the number of digits entered against the maximum number allowed, if configured in config.json
        public bool CheckTotalDigits(int length, Settings? settings)
        {
            if (length > settings?.MaxDigits && settings.MaxDigits != 0)
                throw new Exception($"You have entered more than {settings.MaxDigits} digits");

            return true;
        }

        // Check the values entered to ensure they are numeric. If not, values are stored as 0
        // List is passed by reference (default)
        public void CheckValues(List<int> parsedInputNumbers, string[] inputNumbers)
        {
            foreach (string stringNumber in inputNumbers)
            {
                if (int.TryParse(stringNumber, out int number))
                {
                    parsedInputNumbers.Add(number);
                }
                else
                {
                    parsedInputNumbers.Add(0);
                }
            }
        }

        // Create an array of all numbers after splitting from known delimiters. Remove empty strings after completion.
        // Return as a List of integers
        public string[] ParseValues(string input, Settings? settings)
        {
            var delimiters = settings?.Delimiters?.Select(del => del);
            string[] inputNumbers = input.Split(delimiters?.SelectMany(del => del).ToArray());
            return inputNumbers.Where(num => num.Length > 0).ToArray();
        }
    }

    public interface IRequirementsService
    {
        public List<int> RequirementsCheck(string input);
    }
}
