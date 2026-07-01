using AxiPlus.Application.DTOs.Students;
using AxiPlus.API.Filters;
using AxiPlus.Domain.Entities;
using AxiPlus.Infrastructure.Data;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using AxiPlus.API.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using AxiPlus.Domain.Enums;
using System.Reflection;
using AxiCore.Identity;

namespace AxiPlus.API.Endpoints;

public static class StudentEndpoints
{        
    public static void MapStudentEndpoints(
        this WebApplication app)
   {
        var group = app.MapGroup("/api/students");
        group.AddEndpointFilter<FunctionTraceEndpointFilter>();

        group.MapPost("/register", async (
    RegisterStudentDto dto,
    AppDbContext context,
    BatchAllocationService batchService,
    AxiCoreAccountProvisioningService axiCoreProvisioningService,
    CancellationToken cancellationToken) =>
       {
            // CHECK EXISTING EMAIL
            var exists = await context.Users
                .AnyAsync(x => x.Email == dto.Email);

            if (exists)
           {
                return Results.BadRequest(
                    "Email already exists");
            }

            // FIND FOUNDATION TRACK
            var foundationTrack = await context.Tracks
                .FirstOrDefaultAsync(
                    x => x.Name == "Foundation");

            if (foundationTrack == null)
           {
                return Results.BadRequest(
                    "Foundation track not found");
            }

            var allocatedBatch =
    await batchService
        .AllocateBatchAsync(
            foundationTrack);

            // CREATE USER
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(
                dto.Password);

            var user = new User
           {        
                Id = Guid.NewGuid(),

                FullName = dto.FullName,

                Email = dto.Email,

                PasswordHash = passwordHash,

                RoleId = 5,

                IsActive = true,

                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(user);

            // CREATE STUDENT PROFILE
            var student = new Student
           {      
                Id = Guid.NewGuid(),

                UserId = user.Id,

                BatchId = allocatedBatch.Id,

                TrackId = foundationTrack.Id,

                FullName = dto.FullName,

                Email = dto.Email,

                PhoneNumber = dto.PhoneNumber,

                CollegeName = "",

                JoinedDate = DateTime.UtcNow,

                Gender = "",

                Address = "",

                CreatedAt = DateTime.UtcNow,

                UpdatedAt = DateTime.UtcNow
            };

            context.Students.Add(student);

            context.StudentBillingAccounts.Add(
                CreateBillingAccount(student.Id));

            await context.SaveChangesAsync(cancellationToken);

            await axiCoreProvisioningService.ProvisionStudentAsync(
                dto.FullName,
                dto.Email,
                passwordHash,
                new[] { AxiCoreProductCodes.AxiPlus, AxiCoreProductCodes.AxiForge },
                cancellationToken);

            return Results.Ok(
                "Student registered successfully. AxiPlus and AxiForge access created; email confirmation is pending.");
        });

        group.MapGet("/me",
        [Authorize(Roles = "Student")]
        async (
        ClaimsPrincipal user,
        AppDbContext context) =>
   {
        var email = user
            .FindFirst(ClaimTypes.Email)
            ?.Value;

        if (string.IsNullOrEmpty(email))
       {
            return Results.Unauthorized();
        }

        var student = await context.Students
            .Include(x => x.Batch)
            .Include(x => x.Track)
            .Include(x => x.BillingAccount)
            .FirstOrDefaultAsync(x =>
                x.Email == email);

        if (student == null)
       {
            return Results.NotFound();
        }

        var billing = await EnsureBillingAccountAsync(context, student);

        return Results.Ok(
            new StudentProfileDto
           {       
                FullName = student.FullName,

                Email = student.Email,

                Batch = student.Batch.Name,

                Track = student.Track.Name,

                JoinedDate = student.JoinedDate,

                PhoneNumber = student.PhoneNumber,

                Billing = MapBilling(billing)
            });
    });

        group.MapPut("/me/billing",
        [Authorize(Roles = "Student")]
        async (
        UpdateStudentBillingDto dto,
        ClaimsPrincipal user,
        AppDbContext context) =>
   {
        var student = await GetStudentAsync(user, context);

        if (student == null)
       {
            return Results.NotFound();
        }

        var billing = await EnsureBillingAccountAsync(context, student);
        billing.AutoPayEnabled = dto.AutoPayEnabled;
        billing.UpiId = dto.UpiId.Trim();
        billing.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        return Results.Ok(MapBilling(billing));
    });

        group.MapGet("/me/payments",
        [Authorize(Roles = "Student")]
        async (
        ClaimsPrincipal user,
        AppDbContext context) =>
   {
        var student = await GetStudentAsync(user, context);

        if (student == null)
       {
            return Results.NotFound();
        }

        var payments = await context.StudentPayments
            .Where(x => x.StudentId == student.Id)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => MapPayment(x))
            .ToListAsync();

        return Results.Ok(payments);
    });

        group.MapGet("/me/payments/upcoming",
        [Authorize(Roles = "Student")]
        async (
        ClaimsPrincipal user,
        AppDbContext context) =>
   {
        var student = await GetStudentAsync(user, context);

        if (student == null)
       {
            return Results.NotFound();
        }

        var billing = await EnsureBillingAccountAsync(context, student);

        return Results.Ok(new StudentPaymentDto
       {      
            PaymentId = Guid.Empty,
            Amount = billing.MonthlyFee,
            Currency = billing.Currency,
            Status = PaymentStatus.Pending,
            Method = billing.AutoPayEnabled ? "UPI Autopay" : "UPI",
            UpiId = billing.UpiId,
            ProviderReference = "Upcoming",
            DueDate = billing.NextDueDate,
            GraceEndsAt = billing.GraceEndsAt,
            CreatedAt = DateTime.UtcNow
        });
    });

        group.MapGet("/me/plans",
        [Authorize(Roles = "Student")]
        async (
        ClaimsPrincipal user,
        AppDbContext context) =>
   {
        var student = await GetStudentAsync(user, context);

        if (student == null)
       {
            return Results.NotFound();
        }

        var billing = await EnsureBillingAccountAsync(context, student);

        return Results.Ok(GetPlans(billing));
    });

        group.MapPost("/me/plans/{planCode}",
        [Authorize(Roles = "Student")]
        async (
        string planCode,
        ClaimsPrincipal user,
        AppDbContext context) =>
   {
        var student = await GetStudentAsync(user, context);

        if (student == null)
       {
            return Results.NotFound();
        }

        var plan = GetPlans(null)
            .FirstOrDefault(x =>
                string.Equals(x.Code, planCode, StringComparison.OrdinalIgnoreCase));

        if (plan == null)
       {
            return Results.BadRequest("Plan not found.");
        }

        var billing = await EnsureBillingAccountAsync(context, student);
        billing.MonthlyFee = plan.MonthlyFee;
        billing.Currency = plan.Currency;
        billing.Status = BillingStatus.PaymentDue;
        billing.NextDueDate = DateTime.UtcNow.Date.AddDays(15);
        billing.GraceEndsAt = billing.NextDueDate.AddDays(15);
        billing.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        return Results.Ok(MapBilling(billing));
    });

        group.MapPost("/me/payments/upi",
        [Authorize(Roles = "Student")]
        async (
        UpdateStudentBillingDto dto,
        ClaimsPrincipal user,
        AppDbContext context) =>
   {
        var student = await GetStudentAsync(user, context);

        if (student == null)
       {
            return Results.NotFound();
        }

        if (string.IsNullOrWhiteSpace(dto.UpiId))
       {
            return Results.BadRequest("UPI ID is required.");
        }

        var billing = await EnsureBillingAccountAsync(context, student);
        billing.UpiId = dto.UpiId.Trim();
        billing.AutoPayEnabled = dto.AutoPayEnabled;
        billing.Status = BillingStatus.GracePeriod;
        billing.GraceEndsAt = billing.NextDueDate.AddDays(15);
        billing.UpdatedAt = DateTime.UtcNow;

        var payment = new StudentPayment
       {      
            StudentId = student.Id,
            Amount = billing.MonthlyFee,
            Currency = billing.Currency,
            Status = PaymentStatus.Pending,
            UpiId = billing.UpiId,
            DueDate = billing.NextDueDate,
            GraceEndsAt = billing.GraceEndsAt,
            ProviderReference = $"UPI-{DateTime.UtcNow:yyyyMMddHHmmss}"
        };

        context.StudentPayments.Add(payment);
        await context.SaveChangesAsync();

        return Results.Ok(MapPayment(payment));
    });

    }

