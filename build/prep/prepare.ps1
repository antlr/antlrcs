param (
  [switch]$Debug,
  [string]$VisualStudioVersion = '14.0',
  [string]$Verbosity = 'minimal',
  [string]$Logger
)

# build the solution
$SolutionPath = "..\..\Antlr3.sln"

# make sure the script was run from the expected path
if (!(Test-Path $SolutionPath)) {
  $host.ui.WriteErrorLine("The script was run from an invalid working directory.")
  exit 1
}

If ($Debug) {
  $BuildConfig = 'Debug'
} Else {
  $BuildConfig = 'Release'
}

$DebugBuild = false

# clean up from any previous builds
$CleanItems = "Runtime", "Tool", "Bootstrap", "ST3", "ST4"
$CleanItems | ForEach-Object {
  if (Test-Path $_) {
    Remove-Item -Force -Recurse $_
  }
}

# build the project
$msbuild = "${env:ProgramFiles(x86)}\MSBuild\$VisualStudioVersion\Bin\MSBuild.exe"

If ($Logger) {
  $LoggerArgument = "/logger:$Logger"
}

&$msbuild /nologo /m /nr:false /t:rebuild $LoggerArgument "/verbosity:$Verbosity" /p:Configuration=$BuildConfig $SolutionPath
If (-not $?) {
  $host.ui.WriteErrorLine("Build Failed, Aborting!")
  exit $LASTEXITCODE
}

# back up the bootstrap folder
$ArchivePath = ".\Backup\Bootstrap-" + [System.IO.Path]::GetFileNameWithoutExtension([System.IO.Path]::GetRandomFileName()) + ".7z"
.\7z.exe a -r $ArchivePath "..\Bootstrap\*"

# copy the new bootstrap files
if ($DebugBuild) {
  $BootstrapBinaries = "Antlr3.exe", "Antlr3.exe.config", "Antlr3.Runtime.dll", "Antlr3.Runtime.Debug.dll", "Antlr4.StringTemplate.dll", "Antlr4.StringTemplate.Visualizer.dll", "Antlr3.targets", "Antlr3.props", "AntlrBuildTask.dll"
}
else {
  $BootstrapBinaries = "Antlr3.exe", "Antlr3.exe.config", "Antlr3.Runtime.dll", "Antlr3.Runtime.Debug.dll", "Antlr4.StringTemplate.dll", "Antlr3.targets", "Antlr3.props", "AntlrBuildTask.dll"
}

$BootstrapBinaries | ForEach-Object {
    copy -force "..\..\bin\$BuildConfig\$_" "..\Bootstrap"
    If (-not $?) {
      $host.ui.WriteErrorLine("Bootstrap update failed, Aborting!")
      exit $LASTEXITCODE
    }
}

If (-not $?) {
  $host.ui.WriteErrorLine("Bootstrap update failed, Aborting!")
  exit $LASTEXITCODE
}

if (-not (Test-Path "..\Bootstrap\Codegen\Templates\CSharp2")) {
  mkdir "..\Bootstrap\Codegen\Templates\CSharp2"
}

copy -force "..\..\bin\$BuildConfig\Codegen\Templates\LeftRecursiveRules.stg" "..\Bootstrap\Codegen\Templates"
If (-not $?) {
  $host.ui.WriteErrorLine("Bootstrap update failed, Aborting!")
  exit $LASTEXITCODE
}

copy -force "..\..\bin\$BuildConfig\Codegen\Templates\CSharp2\*" "..\Bootstrap\Codegen\Templates\CSharp2"
If (-not $?) {
  $host.ui.WriteErrorLine('Bootstrap update failed, Aborting!')
  exit $LASTEXITCODE
}

copy -force "..\..\bin\$BuildConfig\Codegen\Templates\CSharp3\*" "..\Bootstrap\Codegen\Templates\CSharp3"
If (-not $?) {
  $host.ui.WriteErrorLine("Bootstrap update failed, Aborting!")
  exit $LASTEXITCODE
}

