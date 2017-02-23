#Package information
$packageGuid = "4592a559-2bd8-4e75-b79e-6343764a454c"
$packageName = "SiteImproveUmbracoPlugin"
$packageVersion = "0.1.5"

#Locations
$currentLocation = Get-Location
$pluginFolder = "$currentLocation\App_Plugins\SiteImprove"
$UmbracoPackagesFolder = "$currentLocation\UmbracoPackages"

Write-Host "Start Build application..."
C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe SiteImprove.Umbraco.Plugin.csproj /p:Configuration=Debug

$newPackageRootFolder = "$UmbracoPackagesFolder\$packageName" + "_v$packageVersion" 
$newPackageFolder = "$newPackageRootFolder\$packageGuid"
New-Item $newPackageFolder -type directory
Write-Host "Created folder $newPackageFolder"

Copy-Item "$pluginFolder\*" $newPackageFolder
Copy-Item "$currentLocation\bin\Debug\SiteImprove.Umbraco.Plugin.dll" $newPackageFolder
Copy-Item "$currentLocation\package.xml" $newPackageFolder

Compress-Archive -Path $newPackageRootFolder -DestinationPath "$newPackageRootFolder.zip"

Write-Host "Done"
Read-Host