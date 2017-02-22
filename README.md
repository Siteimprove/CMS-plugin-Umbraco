## SiteImprove Umbraco Plugin

### Run on local machine
Reference class library to Umbraco project 

Add the following to Post-Build event command line:
``` shell
xcopy /I /E /Y "$(ProjectDir)bin\App_Plugins" "$(ProjectDir)App_Plugins"
```