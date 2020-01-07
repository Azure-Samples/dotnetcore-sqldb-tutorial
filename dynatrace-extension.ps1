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
$appUrl = $data.destinationAppUrl
$scmUrl = "https://{0}" -f $data.publishUrl
$credentials = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(("{0}:{1}" -f $data.userName,$data.userPWD)))

# Ensure Web App is running
Invoke-RestMethod -Method 'GET' -Uri $appUrl

# Install Site Extension via KUDU Rest API
$invoke = Invoke-RestMethod -Method 'GET' -Headers @{Authorization=("Basic {0}" -f $credentials)} -Uri ("{0}/api/extensionfeed" -f $scmUrl) -MaximumRetryCount 5 -RetryIntervalSec 5
$id = ($invoke | ? {$_.id -match "Dynatrace"}).id
try {
  $install = Invoke-RestMethod -Method 'POST' -Headers @{Authorization=("Basic {0}" -f $credentials)} -Uri ("{0}/api/siteextensions/{1}" -f $scmUrl,$id)
  $installStatus = ($install.provisioningState).ToString() + "|" + ($install.installed_date_time).ToString()
  Write-Output "Installation Status : $installStatus"
}
catch{$_}

# Kill Kudu's process, so that the Site Extension gets loaded next time it starts. This returns a 502, but can be ignored.
#Invoke-RestMethod -Headers @{Authorization=("Basic {0}" -f $credentials)} -Method 'DELETE' -Uri ("{0}/api/processes/0" -f $scmUrl)
try {
  Invoke-RestMethod -Headers @{Authorization=("Basic {0}" -f $credentials)} -Method 'DELETE' -Uri ("{0}/api/processes/0" -f $scmUrl)
} catch {
  If ( $error[0].Exception.Response.StatusCode -eq "BadGateway" ) {
    exit 0
  } else {
    Write-Host "Unexpected Status Code: $($error[0].Exception.Response.StatusCode.value__) $($error[0].Exception.Response.StatusCode)" ; exit 1
  }
}

# Now you can make make queries to the Dynatrace Site Extension API.
# If it's the first request to the SCM website, the request may fail due to request-timeout.
$retry = 0
while ($true) {
    try {
        Invoke-RestMethod -Headers @{Authorization=("Basic {0}" -f $credentials)} -Uri ("{0}/dynatrace/api/status" -f $scmUrl)
    } catch {
        if (++$retry -ge 3) {
            break
        }
    }
}

#----------------------------------------------------------
# Install the agent through extensions API ----------------
#----------------------------------------------------------
$settings = @{
    "environmentId" = $environmentId
    "apiUrl"        = $apiUrl
    "apiToken"      = $apiToken
    "sslMode"       = $sslMode
}
Invoke-RestMethod -Headers @{Authorization=("Basic {0}" -f $credentials)} -Method 'PUT' -ContentType "application/json" -Uri ("{0}/dynatrace/api/settings" -f $scmUrl) -Body ($settings | ConvertTo-Json)

# Wait until the agent is installed or the installation fails
while ($true) {
    $status = Invoke-RestMethod -Headers @{Authorization=("Basic {0}" -f $credentials)} -Uri ("{0}/dynatrace/api/status" -f $scmUrl)
    if (($status.state -eq "Installed") -or ($status.state -eq "Failed")) {
        Write-Host "OneAgent Status: $($status.state)"
        break
    }

    Start-Sleep -Seconds 10
}

# Restart app-service so changes gets applied
