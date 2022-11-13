using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using System.Text.Json;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;

namespace AzureConsumptionDisplay.App;
public class CostRetriever : ICostRetriever
{
    private readonly CostRetrieverSettings _settings;
    private readonly IDisplayUpdater _displayUpdater;
    private static readonly TimeSpan _pingTimeoutSeconds = TimeSpan.FromSeconds(60);
    private static readonly TimeSpan _pingInterval = TimeSpan.FromSeconds(3);
    private const string _networkTestIp = "8.8.8.8";
    public CostRetriever(CostRetrieverSettings settings, IDisplayUpdater displayUpdater)  
    {    
        _settings = settings;
        _displayUpdater = displayUpdater;
    }

    public async Task<CostResult> GetCostResult()
    {
        await WaitOnNetwork();
        var token = await GetAuthToken(_settings.AppId, _settings.Secret, _settings.TenantId);
        var current = await GetCurrentUsage(token, _settings.SubscriptionId);
        
        return new CostResult(current);
    }

    static async Task<decimal> GetCurrentUsage(string authToken, string subscriptionId)
    {
        var url = $"https://management.azure.com/subscriptions/{subscriptionId}/providers/Microsoft.CostManagement/query?api-version=2019-11-01";
        using(var client = new HttpClient())
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        var reqBody = BuildCostRequestBody();

        var response = await client.PostAsJsonAsync(url, reqBody);
        var body = await response.Content.ReadAsStringAsync();
        var parsedBody = JObject.Parse(body);
        var dataRow = parsedBody["properties"]["rows"][0];
        return Math.Round((decimal)dataRow[0], 2);
    }
}

 static object BuildCostRequestBody()
{
    return JsonSerializer.Deserialize<object>(
        "{\"type\":\"ActualCost\",\"dataSet\":{\"granularity\":\"Monthly\",\"aggregation\":{\"preTaxCost\":{\"name\":\"PreTaxCost\",\"function\":\"Sum\"}}}}"
    );
}

private async Task WaitOnNetwork()
{
    await Task.WhenAny(
        Task.Delay(_pingTimeoutSeconds),
        Task.Run(async () => {
            var pingResult = Ping();
            while((!pingResult)){
                pingResult = Ping();
                for(var i = 0; i < DisplayUpdater.NumberOfLoadingSegs; i++)
                {
                    var interval = _pingInterval/DisplayUpdater.NumberOfLoadingSegs;
                    if(i == DisplayUpdater.NumberOfLoadingSegs)
                    {
                        i = 0;
                    }
                    await _displayUpdater.DisplayLoading(i);
                    await Task.Delay(interval);
                }
            }
        }));
}

static bool Ping()
{
    Ping ping = new Ping();
    var response = ping.Send(_networkTestIp);
    return response.Status == IPStatus.Success;
}
          

static async Task<string> GetAuthToken(string appId, string secret, string tenantId)
{
    string authContextURL = "https://login.windows.net/" + tenantId;
    var authenticationContext = new AuthenticationContext(authContextURL);
    var credential = new ClientCredential(appId, secret);
    var result = await authenticationContext
        .AcquireTokenAsync("https://management.azure.com/", credential);
    if (result == null)
    {
        throw new InvalidOperationException("Failed to obtain the JWT token");
    }
    string token = result.AccessToken;
    return token;
}
}