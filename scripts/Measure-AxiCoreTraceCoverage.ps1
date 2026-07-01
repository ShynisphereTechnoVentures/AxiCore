param(
    [string[]]$Paths = @("AxiCore.Shared", "AxiForge", "AxiPlus")
)

$ErrorActionPreference = "Stop"

function Count-Matches {
    param(
        [string]$Pattern
    )

    Write-Host "Entering -> AxiCore -> scripts -> Measure-AxiCoreTraceCoverage.ps1 -> Count-Matches"
    try {
        $result = rg -n $Pattern $Paths -g "*.cs" -g "*.razor" 2>$null
        if ($LASTEXITCODE -eq 1) {
            return 0
        }

        return @($result).Count
    }
    finally {
        Write-Host "Exiting -> AxiCore -> scripts -> Measure-AxiCoreTraceCoverage.ps1 -> Count-Matches"
    }
}

$functionTraceCount = Count-Matches 'FunctionTrace\.Enter|Console\.WriteLine\("Entering'
$controlBlockCount = Count-Matches '\b(if|for|foreach|while|switch)\s*\('
$controlTraceCount = Count-Matches 'FunctionTrace\.(If|Loop|Switch|Branch)\('

[pscustomobject]@{
    FunctionTraceLines = $functionTraceCount
    ControlBlocks = $controlBlockCount
    ControlTraceLines = $controlTraceCount
    ControlTraceGap = [Math]::Max(0, $controlBlockCount - $controlTraceCount)
}
