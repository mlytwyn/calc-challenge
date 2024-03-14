using calc_challenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace calc_challenge.Helpers
{
    /// <summary>
    /// Helper logic that will parse strings for information needed.
    /// </summary>
    public class StringParser : IStringParser
    {
        // Given a REGEX pattern, will return a single matching character
        public string RegexMatchSingleChar(string input, string pattern)
        {
            Match match = Regex.Match(input, pattern);
            char delilmiter = '\0';

            if (match.Success)
            {
                delilmiter = match.Groups[1].Value[0];
            }

            return delilmiter.ToString();
        }

        // Given a REGEX pattern, will return matching multiple strings
        public List<string> RegexMatchMultipleString(string input, string pattern)
        {
            MatchCollection matches = Regex.Matches(input, pattern);
            List<string> delimiters = [];

            foreach (Match multiMatch in matches.Cast<Match>())
            {
                string customDelimiter = multiMatch.Groups[1].Value;
                delimiters.Add(customDelimiter);
            }

            return delimiters;
        }

        // Given an array of strings, will parse out each value into a number.
        public List<int> ParseStringForNumbers(string[] inputNumbers)
        {
            List<int> parsedInputNumbers = [];

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

            return parsedInputNumbers;
        }
    }

    public interface IStringParser
    {
        public string RegexMatchSingleChar(string input, string pattern);
        public List<string> RegexMatchMultipleString(string input, string pattern);
        public List<int> ParseStringForNumbers(string[] inputNumbers);
    }
}