copy -force "..\..\bin\$BuildConfig\Targets\Antlr3.Targets.CSharp2.dll" "..\Bootstrap\Targets"
If (-not $?) {
  $host.ui.WriteErrorLine("Bootstrap update failed, Aborting!")
  exit $LASTEXITCODE
}

copy -force "..\..\bin\$BuildConfig\Targets\Antlr3.Targets.CSharp3.dll" "..\Bootstrap\Targets"
If (-not $?) {
  $host.ui.WriteErrorLine("Bootstrap update failed, Aborting!")
  exit $LASTEXITCODE
}

copy -r -force "..\..\bin\$BuildConfig\Tool\*" "..\Bootstrap\Tool"
If (-not $?) {
  $host.ui.WriteErrorLine("Bootstrap update failed, Aborting!")
  exit $LASTEXITCODE
}

Remove-Item -force "..\Bootstrap\Tool\Templates\messages\formats\gnu.stg"

# build the project again with the new bootstrap files
&$msbuild /nologo /m /nr:false /t:rebuild /p:Configuration=$BuildConfig $SolutionPath
If (-not $?) {
  $host.ui.WriteErrorLine("Build Failed, Aborting!")
  exit $LASTEXITCODE
}

# copy files from the build
mkdir Runtime
mkdir Tool
mkdir Bootstrap
mkdir ST3
mkdir ST4
copy "..\..\bin\$BuildConfig\Antlr3.Runtime.dll" ".\Runtime"
copy "..\..\bin\$BuildConfig\Antlr3.Runtime.pdb" ".\Runtime"
copy "..\..\bin\$BuildConfig\Antlr3.Runtime.xml" ".\Runtime"
copy "LICENSE.txt" ".\Runtime"

copy "..\..\bin\$BuildConfig\Antlr3.exe" ".\Tool"
copy "..\..\bin\$BuildConfig\Antlr3.exe.config" ".\Tool"
copy "..\..\bin\$BuildConfig\Antlr3.Runtime.dll" ".\Tool"
copy "..\..\bin\$BuildConfig\Antlr3.Runtime.Debug.dll" ".\Tool"
copy "..\..\bin\$BuildConfig\Antlr4.StringTemplate.dll" ".\Tool"
if ($DebugBuild) {
  copy "..\..\bin\$BuildConfig\Antlr4.StringTemplate.Visualizer.dll" ".\Tool"
}
copy "..\..\bin\$BuildConfig\Antlr3.props" ".\Tool"
copy "..\..\bin\$BuildConfig\Antlr3.targets" ".\Tool"
copy "..\..\bin\$BuildConfig\AntlrBuildTask.dll" ".\Tool"
copy "LICENSE.txt" ".\Tool"

copy ".\Tool\*" ".\Bootstrap"

# copy ST4 binaries and all symbol files to the full Tool folder
copy "..\..\bin\$BuildConfig\Antlr3.pdb" ".\Tool"
copy "..\..\bin\$BuildConfig\Antlr3.Runtime.pdb" ".\Tool"
copy "..\..\bin\$BuildConfig\Antlr3.Runtime.Debug.pdb" ".\Tool"
copy "..\..\bin\$BuildConfig\Antlr4.StringTemplate.pdb" ".\Tool"
if ($DebugBuild) {
  copy "..\..\bin\$BuildConfig\Antlr4.StringTemplate.Visualizer.pdb" ".\Tool"
}
copy "..\..\bin\$BuildConfig\AntlrBuildTask.pdb" ".\Tool"
copy "..\..\bin\$BuildConfig\Antlr3.xml" ".\Tool"
copy "..\..\bin\$BuildConfig\Antlr3.Runtime.xml" ".\Tool"
copy "..\..\bin\$BuildConfig\Antlr3.Runtime.Debug.xml" ".\Tool"
copy "..\..\bin\$BuildConfig\Antlr4.StringTemplate.xml" ".\Tool"
if ($DebugBuild) {
  copy "..\..\bin\$BuildConfig\Antlr4.StringTemplate.Visualizer.xml" ".\Tool"
}
copy "..\..\bin\$BuildConfig\AntlrBuildTask.xml" ".\Tool"

