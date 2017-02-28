#Package information
$packageGuid = "4592a559-2bd8-4e75-b79e-6343764a454c"
$packageName = "SiteImproveUmbracoPlugin"
$packageVersion = "1.0.0"

#Locations
$currentLocation = Get-Location
$pluginFolder = "$currentLocation\App_Plugins\SiteImprove"
$UmbracoPackagesFolder = "$currentLocation\UmbracoPackages"

# Build application
Write-Host "Start Build application..."
C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe SiteImprove.Umbraco.Plugin.csproj /p:Configuration=Debug

# Modify the umbraco package.xml with the settings in this file
Write-Host "Change package.xml data..."
$packageXml = [xml](Get-Content "$currentLocation\package.xml")
$packageXml.SelectSingleNode("//version").InnerText = $packageVersion
$packageXml.Save("$currentLocation\package.xml")

# Create the package folder
$newPackageRootFolder = "$UmbracoPackagesFolder\$packageName" + "_v$packageVersion" 
$newPackageFolder = "$newPackageRootFolder\$packageGuid"
New-Item $newPackageFolder -type directory -Force
Write-Host "Created folder $newPackageFolder"

# Move all the items
Copy-Item "$pluginFolder\*" $newPackageFolder -Recurse -Force
Copy-Item "$currentLocation\bin\Debug\SiteImprove.Umbraco.Plugin.dll" $newPackageFolder -Force
Copy-Item "$currentLocation\package.xml" $newPackageFolder -Force

# Zip the folder, ready to use in Umbraco
Compress-Archive -Path $newPackageRootFolder -DestinationPath "$newPackageRootFolder.zip" -Force

Write-Host "Done"
Read-Host