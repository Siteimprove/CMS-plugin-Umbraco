## SiteImprove Umbraco Plugin

### Run on local machine
Reference class library to Umbraco project 

Add the following to Post-Build event command line:
``` shell
xcopy /I /E /Y "$(ProjectDir)bin\App_Plugins" "$(ProjectDir)App_Plugins"
```

Start console output after document ready
```js
siteimprove.log = true
```

### Build the Umbraco package
You can simply run this powershell (version 5) script, which will build the application, copy all the files in the right folder (/UmbracoPackages) and then finally zip the folder.
In the "createPackage.ps1" you will find variables for defining which version it will build as.