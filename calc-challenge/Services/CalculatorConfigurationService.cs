using Microsoft.Extensions.Configuration;
using calc_challenge.Models;

namespace calc_challenge.Services
{
    public class CalculatorConfigurationService : ICalculatorConfigurationService
    {
        readonly IConfiguration? _configuration;

        /// <summary>
        /// Read configurable data from config.json file in root directory of project. 
        /// This holds values that can be customized within the application without needing code changes
        /// </summary>
        public CalculatorConfigurationService()
        {
            _configuration = new ConfigurationBuilder()
                   .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                   .AddJsonFile("config.json", optional: false, reloadOnChange: true)
                   .Build();
        }

        /// <summary>
        /// Get all settings from configuration file
        /// </summary>
        /// <returns></returns>
        public Settings GetCalculatorSettings()
        {
            return new Settings
            {
                Delimiters = GetDelimiters(),
                MaxDigits = GetMaxDigits(),
                AllowNegativeDigits = GetAllowNegativeNumbers(),
                MaxNumberSize = GetMaxNumberSize()

            };
        }

        /// <summary>
        /// Get delimiters from settings file
        /// </summary>
        /// <returns></returns>
        public List<string> GetDelimiters()
        {
            List<string> delimiters = [];

            if (_configuration != null)
            {
                var returnValue = _configuration.GetSection("CalcSettings:Delimiter").Get<string[]>();
                if (returnValue != null)
                    delimiters.AddRange(returnValue);

            }

            return delimiters;
        }

        /// <summary>
        /// Get max number of digits specified from configuration file
        /// </summary>
        /// <returns></returns>
        public int GetMaxDigits()
        {
            int maxDigits = 0;

            if (_configuration != null)
                maxDigits = _configuration.GetSection("CalcSettings:MaxDigits").Get<int>();

            return maxDigits; ;
        }

        /// <summary>
        /// Get bool to allow/disallow negative numbers
        /// </summary>
        /// <returns></returns>
        public bool GetAllowNegativeNumbers()
        {
            var negativeNumbersAllowed = false;

            if (_configuration != null)
                negativeNumbersAllowed = _configuration.GetSection("CalcSettings:AllowNegativeNumbers").Get<bool>();

            return negativeNumbersAllowed; ;
        }

        /// <summary>
        /// Get maximum number size allowed for user input, e.g. 100, 1000, 10000
        /// </summary>
        /// <returns></returns>
        public int GetMaxNumberSize()
        {
            int maxNumberSize = 0;

            if (_configuration != null)
                maxNumberSize = _configuration.GetSection("CalcSettings:MaxNumberSize").Get<int>();

            return maxNumberSize;
        }
    }

    public interface ICalculatorConfigurationService
    {
        List<string> GetDelimiters();
        int GetMaxDigits();
        Settings GetCalculatorSettings();
    }
}