    private static async Task<Student?> GetStudentAsync(
        ClaimsPrincipal user,
        AppDbContext context)
   {
        var email = user.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrWhiteSpace(email))
       {
            return null;
        }

        return await context.Students
            .Include(x => x.BillingAccount)
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    private static StudentBillingAccount CreateBillingAccount(Guid studentId)
   {
        var dueDate = DateTime.UtcNow.Date.AddDays(15);

        return new StudentBillingAccount
       {      
            StudentId = studentId,
            MonthlyFee = 0,
            NextDueDate = dueDate,
            GraceEndsAt = dueDate.AddDays(15),
            Status = BillingStatus.Trial
        };
    }

    private static async Task<StudentBillingAccount> EnsureBillingAccountAsync(
        AppDbContext context,
        Student student)
   {
        var billing = student.BillingAccount ??
            await context.StudentBillingAccounts
                .FirstOrDefaultAsync(x => x.StudentId == student.Id);

        if (billing != null)
       {
            NormalizeBillingStatus(billing);
            return billing;
        }

        billing = CreateBillingAccount(student.Id);
        context.StudentBillingAccounts.Add(billing);
        await context.SaveChangesAsync();

        return billing;
    }

    private static void NormalizeBillingStatus(StudentBillingAccount billing)
   {
        var now = DateTime.UtcNow;

        if (billing.Status is BillingStatus.Active or BillingStatus.Trial)
       {
            return;
        }

        billing.Status = now > billing.GraceEndsAt
            ? BillingStatus.Locked
            : BillingStatus.GracePeriod;
    }

