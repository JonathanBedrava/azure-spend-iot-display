# azure-spend-iot-display

This is a simple project guide for a nifty little Raspberri Pi desk toy that displays your current Azure consumption on an 7-segment desplay. Amaze all your friends and colleagues with a constant reminder of your profligate cloud hosting spending!

## Pi Setup
- Burn a fresh Raspberry Pi with a fresh Raspbian or similar installed.
- Setup `wpa-supplicant` with your WiFi's SSID and password
- Enable ssh (adding a file called `ssh`  on the boot is all that's necessary)
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
- SSH into your PI and set the following environment variables with the values above like so:
    - `sudo nano ~/.bashrc`
    - Add the following lines to the bottom of the file, substituting the info from above:
    ```
    export AZURE_APP_ID='[YOUR APP ID]]' 
    export AZURE_APP_PASSWORD='[YOUR APP PASSWORD]'
    export AZURE_APP_TENANT='[YOUR TENANT ID]'
    ```
    - Save and exit nano, `Ctrl-X` or something
    - You can either reboot, or load these variables into your current session with the command `source ~/.bashrc`