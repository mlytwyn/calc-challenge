using calc_challenge.Calculator;
using calc_challenge.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;

// Register DI for Parser service
var serviceProvider = new ServiceCollection()
    .AddTransient<ICalculatorConfigurationService, CalculatorConfigurationService>()
    .AddTransient<IRequirementsService, RequirementsService>()
    .BuildServiceProvider();

var requirementsService = serviceProvider.GetRequiredService<IRequirementsService>();
var calculator = new MainCalculator(requirementsService);

calculator.CalculatorMain();