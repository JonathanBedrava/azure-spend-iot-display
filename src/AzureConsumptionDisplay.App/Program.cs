using AzureConsumptionDisplay.App;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

//curl -X POST -d 'grant_type=client_credentials&client_id=[APP_ID]&client_secret=[PASSWORD]&resource=https%3A%2F%2Fmanagement.azure.com%2F' https://login.microsoftonline.com/[TENANT_ID]/oauth2/token
var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");;;
var config = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

var costRetriever = new CostRetriever(new(config["AZURE_APP_ID"], config["AZURE_APP_PASSWORD"], config["AZURE_APP_TENANT"], config["AZURE_SUBSCRIPTION_ID"]));
var cost = await costRetriever.GetCostResult();
Console.WriteLine(JsonConvert.SerializeObject(cost));