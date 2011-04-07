# build the solution
$SolutionPath = "..\..\Antlr3.sln"

# make sure the script was run from the expected path
if (!(Test-Path $SolutionPath)) {
  echo "The script was run from an invalid working directory."
  exit 1
}

$BuildConfig = "Release"

# clean up from any previous builds
$CleanItems = "Runtime", "Tool", "Bootstrap"
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
$BootstrapBinaries = "Antlr3.exe", "Antlr3.exe.config", "Antlr3.Runtime.dll", "Antlr3.Runtime.Debug.dll", "Antlr3.StringTemplate.dll", "Antlr3.targets", "AntlrBuildTask.dll"
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

copy -force "..\$BuildConfig\Codegen\Templates\LeftRecursiveRules.stg" "..\Bootstrap\Codegen\Templates"
if ($LASTEXITCODE -ne 0) {
  echo "Bootstrap update failed, Aborting!"
  exit $p.ExitCode
}

copy -force "..\$BuildConfig\Codegen\Templates\CSharp3\*" "..\Bootstrap\Codegen\Templates\CSharp3"
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
copy "..\$BuildConfig\Antlr3.Runtime.dll" ".\Runtime"

copy "..\$BuildConfig\Antlr3.exe" ".\Tool"
copy "..\$BuildConfig\Antlr3.exe.config" ".\Tool"
copy "..\$BuildConfig\Antlr3.Runtime.dll" ".\Tool"
copy "..\$BuildConfig\Antlr3.Runtime.Debug.dll" ".\Tool"
copy "..\$BuildConfig\Antlr3.StringTemplate.dll" ".\Tool"
copy "..\$BuildConfig\Antlr3.targets" ".\Tool"
copy "..\$BuildConfig\AntlrBuildTask.dll" ".\Tool"

copy ".\Tool\*" ".\Bootstrap"

# copy ST4 binaries and all symbol files to the full Tool folder
copy "..\$BuildConfig\Antlr3.pdb" ".\Tool"
copy "..\$BuildConfig\Antlr3.Runtime.pdb" ".\Tool"
copy "..\$BuildConfig\Antlr3.Runtime.Debug.pdb" ".\Tool"
copy "..\$BuildConfig\Antlr3.StringTemplate.pdb" ".\Tool"
copy "..\$BuildConfig\AntlrBuildTask.pdb" ".\Tool"
copy "..\..\Antlr4.StringTemplate\bin\$BuildConfig\Antlr4.StringTemplate.dll" ".\Tool"
copy "..\..\Antlr4.StringTemplate\bin\$BuildConfig\Antlr4.StringTemplate.pdb" ".\Tool"
copy "..\..\Antlr4.StringTemplate.Visualizer\bin\$BuildConfig\Antlr4.StringTemplate.Visualizer.dll" ".\Tool"
copy "..\..\Antlr4.StringTemplate.Visualizer\bin\$BuildConfig\Antlr4.StringTemplate.Visualizer.pdb" ".\Tool"

mkdir "Tool\Codegen"
mkdir "Tool\Targets"
mkdir "Tool\Tool"
copy -r "..\$BuildConfig\Codegen\*" ".\Tool\Codegen"
copy -r "..\$BuildConfig\Targets\*.dll" ".\Tool\Targets"
copy -r "..\$BuildConfig\Targets\*.pdb" ".\Tool\Targets"
copy -r "..\$BuildConfig\Tool\*" ".\Tool\Tool"

mkdir "Bootstrap\Codegen\Templates\CSharp3"
mkdir "Bootstrap\Tool"
mkdir "Bootstrap\Targets"
copy "..\$BuildConfig\Codegen\Templates\LeftRecursiveRules.stg" ".\Bootstrap\Codegen\Templates"
copy "..\$BuildConfig\Codegen\Templates\CSharp3\*" ".\Bootstrap\Codegen\Templates\CSharp3"
copy "..\$BuildConfig\Targets\Antlr3.Targets.CSharp3.dll" ".\Bootstrap\Targets"
copy -r "..\$BuildConfig\Tool\*" ".\Bootstrap\Tool"
Remove-Item ".\Bootstrap\Tool\Templates\messages\formats\gnu.stg"
