# azure-spend-iot-display

This is a simple project guide for a nifty little Raspberri Pi desk toy that displays your current Azure consumption on an 7-segment desplay. Amaze all your friends and colleagues with a constant reminder of your profligate cloud hosting spending!

## Prerequisites
- A Raspberry Pi. You can probably get away with one as old as v2.
- A computer with .NET Core sdk installed. You'll be compiling .NET Core code and publishing it to your pretty little Raspberry Pi.


## Pi Setup
- Burn a fresh Raspberry Pi with a fresh Raspbian or similar installed. As always, change the default username and password. For reals.
- Setup `wpa-supplicant` with your WiFi's SSID and password
- Enable ssh (adding a file called `ssh`  on the boot is all that's necessary)
- SSH into the Pi and enable I2C support with `raspi-config`
```
Interface Options -> I2C - Yes
```
- Create your Azure service principal for querying the Azure management api. For this example we are using `consumption-display` as our service principal name.
    - This is easy to do in Azure CLI. E.g.: 
    ```
    create-for-rbac -n consumption-display
    ```
    - This will produce an output like the one below. Guard this information as though it were a precious treasure.
    ```
  "appId": "00000000-0000-0000-0000-000000000000",
  "displayName": "consumption-display",
  "password": "PASSWORD",
  "tenant": "00000000-0000-0000-0000-000000000000"
  ```
    - You will also have to log into the Azure portal and grand read access to this service principle on the subscription. Subscriptions -> Access Control -> Add Role Assignment, etc. Know what you are doing.
- SSH into your Pi and install the .NET Core framework
    ```
    curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel Current
    ```
- And then the following:
    ```
    echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
    echo 'export PATH=$PATH:$HOME/.dotnet' >> ~/.bashrc
    source ~/.bashrc
    ```

## Build & Deploy!
- On your desktop, or whatever, navigate to the repo's `/src/AzureConsumptionDisplay.App` directory and create an `appsettings.local.json` and add the following config substituting the config values specific to your service principal:
    ```
    {
        "AZURE_APP_ID": "<Your service principal app id>",
        "AZURE_APP_PASSWORD": "<Your app password>",
        "AZURE_APP_TENANT": "<Your app tenant>",
        "AZURE_SUBSCRIPTION_ID": "<your app subscription id>"
    }
    ```
- Build and publish the app with the command running in the `src/AzureConsumptionDisplay.App` dir:
    ```
    dotnet publish --runtime linux-arm --self-contained
    ```
- Deploy the app using `scp` or similar sftp client like so:
    ```
    scp -r ./bin/debug/net6.0/linux-arm/publish/* [YOUR PI LOGIN]@[YOUR PI IP]:/home/[YOUR PI LOGIN]]/azure-consumption-display/ 
    ```
    - You may need to create the destination directory first
- SSH into your Pi and set up a cronjob for this bad boy with `crontab -e`.
```
*/5 * * * * /home/[YOUR USERNAME]/.dotnet/dotnet /home/[YOUR USERNAME]/azure-consumption-display/AzureConsumptionDisplay.App.dll >/home/[YOUR USERNAME]/azure-consumption-display-log 2>&1

@reboot /home/[YOUR USERNAME]/.dotnet/dotnet /home/[YOUR USERNAME]/azure-consumption-display/AzureConsumptionDisplay.App.dll >/home/[YOUR USERNAME]/azure-consumption-display-log 2>&1
```