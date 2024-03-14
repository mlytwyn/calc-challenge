using calc_challenge.Models;
using System;
using System.Text.RegularExpressions;

namespace calc_challenge.Services
{
    public class RequirementsService(ICalculatorConfigurationService calcConfiguration) : IRequirementsService
    {
        readonly ICalculatorConfigurationService _calcConfiguration = calcConfiguration;

        // The service uses config data stored in config.json to perform the following
        // 1) Determine the delimiters used and parse the numbers into an array.
        // 2) Enforce the maximum amount of digits allowed if configured.
        // 3 Return values to be calculated once requirements are met
        public List<int> RequirementsCheck(string input)
        {
            List<int> parsedInputNumbersList = [];
            string[] parsedInputNumbersArray = [];
            Settings _settings = _calcConfiguration.GetCalculatorSettings();

            StoreOptionalDelimiter(input, _settings);
            parsedInputNumbersArray = ParseValues(input, _settings);
            CheckTotalDigits(parsedInputNumbersArray.Length, _settings);
            parsedInputNumbersList = ForceNumericValues(parsedInputNumbersArray, _settings);
           
            return parsedInputNumbersList;
        }

        // Check the number of digits entered against the maximum number allowed, if configured in config.json
        public bool CheckTotalDigits(int length, Settings settings)
        {
            if (length > settings?.MaxDigits && settings.MaxDigits != 0)
                throw new Exception($"\nYou have entered more than {settings.MaxDigits} digits");

            return true;
        }

        // Check the values entered to ensure they are numeric. If not, values are stored as 0
        // If negative numbers are disallowed, record them and throw exception.
        public List<int> ForceNumericValues(string[] inputNumbers, Settings settings)
        {
            List<int> parsedInputNumbers = [];
            foreach (string stringNumber in inputNumbers)
            {
                if (int.TryParse(stringNumber, out int number))
                {
                    if (number < settings.MaxNumberSize)
                        parsedInputNumbers.Add(number);
                }
                else
                {
                    parsedInputNumbers.Add(0);
                }
            }

            if (!settings.AllowNegativeDigits)
            {
                var negativeNumbers = parsedInputNumbers.Where(num => num < 0);
                if (negativeNumbers.Any())
                {
                    throw new Exception($"\nYou have entered the following negative numbers which are not allowed " +
                        $"{string.Join(",", negativeNumbers)} ");
                }
            }

            return parsedInputNumbers;
        }

        // Create an array of all numbers after splitting from known delimiters. Remove empty strings after completion.
        // Return as a List of integers
        public string[] ParseValues(string input, Settings settings)
        {
            var delimiters = settings?.Delimiters?.Select(del => del);
            string[] inputNumbers = input.Split(delimiters?.SelectMany(del => del).ToArray());
            return inputNumbers.Where(num => num.Length > 0).ToArray();
        }

        // If an optional delimiter is specificed in the input, make note of it here.
        public char StoreOptionalDelimiter(string input, Settings settings)
        {
            // Regex to get 1 character after two forward slashes
            string pattern = @"//(.)";
            char capturedChar = '\0';

            Match match = Regex.Match(input, pattern);
            if (match.Success)
            {
                // Extract the new delmiter and store it in our settings
                capturedChar = match.Groups[1].Value[0];
                settings.Delimiters?.Add(capturedChar.ToString());
            }

            return capturedChar;
        }
    }

    public interface IRequirementsService
    {
        public List<int> RequirementsCheck(string input);
    }
}
