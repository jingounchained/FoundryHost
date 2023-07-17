# Foundry Host 1.0
A .NET 7 Worker Service wrapper for Hosting a Foundry server.

## Windows Installation and Configuration
You may use the FoundryHost.exe executable on the [latest release page](https://github.com/jingounchained/FoundryHost/releases/latest), or you may clone this repo and Publish the application yourself. Place the resulting FoundryHost.exe file in a path you wish to run the service from. It's recommended to place in the Foundry Virtual Tabletop folder alongside the Foundry Virtual Tabletop.exe, as it will find path to main.js from there without additional parameters.

## Configuration
You must install NodeJS https://nodejs.org/en/download  
Foundry Host is designed to work with the existing configuration of your Foundry installation.   
Detials on configuring Foundry can be found here: https://foundryvtt.com/article/configuration/  

### Service Account  
Foundry Host must either be configured with the DataPath and Port provided by parameter or variable, OR configured to run on the user account which installed Foundry.  The User Account MUST have the Log On As A Service permission. https://learn.microsoft.com/en-us/windows/security/threat-protection/security-policy-settings/log-on-as-a-service  

### Parameters or Environmental Variables     
You can mix and match these as you see fit. Following the same logic as Foundry, Parameters will be given priority over Environmental Variables, and those will be given priority over the options.json file.  The FOUDNRY parameter and variable is necessary to point to main.js if you are not running from the same root as Foundry VTT or have relocated the resources/app directories.
  
Below is a list of valid parameters and variables  
Command Line Parameters:   
	--foundry="path\to\main.js"  
	--datapath="user\data\path"  
	--port="<port integer>"  

Environmental Variables:  
	FOUNDRY : path\to\main.js  
	FOUNDRY_VTT_DATA_PATH: user\data\path  
	FOUDNRY_VTT_PORT: numeric port such as 30000  
  
### Install the Service  
I recommend using PowerShell to install. You'll need to run as Administrator in order to register the Service with Windows.   
Execute the command `sc.exe create FoundryHost binpath="<path/to/FoundryHost.exe" start=auto obj=".\localUsername" password="passwordForUser"`  
Optionally I would add a brief description such as `sc.exe description FoundryHost "Host Service for Foundry VTT"`  
![image](https://github.com/jingounchained/FoundryHost/assets/32217493/87d1f4cc-34e6-4c36-b93d-25e2403c2df4)


Set your Environmental Variable for FOUNDRY  
![image](https://github.com/jingounchained/FoundryHost/assets/32217493/5e7d0a20-afb5-4bf0-83df-6ec783d438b2)

Now you should see FoundryHost in your Services List.   
![image](https://github.com/jingounchained/FoundryHost/assets/32217493/d7cf39b4-5843-46c6-8dea-95770b9a9877)


If no issues were encountered you should now see the Service in your Services List and should be able to start it. Navigate to localhost:port to confirm.
![image](https://github.com/jingounchained/FoundryHost/assets/32217493/5d27e73f-aba8-4e80-bb13-82386d1cbdda)


