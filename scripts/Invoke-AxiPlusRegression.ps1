param(
    [string]$ApiBaseUrl = "http://localhost:5228"
)

$ErrorActionPreference = "Stop"

function Write-Step {
    param([string]$Message)

    Write-Host "[AxiPlus Regression] $Message"
}

function Invoke-Login {
    param(
        [string]$Email,
        [string]$Password,
        [string]$ExpectedRole
    )

    Write-Step "Login: $Email"

    $body = @{
        email = $Email
        password = $Password
    } | ConvertTo-Json

    $login = Invoke-RestMethod `
        -Method Post `
        -Uri "$ApiBaseUrl/api/auth/login" `
        -ContentType "application/json" `
        -Body $body

    if ($login.role -ne $ExpectedRole) {
        throw "Expected $ExpectedRole for $Email but got $($login.role)."
    }

    return $login
}

function Invoke-AuthorizedGet {
    param(
        [string]$Name,
        [string]$Url,
        [hashtable]$Headers
    )

    Write-Step "GET: $Name"

    $response = Invoke-WebRequest `
        -Method Get `
        -Uri $Url `
        -Headers $Headers `
        -UseBasicParsing `
        -TimeoutSec 15

    if ($response.StatusCode -ne 200) {
        throw "$Name returned HTTP $($response.StatusCode)."
    }
}

Write-Step "Starting against $ApiBaseUrl"

$accounts = @(
    @{ Email = "sa@axiplus.com"; Password = "sa@123"; Role = "SuperAdmin" },
    @{ Email = "admin@axiplus.com"; Password = "Admin@123"; Role = "Admin" },
    @{ Email = "mm@axiplus.com"; Password = "MM@123"; Role = "MainMentor" },
    @{ Email = "am@axiplus.com"; Password = "AM@123"; Role = "AssistantMentor" },
    @{ Email = "child@axiplus.com"; Password = "child@123"; Role = "Student" },
    @{ Email = "col@axiplus.com"; Password = "col@123"; Role = "CollegeCoordinator" }
)

foreach ($account in $accounts) {
    Invoke-Login `
        -Email $account.Email `
        -Password $account.Password `
        -ExpectedRole $account.Role | Out-Null
}

$studentLogin = Invoke-Login `
    -Email "child@axiplus.com" `
    -Password "child@123" `
    -ExpectedRole "Student"

$headers = @{
    Authorization = "Bearer $($studentLogin.token)"
}

$studentChecks = @(
    @{ Name = "dashboard"; Url = "$ApiBaseUrl/api/dashboard/student/me" },
    @{ Name = "profile"; Url = "$ApiBaseUrl/api/students/me" },
    @{ Name = "modules"; Url = "$ApiBaseUrl/api/module/student/me" },
    @{ Name = "payments"; Url = "$ApiBaseUrl/api/students/me/payments" },
    @{ Name = "plans"; Url = "$ApiBaseUrl/api/students/me/plans" },
    @{ Name = "upcoming-payment"; Url = "$ApiBaseUrl/api/students/me/payments/upcoming" },
    @{ Name = "live-classes"; Url = "$ApiBaseUrl/api/student-portal/live-classes" },
    @{ Name = "recordings"; Url = "$ApiBaseUrl/api/student-portal/recordings" },
    @{ Name = "practice"; Url = "$ApiBaseUrl/api/student-portal/practice" },
    @{ Name = "notifications"; Url = "$ApiBaseUrl/api/student-portal/notifications" },
    @{ Name = "support-tickets"; Url = "$ApiBaseUrl/api/student-portal/support-tickets" },
    @{ Name = "lessons-module-1"; Url = "$ApiBaseUrl/api/lesson/module/1" }
)

foreach ($check in $studentChecks) {
    Invoke-AuthorizedGet `
        -Name $check.Name `
        -Url $check.Url `
        -Headers $headers
}

$lessons = Invoke-RestMethod `
    -Method Get `
    -Uri "$ApiBaseUrl/api/lesson/module/1" `
    -Headers $headers

$lessonId = $lessons[0].id

Invoke-AuthorizedGet `
    -Name "lesson-details" `
    -Url "$ApiBaseUrl/api/lesson/student/me/details/$lessonId" `
    -Headers $headers

Write-Step "POST: practice launch"

$launch = Invoke-RestMethod `
    -Method Post `
    -Uri "$ApiBaseUrl/api/practice-launch/lessons/$lessonId" `
    -Headers $headers

if ([string]::IsNullOrWhiteSpace($launch.redirectUrl)) {
    throw "Practice launch did not return a redirect URL."
}

Write-Step "PASS"
