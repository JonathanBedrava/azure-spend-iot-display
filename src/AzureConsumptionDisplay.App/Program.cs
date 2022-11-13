using AzureConsumptionDisplay.App;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var config = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.json")
    .Build();

var costRetriever = new CostRetriever(new(config["AZURE_APP_ID"], config["AZURE_APP_PASSWORD"], config["AZURE_APP_TENANT"], config["AZURE_SUBSCRIPTION_ID"]));
var cost = await costRetriever.GetCostResult();
var displayUpdater = new DisplayUpdater();
await displayUpdater.UpdateDisplay(cost.CurrentCost);