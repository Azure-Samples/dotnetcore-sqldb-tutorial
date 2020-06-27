#!/usr/bin/env pwsh

<# 
.SYNOPSIS 
    Run designated container image locally
 
.EXAMPLE
    ./run_container.ps1 -Workspace test -Destroy
#> 
#Requires -Version 7

param (
    [parameter(Mandatory=$false)][string]$Image="ewdev.azurecr.io/vdc/aspnet-core-sqldb:latest",
    [parameter(Mandatory=$false)][string]$HostingEnvironment="Development",
    [parameter(Mandatory=$false)][string]$ClientId=$env:APP_CLIENT_ID,
    [parameter(Mandatory=$false)][string]$ConnectionString=$env:ConnectionStrings:MyDbConnection,
    [parameter(Mandatory=$false)][int]$HttpPort=8080,
    [parameter(Mandatory=$false)][int]$InternalHttpPort=80
)

if (!$ConnectionString) {
    $appSettingsPath = Join-Path (Split-Path -parent -Path $MyInvocation.MyCommand.Path) "appsettings.${HostingEnvironment}.json"
    if (!(Test-Path $appSettingsPath)) {
        Write-Error "$appSettingsPath does not exist, can't determine connection string. Aborting."
        exit
    }
    $appSettings = Get-Content $appSettingsPath | ConvertFrom-Json
    Write-Debug $appSettings
    $ConnectionString = $appSettings.ConnectionStrings.MyDbConnection
    if (!$ConnectionString) {
        Write-Error "Could not determine connection string"
        exit
    }
}

$name = "$((Get-Item (Split-Path -parent -Path $MyInvocation.MyCommand.Path)).Name) $HostingEnvironment".ToLower() -replace "\W",""
$urlSpec = "http://+:$InternalHttpPort"
docker run -d -p ${HttpPort}:${InternalHttpPort} --name $Name -e APP_CLIENT_ID=$ClientId -e ASPNETCORE_ENVIRONMENT=$HostingEnvironment -e ASPNETCORE_URLS=$urlSpec -e APPSETTING_ASPNETCORE_URLS=$urlSpec -e ConnectionStrings:MyDbConnection=$ConnectionString $Image
