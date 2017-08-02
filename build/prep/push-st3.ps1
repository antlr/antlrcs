param (
	[string]$Source = 'https://www.nuget.org/'
)

. .\version.ps1

If ($AntlrVersion.EndsWith('-dev')) {
	$host.ui.WriteErrorLine("Cannot push development version '$AntlrVersion' to NuGet.")
	Exit 1
}

$packages = @(
	'StringTemplate3')

# Make sure all packages exist before pushing any packages
ForEach ($package in $packages) {
	If (-not (Test-Path ".\nuget\$package.$AntlrVersion.nupkg")) {
		$host.ui.WriteErrorLine("Couldn't locate NuGet package: $JarPath")
		exit 1
	}

	If (-not (Test-Path ".\nuget\$package.$AntlrVersion.symbols.nupkg")) {
		$host.ui.WriteErrorLine("Couldn't locate NuGet symbols package: $JarPath")
		exit 1
	}
}

$nuget = '.\NuGet.exe'

ForEach ($package in $packages) {
	&$nuget 'push' ".\nuget\$package.$AntlrVersion.nupkg" -Source $Source
}
