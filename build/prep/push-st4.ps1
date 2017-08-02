param (
	[string]$Source = 'https://www.nuget.org/'
)

. .\version.ps1

If ($STVersion.EndsWith('-dev')) {
	$host.ui.WriteErrorLine("Cannot push development version '$STVersion' to NuGet.")
	Exit 1
}

$packages = @(
	'StringTemplate4'
	'StringTemplate4.Visualizer')

# Make sure all packages exist before pushing any packages
ForEach ($package in $packages) {
	If (-not (Test-Path ".\nuget\$package.$STVersion.nupkg")) {
		$host.ui.WriteErrorLine("Couldn't locate NuGet package: $JarPath")
		exit 1
	}

	If (-not (Test-Path ".\nuget\$package.$STVersion.symbols.nupkg")) {
		$host.ui.WriteErrorLine("Couldn't locate NuGet symbols package: $JarPath")
		exit 1
	}
}

$nuget = '.\NuGet.exe'

ForEach ($package in $packages) {
	&$nuget 'push' ".\nuget\$package.$STVersion.nupkg" -Source $Source
}
