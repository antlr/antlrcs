# build the solution
$SolutionPath = "..\..\Antlr3.sln"

# make sure the script was run from the expected path
if (!(Test-Path $SolutionPath)) {
  echo "The script was run from an invalid working directory."
  exit 1
}

$BuildConfig = "Release"
$DebugBuild = false

# clean up from any previous builds
$CleanItems = "Runtime", "Tool", "Bootstrap", "ST3", "ST4"
$CleanItems | ForEach-Object {
  if (Test-Path $_) {
    Remove-Item -Force -Recurse $_
  }
}

# build the project
$msbuild = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe"
#if (!(Test-Path $msbuild)) {
#    [void][System.Reflection.Assembly]::Load('Microsoft.Build.Utilities.v3.5, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a') 
#    $msbuild = [Microsoft.Build.Utilities.ToolLocationHelper]::GetPathToDotNetFrameworkFile("msbuild.exe", "VersionLatest") 
#}

&$msbuild /nologo /m /nr:false /t:rebuild /p:Configuration=$BuildConfig $SolutionPath
if ($LASTEXITCODE -ne 0) {
  echo "Build Failed, Aborting!"
  exit $p.ExitCode
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
    copy -force "..\$BuildConfig\$_" "..\Bootstrap"
    if ($LASTEXITCODE -ne 0) {
      echo "Bootstrap update failed, Aborting!"
      exit $p.ExitCode
    }
}

if ($LASTEXITCODE -ne 0) {
  echo "Bootstrap update failed, Aborting!"
  exit $p.ExitCode
}

if (-not (Test-Path "..\Bootstrap\Codegen\Templates\CSharp2")) {
  mkdir "..\Bootstrap\Codegen\Templates\CSharp2"
}

copy -force "..\$BuildConfig\Codegen\Templates\LeftRecursiveRules.stg" "..\Bootstrap\Codegen\Templates"
if ($LASTEXITCODE -ne 0) {
  echo "Bootstrap update failed, Aborting!"
  exit $p.ExitCode
}

copy -force "..\$BuildConfig\Codegen\Templates\CSharp2\*" "..\Bootstrap\Codegen\Templates\CSharp2"
if ($LASTEXITCODE -ne 0) {
  echo "Bootstrap update failed, Aborting!"
  exit $p.ExitCode
}

copy -force "..\$BuildConfig\Codegen\Templates\CSharp3\*" "..\Bootstrap\Codegen\Templates\CSharp3"
if ($LASTEXITCODE -ne 0) {
  echo "Bootstrap update failed, Aborting!"
  exit $p.ExitCode
}

copy -force "..\$BuildConfig\Targets\Antlr3.Targets.CSharp2.dll" "..\Bootstrap\Targets"
if ($LASTEXITCODE -ne 0) {
  echo "Bootstrap update failed, Aborting!"
  exit $p.ExitCode
}

copy -force "..\$BuildConfig\Targets\Antlr3.Targets.CSharp3.dll" "..\Bootstrap\Targets"
if ($LASTEXITCODE -ne 0) {
  echo "Bootstrap update failed, Aborting!"
  exit $p.ExitCode
}

copy -r -force "..\$BuildConfig\Tool\*" "..\Bootstrap\Tool"
if ($LASTEXITCODE -ne 0) {
  echo "Bootstrap update failed, Aborting!"
  exit $p.ExitCode
}

Remove-Item -force "..\Bootstrap\Tool\Templates\messages\formats\gnu.stg"

# build the project again with the new bootstrap files
&$msbuild /nologo /m /nr:false /t:rebuild /p:Configuration=$BuildConfig $SolutionPath
if ($LASTEXITCODE -ne 0) {
  echo "Build Failed, Aborting!"
  exit $p.ExitCode
}

# copy files from the build
mkdir Runtime
mkdir Tool
mkdir Bootstrap
mkdir ST3
mkdir ST4
copy "..\$BuildConfig\Antlr3.Runtime.dll" ".\Runtime"
copy "..\$BuildConfig\Antlr3.Runtime.pdb" ".\Runtime"
copy "..\$BuildConfig\Antlr3.Runtime.xml" ".\Runtime"
copy "LICENSE.txt" ".\Runtime"

copy "..\$BuildConfig\Antlr3.exe" ".\Tool"
copy "..\$BuildConfig\Antlr3.exe.config" ".\Tool"
copy "..\$BuildConfig\Antlr3.Runtime.dll" ".\Tool"
copy "..\$BuildConfig\Antlr3.Runtime.Debug.dll" ".\Tool"
copy "..\$BuildConfig\Antlr4.StringTemplate.dll" ".\Tool"
if ($DebugBuild) {
  copy "..\$BuildConfig\Antlr4.StringTemplate.Visualizer.dll" ".\Tool"
}
copy "..\$BuildConfig\Antlr3.props" ".\Tool"
copy "..\$BuildConfig\Antlr3.targets" ".\Tool"
copy "..\$BuildConfig\AntlrBuildTask.dll" ".\Tool"
copy "LICENSE.txt" ".\Tool"

copy ".\Tool\*" ".\Bootstrap"

