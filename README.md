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
- SSH into your PI and set the following environment variables with the values from above as well as your Azure subscription id with the following commands:
    ```
    echo "export AZURE_APP_ID='[YOUR APP ID]'" >> ~/.bashrc 
    echo "export AZURE_APP_PASSWORD='[YOUR APP PASSWORD]'" >> ~/.bashrc
    echo "export AZURE_APP_TENANT='[YOUR TENANT ID]'" >> ~/.bashrc
    echo "export AZURE_SUBSCRIPTION_ID='[YOUR SUBSCRIPTION ID]'" >> ~/.bashrc
    ```
    - You can either reboot, or load these variables into your current session with the command `source ~/.bashrc`
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
- On your desktop, or whatever, build and publish the app with the command running in the `src/AzureConsumptionDisplay.App` dir:
    ```
    dotnet publish --runtime linux-arm --self-contained
    ```
- Deploy the app using `scp` or similar sftp client like so:
    ```
    scp -r ./bin/debug/net6.0/linux-arm/publish/* [YOUR PI LOGIN]@[YOUR PI IP]:/home/[YOUR PI LOGIN]]/azure-consumption-display/ 
    ```
    - You may need to create the destination directory first
- SSH into your Pi and set up a cronjob for this bad boy.
```
*/5 * * * * bash ~/home/[YOUR USER]/azure-consumption-display/AzureConsumptionDisplay.App >> home/[YOUR USER]/azure-consumption-display-log 2>&1
@reboot bash ~home/[YOUR USER]/azure-consumption-display/AzureConsumptionDisplay.App >> home/[YOUR USER]/azure-consumption-display-log 2>&1
```