#----------------------------------------------------------
# Install Dynatrace extensions ............----------------
#----------------------------------------------------------

[CmdletBinding()]
param(
    [Parameter(Mandatory=$True)]
	[string]$subscription,

    [Parameter(Mandatory=$True)]
	[string]$resourceGroup,

    [Parameter(Mandatory=$True)]
	[string]$appName,

	[Parameter(Mandatory=$True)]
	[string]$environmentId,

    [Parameter(Mandatory=$True)]
    [string]$apiToken,

    [string]$apiUrl = "",

    [string]$sslMode = "Default"
)

# Get SCM credentials
$data = (az webapp deployment list-publishing-profiles --name $appName --subscription $subscription --resource-group $resourceGroup | ConvertFrom-Json) | Where-Object {$_.publishMethod -eq 'MSDeploy'}
$scmUrl = "https://{0}" -f $data.publishUrl
$credentials = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(("{0}:{1}" -f $data.userName,$data.userPWD)))

# Install Site Extension via KUDU Rest API
$invoke = Invoke-RestMethod -Method 'GET' -Headers @{Authorization=("Basic {0}" -f $credentials)} -Uri ("{0}/api/extensionfeed" -f $scmUrl)
$id = ($invoke | ? {$_.id -match "Dynatrace"}).id
az resource create --resource-type "Microsoft.Web/sites/siteextensions" --resource-group $resourceGroup --name "$appName/siteextensions/$id" --properties '{}'

$installStatus = ""
Do {
  Start-Sleep 3
  $install = Invoke-RestMethod -Method 'GET' -Headers @{Authorization=("Basic {0}" -f $credentials)} -Uri ("{0}/api/siteextensions" -f $scmUrl)
  $installStatus = ($install | ? {$_.id -match "Dynatrace"}).provisioningState
}
Until (($installStatus -eq "Succeeded") -or ($installStatus -eq "Failure"))

Write-Output "Installation Status: $installStatus"

# Now you can make make queries to the Dynatrace Site Extension API.
# If it's the first request to the SCM website, the request may fail due to request-timeout.
$up = "false"
Do {
  try {
    Invoke-RestMethod -Headers @{Authorization=("Basic {0}" -f $credentials)} -Uri ("{0}/dynatrace/api/status" -f $scmUrl)
    $up = "true"
  } catch {
    $up = "false"
  }
} Until ($up -eq "true")

# Install the agent through extensions API
$settings = @{
    "environmentId" = $environmentId
    "apiUrl"        = $apiUrl
    "apiToken"      = $apiToken
    "sslMode"       = $sslMode
}
Invoke-RestMethod -Headers @{Authorization=("Basic {0}" -f $credentials)} -Method 'PUT' -ContentType "application/json" -Uri ("{0}/dynatrace/api/settings" -f $scmUrl) -Body ($settings | ConvertTo-Json)

# Wait until the agent is installed or the installation fails
$status = ""
Do {
  Start-Sleep 3
  $request = Invoke-RestMethod -Headers @{Authorization=("Basic {0}" -f $credentials)} -Uri ("{0}/dynatrace/api/status" -f $scmUrl)
  $status = $request.state
  Write-Output "Current Agent Status: $status"
} Until (($status -eq "Installed") -or ($status -eq "Failed"))

Write-Output "Final Agent Status: $status"

# Restart app-service so changes gets applied
