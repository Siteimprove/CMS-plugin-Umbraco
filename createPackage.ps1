#Package information
$packageGuid = "4592a559-2bd8-4e75-b79e-6343764a454c"
$packageName = "SiteImproveUmbracoPlugin"
$packageVersion = "1.1.0"

#Locations
$currentLocation = Get-Location
$pluginFolder = "$currentLocation\App_Plugins\SiteImprove"
$UmbracoPackagesFolder = "$currentLocation\UmbracoPackages"

# Build application
Write-Host "Start Build application..."
C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe SiteImprove.Umbraco.Plugin.csproj /p:Configuration=Debug

# Create the package folder
$newPackageRootFolder = "$UmbracoPackagesFolder\$packageName" + "_v$packageVersion" 
$newPackageFolder = "$newPackageRootFolder\$packageGuid"
Remove-Item -Recurse -Force $newPackageRootFolder
New-Item $newPackageFolder -type directory -Force
Write-Host "Created folder $newPackageFolder"

# Modify the umbraco package.xml with the settings in this file
Write-Host "Change package.xml data..."
$packageXml = [xml](Get-Content "$currentLocation\package.xml")
$packageXml.SelectSingleNode("//version").InnerText = $packageVersion
$packageXml.Save("$currentLocation\package.xml")

# Move all the items
Copy-Item "$pluginFolder\*" $newPackageFolder -Recurse -Force
Copy-Item "$currentLocation\bin\Debug\SiteImprove.Umbraco.Plugin.dll" $newPackageFolder -Force
Copy-Item "$currentLocation\package.xml" $newPackageFolder -Force

$newPackageXml = [xml](Get-Content "$newPackageFolder\package.xml")

Get-ChildItem $newPackageFolder\* -Recurse -File |
	ForEach-Object {
		# Change all names except package.xml
		if($_.Name -eq "package.xml") { return }

		$guid = New-Guid
		$extension = $_.Extension
		$dir = $_.Directory
		Rename-Item "$dir\$_" "$dir\$guid$extension"
		
		$newPackageXml.SelectSingleNode("//guid[text()='"+ $_.Name +"']").InnerText = "$guid$extension"
	}
$newPackageXml.Save("$newPackageFolder\package.xml")

# Zip the folder, ready to use in Umbraco
Compress-Archive -Path $newPackageRootFolder -DestinationPath "$newPackageRootFolder.zip" -Force

# Cleanup
Remove-Item -Recurse -Force $newPackageRootFolder

Write-Host "Done"
Read-Host