using AzureConsumptionDisplay.App;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var config = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.local.json", true)
    .Build();

var displayUpdater = new DisplayUpdater();
try
{
    var costRetriever = new CostRetriever(
        new(config["AZURE_APP_ID"], config["AZURE_APP_PASSWORD"], config["AZURE_APP_TENANT"], config["AZURE_SUBSCRIPTION_ID"]),
        displayUpdater
    );
    var cost = await costRetriever.GetCostResult();
    await displayUpdater.UpdateDisplay(cost.CurrentCost);
}
catch
{
    await displayUpdater.SetErrorDisplay();
    throw;
}