mkdir "Tool\Codegen"
mkdir "Tool\Targets"
mkdir "Tool\Tool"
copy -r "..\..\bin\$BuildConfig\Codegen\*" ".\Tool\Codegen"
copy -r "..\..\bin\$BuildConfig\Targets\*.dll" ".\Tool\Targets"
copy -r "..\..\bin\$BuildConfig\Targets\*.pdb" ".\Tool\Targets"
copy -r "..\..\bin\$BuildConfig\Targets\*.xml" ".\Tool\Targets"
copy -r "..\..\bin\$BuildConfig\Tool\*" ".\Tool\Tool"

mkdir "Bootstrap\Codegen\Templates\CSharp2"
mkdir "Bootstrap\Codegen\Templates\CSharp3"
mkdir "Bootstrap\Tool"
mkdir "Bootstrap\Targets"
copy "..\..\bin\$BuildConfig\Codegen\Templates\LeftRecursiveRules.stg" ".\Bootstrap\Codegen\Templates"
copy "..\..\bin\$BuildConfig\Codegen\Templates\CSharp2\*" ".\Bootstrap\Codegen\Templates\CSharp2"
copy "..\..\bin\$BuildConfig\Codegen\Templates\CSharp3\*" ".\Bootstrap\Codegen\Templates\CSharp3"
copy "..\..\bin\$BuildConfig\Targets\Antlr3.Targets.CSharp2.dll" ".\Bootstrap\Targets"
copy "..\..\bin\$BuildConfig\Targets\Antlr3.Targets.CSharp3.dll" ".\Bootstrap\Targets"
copy -r "..\..\bin\$BuildConfig\Tool\*" ".\Bootstrap\Tool"
Remove-Item ".\Bootstrap\Tool\Templates\messages\formats\gnu.stg"

# ST3 dist
copy "..\..\Antlr3.StringTemplate\bin\$BuildConfig\Antlr3.StringTemplate.dll" ".\ST3"
copy "..\..\Antlr3.StringTemplate\bin\$BuildConfig\Antlr3.Runtime.dll" ".\ST3"
copy "..\..\Antlr3.StringTemplate\bin\$BuildConfig\Antlr3.StringTemplate.pdb" ".\ST3"
copy "..\..\Antlr3.StringTemplate\bin\$BuildConfig\Antlr3.Runtime.pdb" ".\ST3"
copy "..\..\Antlr3.StringTemplate\bin\$BuildConfig\Antlr3.StringTemplate.xml" ".\ST3"
copy "..\..\Antlr3.StringTemplate\bin\$BuildConfig\Antlr3.Runtime.xml" ".\ST3"
copy "LICENSE.txt" ".\ST3"

# ST4 dist
copy "..\..\bin\$BuildConfig\Antlr3.Runtime.dll" ".\ST4"
copy "..\..\bin\$BuildConfig\Antlr4.StringTemplate.dll" ".\ST4"
copy "..\..\bin\$BuildConfig\Antlr4.StringTemplate.Visualizer.dll" ".\ST4"
copy "..\..\bin\$BuildConfig\Antlr3.Runtime.pdb" ".\ST4"
copy "..\..\bin\$BuildConfig\Antlr4.StringTemplate.pdb" ".\ST4"
copy "..\..\bin\$BuildConfig\Antlr4.StringTemplate.Visualizer.pdb" ".\ST4"
copy "..\..\bin\$BuildConfig\Antlr3.Runtime.xml" ".\ST4"
copy "..\..\bin\$BuildConfig\Antlr4.StringTemplate.xml" ".\ST4"
copy "..\..\bin\$BuildConfig\Antlr4.StringTemplate.Visualizer.xml" ".\ST4"
copy "LICENSE.txt" ".\ST4"

