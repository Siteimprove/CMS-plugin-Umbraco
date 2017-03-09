## SiteImprove Umbraco Plugin 

### Install
Install either through the CMS or install the nuget both are located under /build/Package

### Run on local machine
Reference class library to Umbraco project 

Add the following to Post-Build event command line:
``` shell
xcopy /I /E /Y "$(ProjectDir)bin\App_Plugins" "$(ProjectDir)App_Plugins"
```

### Check logs
Start console output after document ready
```js
siteimprove.log = true
```

### Build the Umbraco package
Run the build.bat

### TODO
 - Upload package to some nuget repository