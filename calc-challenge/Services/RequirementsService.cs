﻿using calc_challenge.Helpers;
using calc_challenge.Models;
using System;
using System.Collections.Generic;

namespace calc_challenge.Services
{
    public class RequirementsService(ICalculatorConfigurationService calcConfiguration, IStringParser stringParser) : IRequirementsService
    {
        readonly ICalculatorConfigurationService _calcConfiguration = calcConfiguration;
        readonly IStringParser _stringParser = stringParser;

        /// <summary>
        /// The service uses config data stored in config.json and user input to perform the following
        /// 1) Determine the delimiters used and parse the numbers into an array.
        /// 2) Enforce the maximum amount of digits allowed if configured.
        /// 3) Return values to be calculated once requirements are met
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public List<int> RequirementsCheck(string input)
        {
            string[] parsedInputNumbersArray = [];
            Settings _settings = _calcConfiguration.GetCalculatorSettings();

            // Store any custom delimiters into our list of known delimiters.
            _settings.Delimiters.AddRange(StoreOptionalDelimiter(input));
            parsedInputNumbersArray = ParseValues(input, _settings);
            CheckTotalDigits(parsedInputNumbersArray.Length, _settings);

            return ForceNumericValues(parsedInputNumbersArray, _settings);
        }

        /// <summary>
        /// Check the number of digits entered against the maximum number allowed, if configured in config.json
        /// </summary>
        /// <param name="length"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool CheckTotalDigits(int length, Settings settings)
        {
            if (length > settings?.MaxDigits && settings.MaxDigits != 0)
                throw new Exception($"\nYou have entered more than {settings.MaxDigits} digits");

            return true;
        }

        /// <summary>
        /// Check the values entered to ensure they are numeric. If not, values are stored as 0
        /// If negative numbers are disallowed, record them and throw exception.
        /// </summary>
        /// <param name="inputNumbers"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<int> ForceNumericValues(string[] inputNumbers, Settings settings)
        {
            List<int> parsedInputNumbers = [];
            parsedInputNumbers = _stringParser.ParseStringForNumbers(inputNumbers);

            // Replace any numbers that are larger than the configurable maximum size in config.json with 0
            parsedInputNumbers = parsedInputNumbers.Select(num =>
            {
                if (num > settings.MaxNumberSize)
                    num = 0;

                return num;
            }).ToList();

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

        /// <summary>
        /// Create an array of all numbers after splitting from known delimiters. Remove empty strings after completion.
        /// Return as a List of integers
        /// </summary>
        /// <param name="input"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public string[] ParseValues(string input, Settings settings)
        {
            var delimiters = settings.Delimiters.ToArray();
            string[] splitInput = input.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            return splitInput.Where(num => num.Length > 0).ToArray();
        }

        /// <summary>
        /// If an optional delimiter is specificed in the input, make note of it here.
        /// Optionally these regex values could be moved to config.json to make them more tweakable without rebuilding
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public List<string> StoreOptionalDelimiter(string input)
        {
            var newDelimiters = new List<string>();

            // Regex to get 1 character after two forward slashes
            // Ex: //. , //* , //#
            string singleCharacterPattern = @"//(.)";

            //// Regex to get characters after two forward slashes and between two brackets
            //// Ex: //[*], //[*#*][#*#], //[******######][z8x]
            string multipleCharacterPattern = @"\[(.*?)\]";

            // If the input starts with // and we are not immediatly defining a delimiter in brackets.
            if (input.StartsWith("//") && input[2].ToString() != "[")
                newDelimiters.Add(_stringParser.RegexMatchSingleChar(input, singleCharacterPattern));

            // If the input starts with // and it contains an opening and closing bracket
            if (input.StartsWith("//") && input.Contains('[') && input.Contains(']'))
                newDelimiters.AddRange(_stringParser.RegexMatchMultipleString(input, multipleCharacterPattern));

            return newDelimiters;
        }
    }

    public interface IRequirementsService
    {
        public List<int> RequirementsCheck(string input);
    }
}