# compress the distributable packages
$AntlrVersion = "3.5.0.3-dev"
$STVersion = "4.0.7.2-dev"

$ArchivePath = ".\dist\antlr-dotnet-csharpbootstrap-" + $AntlrVersion + ".7z"
.\7z.exe a -r -mx9 $ArchivePath ".\Bootstrap\*"
$ArchivePath = ".\dist\antlr-dotnet-csharpruntime-" + $AntlrVersion + ".7z"
.\7z.exe a -r -mx9 $ArchivePath ".\Runtime\*"
$ArchivePath = ".\dist\antlr-dotnet-tool-" + $AntlrVersion + ".7z"
.\7z.exe a -r -mx9 $ArchivePath ".\Tool\*"
$ArchivePath = ".\dist\antlr-dotnet-st3-" + $AntlrVersion + ".7z"
.\7z.exe a -r -mx9 $ArchivePath ".\ST3\*"
$ArchivePath = ".\dist\antlr-dotnet-st4-" + $STVersion + ".7z"
.\7z.exe a -r -mx9 $ArchivePath ".\ST4\*"

# Build the NuGet packages

if (-not (Test-Path nuget)) {
  mkdir "nuget"
}

.\NuGet.exe pack .\Antlr3.Runtime.nuspec -OutputDirectory nuget -Prop Configuration=$BuildConfig -Version $AntlrVersion -Prop ANTLRVersion=$AntlrVersion -Prop STVersion=$STVersion -Symbols
If (-not $?) {
  $host.ui.WriteErrorLine("Failed to create NuGet package, Aborting!")
  exit $LASTEXITCODE
}

.\NuGet.exe pack .\Antlr3.Runtime.Debug.nuspec -OutputDirectory nuget -Prop Configuration=$BuildConfig -Version $AntlrVersion -Prop ANTLRVersion=$AntlrVersion -Prop STVersion=$STVersion -Symbols
If (-not $?) {
  $host.ui.WriteErrorLine("Failed to create NuGet package, Aborting!")
  exit $LASTEXITCODE
}

.\NuGet.exe pack .\Antlr3.nuspec -OutputDirectory nuget -Prop Configuration=$BuildConfig -Version $AntlrVersion -Prop ANTLRVersion=$AntlrVersion -Prop STVersion=$STVersion -Symbols
If (-not $?) {
  $host.ui.WriteErrorLine("Failed to create NuGet package, Aborting!")
  exit $LASTEXITCODE
}

.\NuGet.exe pack .\StringTemplate3.nuspec -OutputDirectory nuget -Prop Configuration=$BuildConfig -Version $AntlrVersion -Prop ANTLRVersion=$AntlrVersion -Prop STVersion=$STVersion -Symbols
If (-not $?) {
  $host.ui.WriteErrorLine("Failed to create NuGet package, Aborting!")
  exit $LASTEXITCODE
}

.\NuGet.exe pack .\StringTemplate4.nuspec -OutputDirectory nuget -Prop Configuration=$BuildConfig -Version $STVersion -Prop ANTLRVersion=$AntlrVersion -Prop STVersion=$STVersion -Symbols
If (-not $?) {
  $host.ui.WriteErrorLine("Failed to create NuGet package, Aborting!")
  exit $LASTEXITCODE
}

.\NuGet.exe pack .\StringTemplate4.Visualizer.nuspec -OutputDirectory nuget -Prop Configuration=$BuildConfig -Version $STVersion -Prop ANTLRVersion=$AntlrVersion -Prop STVersion=$STVersion -Symbols
If (-not $?) {
  $host.ui.WriteErrorLine("Failed to create NuGet package, Aborting!")
  exit $LASTEXITCODE
}
