using Microsoft.Extensions.Configuration;
using calc_challenge.Models;

namespace calc_challenge.Services
{
    public class CalculatorConfigurationService : ICalculatorConfigurationService
    {
        readonly IConfiguration? _configuration;

        // Read configurable data from config.json file in root directory of project. 
        // This holds values that can be customized within the application without needing code changes
        public CalculatorConfigurationService()
        {
            _configuration = new ConfigurationBuilder()
                   .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                   .AddJsonFile("config.json", optional: false, reloadOnChange: true)
                   .Build();
        }

        // Get all settings from configuration file
        public Settings GetAllSettings()
        {
            var settings = new Settings();
            settings.Delimiters = GetDelimiters();
            settings.MaxDigits = GetMaxDigits();
            return settings;
        }

        // Get delimiters from settings file
        public List<string> GetDelimiters()
        {
            List<string> delimiters = new List<string>();

            if (_configuration != null)
            {
                var returnValue = _configuration.GetSection("CalcSettings:Delimiter").Get<string[]>();
                if (returnValue != null)
                    delimiters.AddRange(returnValue);

            }

            return delimiters;
        }

        // Get max number of digits specified from configuration file
        public int GetMaxDigits()
        {
            int maxDigits = 0;

            if (_configuration != null)
                maxDigits = _configuration.GetSection("CalcSettings:MaxDigits").Get<int>();

            return maxDigits; ;
        }
    }

    public interface ICalculatorConfigurationService
    {
        List<string> GetDelimiters();
        int GetMaxDigits();
        Settings GetAllSettings();
    }
}