    private static StudentBillingDto MapBilling(StudentBillingAccount billing)
   {
        NormalizeBillingStatus(billing);

        return new StudentBillingDto
       {       
            CurrentPlanCode = GetCurrentPlanCode(billing.MonthlyFee),
            CurrentPlanName = GetCurrentPlanName(billing.MonthlyFee),
            MonthlyFee = billing.MonthlyFee,
            Currency = billing.Currency,
            NextDueDate = billing.NextDueDate,
            GraceEndsAt = billing.GraceEndsAt,
            Status = billing.Status,
            AutoPayEnabled = billing.AutoPayEnabled,
            UpiId = billing.UpiId,
            DaysRemaining = Math.Max(
                0,
                (billing.GraceEndsAt.Date - DateTime.UtcNow.Date).Days),
            IsLocked = billing.Status == BillingStatus.Locked
        };
    }

    private static StudentPaymentDto MapPayment(StudentPayment payment)
   {
        return new StudentPaymentDto
       {     
            PaymentId = payment.Id,
            Amount = payment.Amount,
            Currency = payment.Currency,
            Status = payment.Status,
            Method = payment.Method,
            UpiId = payment.UpiId,
            ProviderReference = payment.ProviderReference,
            DueDate = payment.DueDate,
            GraceEndsAt = payment.GraceEndsAt,
            CreatedAt = payment.CreatedAt,
            PaidAt = payment.PaidAt
        };
    }

    private static List<StudentPlanDto> GetPlans(StudentBillingAccount? billing)
   {
        var currentCode = billing == null
            ? string.Empty
            : GetCurrentPlanCode(billing.MonthlyFee);

        var plans = new List<StudentPlanDto>
       {       
            new()
           {      
                Code = "foundation",
                Name = "Foundation",
                Description = "Core LMS access for regular batch learning.",
                MonthlyFee = 0,
                Features = new List<string>
               {        
                    "Live classes",
                    "PDF notes",
                    "Assignments",
                    "Attendance support"
                }
            },
            new()
           {       
                Code = "plus",
                Name = "AxiPlus Plus",
                Description = "Adds interview preparation and priority support.",
                MonthlyFee = 2999,
                Features = new List<string>
               {       
                    "Everything in Foundation",
                    "AI interview platform access",
                    "Priority ticket review",
                    "Placement assistance"
                }
            },
            new()
           {        
                Code = "pro",
                Name = "AxiPlus Pro",
                Description = "Advanced mentoring for internship and placement readiness.",
                MonthlyFee = 4999,
                Features = new List<string>
               {        
                    "Everything in Plus",
                    "Dedicated mentor reviews",
                    "Project review support",
                    "Internship readiness tracking"
                }
            }
        };

        foreach (var plan in plans)
       {
            plan.IsCurrent = plan.Code == currentCode;
        }

        return plans;
    }

    private static string GetCurrentPlanCode(decimal monthlyFee)
   {
        return monthlyFee switch
       {        
            >= 4999 => "pro",
            >= 2999 => "plus",
            _ => "foundation"
        };
    }

    private static string GetCurrentPlanName(decimal monthlyFee)
   {
        return monthlyFee switch
       {       
            >= 4999 => "AxiPlus Pro",
            >= 2999 => "AxiPlus Plus",
            _ => "Foundation"
        };
    }

}
