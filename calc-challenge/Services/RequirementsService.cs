using calc_challenge.Helpers;
using calc_challenge.Models;
using System.Collections.Generic;

namespace calc_challenge.Services
{
    public class RequirementsService(ICalculatorConfigurationService calcConfiguration, IStringParser stringParser) : IRequirementsService
    {
        readonly ICalculatorConfigurationService _calcConfiguration = calcConfiguration;
        readonly IStringParser _stringParser = stringParser;

        // The service uses config data stored in config.json and user input to perform the following
        // 1) Determine the delimiters used and parse the numbers into an array.
        // 2) Enforce the maximum amount of digits allowed if configured.
        // 3) Return values to be calculated once requirements are met
        public List<int> RequirementsCheck(string input)
        {
            List<int> parsedInputNumbersList = [];
            string[] parsedInputNumbersArray = [];
            Settings _settings = _calcConfiguration.GetCalculatorSettings();

            _settings.Delimiters.AddRange(StoreOptionalDelimiter(input));
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
            parsedInputNumbers = _stringParser.ParseStringForNumbers(inputNumbers);

            // Remove any numbers that are larger than the configurable maximum size in config.json
            parsedInputNumbers.RemoveAll(num => num > settings.MaxNumberSize);

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
        // Optionally these regex values could be moved to config.json to make them more tweakable without rebuilding
        public List<string> StoreOptionalDelimiter(string input)
        {
            var newDelimiters = new List<string>();

            // Regex to get 1 character after two forward slashes
            // Ex: //. , //* , //#
            string singleCharacterPattern = @"//(.)";

            //// Regex to get characters after two forward slashes and between two brackets
            //// Ex: //[*], //[*#*][#*#], //[******######][z8x]
            string multipleCharacterPattern = @"\[(.*?)\]";

            if (input.Contains('[') && input.Contains(']') && input.StartsWith("/"))
                newDelimiters.AddRange(_stringParser.RegexMatchMultipleString(input, multipleCharacterPattern));

            if (input.Contains("//") && input.StartsWith('/'))
                newDelimiters.Add(_stringParser.RegexMatchSingleChar(input, singleCharacterPattern));

            return newDelimiters;
        }
    }

    public interface IRequirementsService
    {
        public List<int> RequirementsCheck(string input);
    }
}
