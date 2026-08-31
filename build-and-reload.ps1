$taskDotnetHome = Join-Path $PSScriptRoot '.dotnet-cli'
$taskNuGetPackages = Join-Path $PSScriptRoot '.nuget-packages'

$env:DOTNET_CLI_HOME = $taskDotnetHome
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
$env:NUGET_PACKAGES = $taskNuGetPackages

dotnet build (Join-Path $PSScriptRoot 'QuickSell.csproj') -p:DeployToProfile=true
exit $LASTEXITCODE