# copy ST4 binaries and all symbol files to the full Tool folder
copy "..\$BuildConfig\Antlr3.pdb" ".\Tool"
copy "..\$BuildConfig\Antlr3.Runtime.pdb" ".\Tool"
copy "..\$BuildConfig\Antlr3.Runtime.Debug.pdb" ".\Tool"
copy "..\$BuildConfig\Antlr4.StringTemplate.pdb" ".\Tool"
if ($DebugBuild) {
  copy "..\$BuildConfig\Antlr4.StringTemplate.Visualizer.pdb" ".\Tool"
}
copy "..\$BuildConfig\AntlrBuildTask.pdb" ".\Tool"
copy "..\$BuildConfig\Antlr3.xml" ".\Tool"
copy "..\$BuildConfig\Antlr3.Runtime.xml" ".\Tool"
copy "..\$BuildConfig\Antlr3.Runtime.Debug.xml" ".\Tool"
copy "..\$BuildConfig\Antlr4.StringTemplate.xml" ".\Tool"
if ($DebugBuild) {
  copy "..\$BuildConfig\Antlr4.StringTemplate.Visualizer.xml" ".\Tool"
}
copy "..\$BuildConfig\AntlrBuildTask.xml" ".\Tool"

mkdir "Tool\Codegen"
mkdir "Tool\Targets"
mkdir "Tool\Tool"
copy -r "..\$BuildConfig\Codegen\*" ".\Tool\Codegen"
copy -r "..\$BuildConfig\Targets\*.dll" ".\Tool\Targets"
copy -r "..\$BuildConfig\Targets\*.pdb" ".\Tool\Targets"
copy -r "..\$BuildConfig\Targets\*.xml" ".\Tool\Targets"
copy -r "..\$BuildConfig\Tool\*" ".\Tool\Tool"

mkdir "Bootstrap\Codegen\Templates\CSharp2"
mkdir "Bootstrap\Codegen\Templates\CSharp3"
mkdir "Bootstrap\Tool"
mkdir "Bootstrap\Targets"
copy "..\$BuildConfig\Codegen\Templates\LeftRecursiveRules.stg" ".\Bootstrap\Codegen\Templates"
copy "..\$BuildConfig\Codegen\Templates\CSharp2\*" ".\Bootstrap\Codegen\Templates\CSharp2"
copy "..\$BuildConfig\Codegen\Templates\CSharp3\*" ".\Bootstrap\Codegen\Templates\CSharp3"
copy "..\$BuildConfig\Targets\Antlr3.Targets.CSharp2.dll" ".\Bootstrap\Targets"
copy "..\$BuildConfig\Targets\Antlr3.Targets.CSharp3.dll" ".\Bootstrap\Targets"
copy -r "..\$BuildConfig\Tool\*" ".\Bootstrap\Tool"
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
copy "..\$BuildConfig\Antlr3.Runtime.dll" ".\ST4"
copy "..\$BuildConfig\Antlr4.StringTemplate.dll" ".\ST4"
copy "..\$BuildConfig\Antlr4.StringTemplate.Visualizer.dll" ".\ST4"
copy "..\$BuildConfig\Antlr3.Runtime.pdb" ".\ST4"
copy "..\$BuildConfig\Antlr4.StringTemplate.pdb" ".\ST4"
copy "..\$BuildConfig\Antlr4.StringTemplate.Visualizer.pdb" ".\ST4"
copy "..\$BuildConfig\Antlr3.Runtime.xml" ".\ST4"
copy "..\$BuildConfig\Antlr4.StringTemplate.xml" ".\ST4"
copy "..\$BuildConfig\Antlr4.StringTemplate.Visualizer.xml" ".\ST4"
copy "LICENSE.txt" ".\ST4"

# compress the distributable packages
$AntlrVersion = "3.5.0.3-alpha001"
$STVersion = "4.0.7.2-alpha001"

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

.\NuGet.exe pack .\Antlr3.Runtime.nuspec -OutputDirectory nuget -Prop Configuration=Release -Version $AntlrVersion -Prop ANTLRVersion=$AntlrVersion -Prop STVersion=$STVersion -Symbols
.\NuGet.exe pack .\Antlr3.Runtime.Debug.nuspec -OutputDirectory nuget -Prop Configuration=Release -Version $AntlrVersion -Prop ANTLRVersion=$AntlrVersion -Prop STVersion=$STVersion -Symbols
.\NuGet.exe pack .\Antlr3.nuspec -OutputDirectory nuget -Prop Configuration=Release -Version $AntlrVersion -Prop ANTLRVersion=$AntlrVersion -Prop STVersion=$STVersion -Symbols
.\NuGet.exe pack .\StringTemplate3.nuspec -OutputDirectory nuget -Prop Configuration=Release -Version $AntlrVersion -Prop ANTLRVersion=$AntlrVersion -Prop STVersion=$STVersion -Symbols
.\NuGet.exe pack .\StringTemplate4.nuspec -OutputDirectory nuget -Prop Configuration=Release -Version $STVersion -Prop ANTLRVersion=$AntlrVersion -Prop STVersion=$STVersion -Symbols
.\NuGet.exe pack .\StringTemplate4.Visualizer.nuspec -OutputDirectory nuget -Prop Configuration=Release -Version $STVersion -Prop ANTLRVersion=$AntlrVersion -Prop STVersion=$STVersion -Symbols
