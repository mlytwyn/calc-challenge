using calc_challenge.Models;
using System;

namespace calc_challenge.Services
{
    public class RequirementsService(ICalculatorConfigurationService calcConfiguration) : IRequirementsService
    {
        readonly ICalculatorConfigurationService _calcConfiguration = calcConfiguration;

        // The service uses config data stored in config.json to perform the following
        // 1) Determine the delimiter used and parse the numbers into an array.
        // 2) Enforce the maximum amount of digits allowed.
        public List<int> RequirementsCheck(string input)
        {
            var settings = _calcConfiguration.GetAllSettings();
            string[] inputNumbers = input.Split(settings?.Delimiters?.FirstOrDefault());
            List<int> parsedInputNumbers = [];

            try
            {
                CheckLength(inputNumbers.Length, settings);
                CheckValues(parsedInputNumbers, inputNumbers);
                return parsedInputNumbers;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return parsedInputNumbers;
            }
        }

        public bool CheckLength(int length, Settings? settings)
        {
            if (length > settings?.MaxDigits && settings.MaxDigits != 0)
                throw new Exception($"You have entered more than {settings.MaxDigits} digits");

            return true;
        }

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
    }

    public interface IRequirementsService
    {
        public List<int> RequirementsCheck(string input);
    }
}
