param(
    [string]$AxiForgeApi = "http://localhost:5055",
    [string]$AxiForgeWeb = "http://localhost:5242",
    [string]$AxiPlusApi = "http://localhost:5228",
    [string]$AxiPlusWeb = "http://localhost:5094"
)

$ErrorActionPreference = "Stop"

function Invoke-RegressionCheck {
    param(
        [string]$Name,
        [scriptblock]$Check
    )

    Write-Host "Entering -> AxiCore -> scripts -> Invoke-AxiCoreRegression.ps1 -> Invoke-RegressionCheck"
    try {
        & $Check
        Write-Host "PASS -> $Name"
    }
    catch {
        Write-Host "FAIL -> $Name -> $($_.Exception.Message)"
        throw
    }
    finally {
        Write-Host "Exiting -> AxiCore -> scripts -> Invoke-AxiCoreRegression.ps1 -> Invoke-RegressionCheck"
    }
}

function Assert-HttpSuccess {
    param(
        [string]$Name,
        [string]$Url,
        [string]$Method = "GET",
        [object]$Body = $null
    )

    Write-Host "Entering -> AxiCore -> scripts -> Invoke-AxiCoreRegression.ps1 -> Assert-HttpSuccess"
    try {
        $parameters = @{
            Uri = $Url
            Method = $Method
            UseBasicParsing = $true
        }

        if ($null -ne $Body) {
            $parameters.ContentType = "application/json"
            $parameters.Body = ($Body | ConvertTo-Json -Depth 12)
        }

        $response = Invoke-WebRequest @parameters
        if ($response.StatusCode -lt 200 -or $response.StatusCode -gt 299) {
            throw "$Name returned HTTP $($response.StatusCode)"
        }
    }
    finally {
        Write-Host "Exiting -> AxiCore -> scripts -> Invoke-AxiCoreRegression.ps1 -> Assert-HttpSuccess"
    }
}

Invoke-RegressionCheck "AxiForge API health" {
    Assert-HttpSuccess "AxiForge API health" "$AxiForgeApi/api/health"
}

Invoke-RegressionCheck "AxiForge login page" {
    Assert-HttpSuccess "AxiForge login page" "$AxiForgeWeb/login"
}

Invoke-RegressionCheck "AxiForge practice page" {
    Assert-HttpSuccess "AxiForge practice page" "$AxiForgeWeb/practice"
}

Invoke-RegressionCheck "AxiForge roadmaps page" {
    Assert-HttpSuccess "AxiForge roadmaps page" "$AxiForgeWeb/roadmaps"
}

Invoke-RegressionCheck "AxiForge coding problems API" {
    Assert-HttpSuccess "AxiForge coding problems API" "$AxiForgeApi/api/coding/problems"
}

Invoke-RegressionCheck "AxiForge assessments API" {
    Assert-HttpSuccess "AxiForge assessments API" "$AxiForgeApi/api/assessments"
}

Invoke-RegressionCheck "AxiForge password reset request" {
    Assert-HttpSuccess "AxiForge password reset request" "$AxiForgeApi/api/auth/request-password-reset" "POST" @{ email = "regression.student@example.com" }
}

Invoke-RegressionCheck "AxiPlus login page" {
    Assert-HttpSuccess "AxiPlus login page" "$AxiPlusWeb/login"
}

Invoke-RegressionCheck "AxiPlus Admin AxiForge page shell" {
    Assert-HttpSuccess "AxiPlus Admin AxiForge page shell" "$AxiPlusWeb/admin/axiforge"
}

Invoke-RegressionCheck "AxiPlus API swagger" {
    Assert-HttpSuccess "AxiPlus API swagger" "$AxiPlusApi/swagger/index.html"
}

Write-Host "REGRESSION COMPLETE -> AxiCore -> AxiPlus/AxiForge shell and API smoke checks passed."
