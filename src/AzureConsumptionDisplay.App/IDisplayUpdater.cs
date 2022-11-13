namespace AzureConsumptionDisplay.App
{
    public interface IDisplayUpdater
    {
         Task UpdateDisplay(decimal amount);
         Task SetErrorDisplay();
         Task DisplayLoading(int step);
    }
}