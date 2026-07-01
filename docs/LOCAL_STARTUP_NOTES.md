# Local Startup Notes

Date: June 29, 2026

## Current Web URLs

- AxiForge.Web: `http://localhost:5242`
- AxiPlus.Web: `http://localhost:5094`

## What Happened

AxiForge.Web was already running on port `5242`, so starting it again failed with:

`Failed to bind to address http://127.0.0.1:5242: address already in use.`

That means the app is already started, not broken.

AxiPlus.Web uses launch settings from:

`AxiPlus/AxiPlus.Web/Properties/launchSettings.json`

Its HTTP URL is `http://localhost:5094`. Opening `http://localhost:5092` will fail unless you explicitly start it on that port.

## Recommended Terminal Commands

Start AxiForge Web:

```powershell
dotnet run --project AxiForge\AxiForge.Web\AxiForge.Web.csproj --no-build
```

Open:

```text
http://localhost:5242/login
```

Start AxiPlus Web:

```powershell
dotnet run --project AxiPlus\AxiPlus.Web\AxiPlus.Web.csproj --no-build
```

Open:

```text
http://localhost:5094/login
```

## If A Port Is Already In Use

Find the process:

```powershell
netstat -ano | Select-String ':5242'
netstat -ano | Select-String ':5094'
```

Stop a specific process:

```powershell
Stop-Process -Id <PID> -Force
```

Only stop the PID shown for the port you want to free.

## Silent Terminal Is Normal

The web apps may not print much after startup because logging is filtered to warning and above. If the terminal command does not exit and the port is listening, the app is running.
