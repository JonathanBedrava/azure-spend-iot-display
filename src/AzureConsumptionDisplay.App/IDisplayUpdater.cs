namespace AzureConsumptionDisplay.App
{
    public interface IDisplayUpdater
    {
         Task UpdateDisplay(decimal amount);
    }
}