namespace AzureConsumptionDisplay.App;
public interface ICostRetriever
{
    Task<CostResult> GetCostResult();
}