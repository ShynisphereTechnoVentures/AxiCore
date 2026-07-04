using AxiCore.Identity;
using AxiCore.Persistence;
using AxiPlus.Domain.Entities;
using AxiPlus.Domain.Enums;
using AxiPlus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AxiPlus.API.Seed;

public static class AxiPlusDemoDataSeeder
{
    private static readonly string[] RoleNames =
    {
        "SuperAdmin",
        "Admin",
        "MainMentor",
        "AssistantMentor",
        "Student",
        "CollegeCoordinator"
    };

    public static async Task SeedAsync(
        AppDbContext context,
        AxiCoreDbContext axiCoreContext,
        string? mode,
        CancellationToken cancellationToken)
    {
        Console.WriteLine("Entering -> AxiPlus.API -> Seed -> AxiPlusDemoDataSeeder.cs -> SeedAsync");
        try
        {
            var normalizedMode = string.IsNullOrWhiteSpace(mode)
                ? "Required"
                : mode.Trim();

            if (normalizedMode.Equals("None", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            await EnsureRolesAsync(context, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            if (!normalizedMode.Equals("Demo", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            await EnsureDemoDataAsync(context, axiCoreContext, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            await axiCoreContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiPlus.API -> Seed -> AxiPlusDemoDataSeeder.cs -> SeedAsync -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiPlus.API -> Seed -> AxiPlusDemoDataSeeder.cs -> SeedAsync");
        }
    }

    private static async Task EnsureRolesAsync(
        AppDbContext context,
        CancellationToken cancellationToken)
    {
        foreach (var roleName in RoleNames)
        {
            var exists = await context.Roles
                .AnyAsync(x => x.Name == roleName, cancellationToken);

            if (!exists)
            {
                context.Roles.Add(new Role { Name = roleName });
            }
        }
    }

    private static async Task EnsureDemoDataAsync(
        AppDbContext context,
        AxiCoreDbContext axiCoreContext,
        CancellationToken cancellationToken)
    {
        var roles = await context.Roles
            .ToDictionaryAsync(x => x.Name, cancellationToken);

        var superAdmin = await EnsureUserAsync(
            context,
            roles["SuperAdmin"],
            "AxiPlus SA",
            "sa@axiplus.com",
            "sa@123",
            cancellationToken);

        var admin = await EnsureUserAsync(
            context,
            roles["Admin"],
            "AxiPlus Admin",
            "admin@axiplus.com",
            "Admin@123",
            cancellationToken);

        var mainMentor = await EnsureUserAsync(
            context,
            roles["MainMentor"],
            "Main Mentor",
            "mm@axiplus.com",
            "MM@123",
            cancellationToken);

        var assistantMentor = await EnsureUserAsync(
            context,
            roles["AssistantMentor"],
            "Assistant Mentor",
            "am@axiplus.com",
            "AM@123",
            cancellationToken);

        var studentUser = await EnsureUserAsync(
            context,
            roles["Student"],
            "Child Student",
            "child@axiplus.com",
            "child@123",
            cancellationToken);

        await EnsureUserAsync(
            context,
            roles["CollegeCoordinator"],
            "College Coordinator",
            "col@axiplus.com",
            "col@123",
            cancellationToken);

        var track = await EnsureTrackAsync(context, cancellationToken);
        var module = await EnsureModuleAsync(context, cancellationToken);
        await EnsureTrackModuleAsync(context, track, module, cancellationToken);

        var lesson = await EnsureLessonAsync(context, module, cancellationToken);
        var batch = await EnsureBatchAsync(
            context,
            track,
            mainMentor,
            assistantMentor,
            cancellationToken);

        var student = await EnsureStudentAsync(
            context,
            studentUser,
            batch,
            track,
            cancellationToken);

        await EnsureStudentModuleAsync(context, student, module, cancellationToken);
        await EnsureLessonProgressAsync(context, student, lesson, cancellationToken);
        await EnsureBillingAsync(context, student, cancellationToken);
        await EnsurePortalDataAsync(context, student, batch, lesson, cancellationToken);
        await EnsureAxiCoreStudentAsync(axiCoreContext, studentUser, cancellationToken);

        _ = superAdmin;
        _ = admin;
    }

    private static async Task<User> EnsureUserAsync(
        AppDbContext context,
        Role role,
        string fullName,
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        if (user == null)
        {
            user = new User
            {
                FullName = fullName,
                Email = email,
                PasswordHash = passwordHash,
                RoleId = role.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(user);
            return user;
        }

        user.FullName = fullName;
        user.PasswordHash = passwordHash;
        user.RoleId = role.Id;
        user.IsActive = true;
        user.UpdatedAt = DateTime.UtcNow;

        return user;
    }

    private static async Task<Track> EnsureTrackAsync(
        AppDbContext context,
        CancellationToken cancellationToken)
    {
        var track = await context.Tracks
            .FirstOrDefaultAsync(x => x.Name == "Foundation", cancellationToken);

        if (track != null)
        {
            track.IsActive = true;
            return track;
        }

        track = new Track
        {
            Name = "Foundation",
            Level = "Beginner",
            BatchPrefix = "AXF",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Tracks.Add(track);
        return track;
    }

    private static async Task<Module> EnsureModuleAsync(
        AppDbContext context,
        CancellationToken cancellationToken)
    {
        var module = await context.Modules
            .FirstOrDefaultAsync(x => x.Id == 1, cancellationToken);

        if (module == null)
        {
            module = new Module
            {
                Id = 1,
                Title = "Foundation Module",
                Description = "Regression-ready foundation learning module.",
                IsPublished = true,
                IsActive = true,
                Order = 1,
                CreatedAt = DateTime.UtcNow
            };

            context.Modules.Add(module);
            return module;
        }

        module.Title = string.IsNullOrWhiteSpace(module.Title)
            ? "Foundation Module"
            : module.Title;
        module.Description = string.IsNullOrWhiteSpace(module.Description)
            ? "Regression-ready foundation learning module."
            : module.Description;
        module.IsPublished = true;
        module.IsActive = true;
        module.Order = module.Order == 0 ? 1 : module.Order;

        return module;
    }

    private static async Task EnsureTrackModuleAsync(
        AppDbContext context,
        Track track,
        Module module,
        CancellationToken cancellationToken)
    {
        var exists = await context.TrackModules
            .AnyAsync(
                x => x.TrackId == track.Id && x.ModuleId == module.Id,
                cancellationToken);

        if (!exists)
        {
            context.TrackModules.Add(new TrackModule
            {
                Track = track,
                Module = module,
                Order = 1
            });
        }
    }

    private static async Task<Lesson> EnsureLessonAsync(
        AppDbContext context,
        Module module,
        CancellationToken cancellationToken)
    {
        var lesson = await context.Lessons
            .FirstOrDefaultAsync(
                x => x.ModuleId == module.Id && x.Order == 1,
                cancellationToken);

        if (lesson == null)
        {
            lesson = new Lesson
            {
                Module = module,
                Title = "Regression Practice Lesson",
                Description = "Stable lesson for API and launch regression.",
                Content = "Complete the linked practice exercise.",
                Order = 1,
                IsPublished = true,
                PracticeLink = "regression-practice",
                CreatedAt = DateTime.UtcNow
            };

            context.Lessons.Add(lesson);
            return lesson;
        }

        lesson.Title = string.IsNullOrWhiteSpace(lesson.Title)
            ? "Regression Practice Lesson"
            : lesson.Title;
        lesson.Description = string.IsNullOrWhiteSpace(lesson.Description)
            ? "Stable lesson for API and launch regression."
            : lesson.Description;
        lesson.Content = string.IsNullOrWhiteSpace(lesson.Content)
            ? "Complete the linked practice exercise."
            : lesson.Content;
        lesson.IsPublished = true;
        lesson.PracticeLink = string.IsNullOrWhiteSpace(lesson.PracticeLink)
            ? "regression-practice"
            : lesson.PracticeLink;

        return lesson;
    }

    private static async Task<Batch> EnsureBatchAsync(
        AppDbContext context,
        Track track,
        User mainMentor,
        User assistantMentor,
        CancellationToken cancellationToken)
    {
        var batch = await context.Batches
            .FirstOrDefaultAsync(x => x.Name == "AXF-REG-001", cancellationToken);

        if (batch == null)
        {
            batch = new Batch
            {
                Name = "AXF-REG-001",
                BatchNumber = 1,
                Track = track,
                Level = track.Level,
                Capacity = 30,
                CurrentStrength = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                MentorId = mainMentor.Id,
                AssistantMentorId = assistantMentor.Id
            };

            context.Batches.Add(batch);
            return batch;
        }

        batch.Track = track;
        batch.Level = string.IsNullOrWhiteSpace(batch.Level) ? track.Level : batch.Level;
        batch.CurrentStrength = Math.Max(batch.CurrentStrength, 1);
        batch.IsActive = true;
        batch.MentorId = mainMentor.Id;
        batch.AssistantMentorId = assistantMentor.Id;

        return batch;
    }

    private static async Task<Student> EnsureStudentAsync(
        AppDbContext context,
        User studentUser,
        Batch batch,
        Track track,
        CancellationToken cancellationToken)
    {
        var student = await context.Students
            .FirstOrDefaultAsync(x => x.Email == studentUser.Email, cancellationToken);

        if (student == null)
        {
            student = new Student
            {
                UserId = studentUser.Id,
                Batch = batch,
                Track = track,
                FullName = studentUser.FullName,
                Email = studentUser.Email,
                PhoneNumber = "9999999999",
                CollegeName = "Regression College",
                JoinedDate = DateTime.UtcNow.Date,
                Gender = "Not specified",
                Address = "Regression Address",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.Students.Add(student);
            return student;
        }

        student.UserId = studentUser.Id;
        student.Batch = batch;
        student.Track = track;
        student.FullName = studentUser.FullName;
        student.Email = studentUser.Email;
        student.PhoneNumber = string.IsNullOrWhiteSpace(student.PhoneNumber)
            ? "9999999999"
            : student.PhoneNumber;
        student.CollegeName = string.IsNullOrWhiteSpace(student.CollegeName)
            ? "Regression College"
            : student.CollegeName;
        student.UpdatedAt = DateTime.UtcNow;

        return student;
    }

    private static async Task EnsureStudentModuleAsync(
        AppDbContext context,
        Student student,
        Module module,
        CancellationToken cancellationToken)
    {
        var exists = await context.StudentModules
            .AnyAsync(
                x => x.StudentId == student.Id && x.ModuleId == module.Id,
                cancellationToken);

        if (!exists)
        {
            context.StudentModules.Add(new StudentModule
            {
                Student = student,
                Module = module,
                IsUnlocked = true,
                IsCompleted = false,
                ExamPassed = false,
                ExamAttempts = 0,
                ProgressPercentage = 0,
                Status = ModuleStatus.Active,
                AssignedAt = DateTime.UtcNow
            });
        }
    }

    private static async Task EnsureLessonProgressAsync(
        AppDbContext context,
        Student student,
        Lesson lesson,
        CancellationToken cancellationToken)
    {
        var exists = await context.StudentLessonProgresses
            .AnyAsync(
                x => x.StudentId == student.Id && x.LessonId == lesson.Id,
                cancellationToken);

        if (!exists)
        {
            context.StudentLessonProgresses.Add(new StudentLessonProgress
            {
                Student = student,
                Lesson = lesson,
                Status = LessonStatus.LiveSessionPending,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            });
        }
    }

    private static async Task EnsureBillingAsync(
        AppDbContext context,
        Student student,
        CancellationToken cancellationToken)
    {
        var billing = await context.StudentBillingAccounts
            .FirstOrDefaultAsync(x => x.StudentId == student.Id, cancellationToken);

        var nextDueDate = DateTime.UtcNow.Date.AddDays(15);

        if (billing == null)
        {
            context.StudentBillingAccounts.Add(new StudentBillingAccount
            {
                Student = student,
                MonthlyFee = 0,
                Currency = "INR",
                NextDueDate = nextDueDate,
                GraceEndsAt = nextDueDate.AddDays(15),
                Status = BillingStatus.Trial,
                AutoPayEnabled = false,
                UpiId = "child@upi",
                CreatedAt = DateTime.UtcNow
            });

            return;
        }

        billing.MonthlyFee = 0;
        billing.Currency = string.IsNullOrWhiteSpace(billing.Currency) ? "INR" : billing.Currency;
        billing.NextDueDate = billing.NextDueDate <= DateTime.UtcNow.Date
            ? nextDueDate
            : billing.NextDueDate;
        billing.GraceEndsAt = billing.NextDueDate.AddDays(15);
        billing.Status = BillingStatus.Trial;
        billing.UpdatedAt = DateTime.UtcNow;
    }

    private static async Task EnsurePortalDataAsync(
        AppDbContext context,
        Student student,
        Batch batch,
        Lesson lesson,
        CancellationToken cancellationToken)
    {
        if (!await context.Sessions.AnyAsync(x => x.BatchId == batch.Id, cancellationToken))
        {
            context.Sessions.Add(new Session
            {
                BatchId = batch.Id,
                Title = "Regression Live Class",
                MeetLink = "https://meet.axionora.local/regression",
                StartTime = DateTime.UtcNow.AddHours(1),
                EndTime = DateTime.UtcNow.AddHours(2),
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.LessonLiveClasses.AnyAsync(x => x.LessonId == lesson.Id, cancellationToken))
        {
            context.LessonLiveClasses.Add(new LessonLiveClass
            {
                Lesson = lesson,
                MeetingLink = "https://meet.axionora.local/lesson-regression",
                RecordingLink = "https://cdn.axionora.local/recordings/regression.mp4",
                ScheduledAt = DateTime.UtcNow.AddDays(-1),
                IsCompleted = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.StudentNotifications.AnyAsync(x => x.StudentId == student.Id, cancellationToken))
        {
            context.StudentNotifications.Add(new StudentNotification
            {
                Student = student,
                Title = "Regression notification",
                Message = "Your regression class is ready.",
                Type = "Info",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (!await context.SupportTickets.AnyAsync(x => x.StudentId == student.Id, cancellationToken))
        {
            context.SupportTickets.Add(new SupportTicket
            {
                Student = student,
                Subject = "Regression support ticket",
                Message = "Seeded ticket for regression smoke tests.",
                Status = SupportTicketStatus.Open,
                CreatedAt = DateTime.UtcNow
            });
        }
    }

    private static async Task EnsureAxiCoreStudentAsync(
        AxiCoreDbContext context,
        User studentUser,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = studentUser.Email.Trim().ToLowerInvariant();
        var user = await context.Users
            .FirstOrDefaultAsync(x => x.Email == normalizedEmail, cancellationToken);

        if (user == null)
        {
            user = new AxiCoreUser
            {
                FullName = studentUser.FullName,
                Email = normalizedEmail,
                PasswordHash = studentUser.PasswordHash,
                IsActive = true,
                EmailConfirmed = false,
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(user);
        }
        else
        {
            user.FullName = studentUser.FullName;
            user.PasswordHash = string.IsNullOrWhiteSpace(user.PasswordHash)
                ? studentUser.PasswordHash
                : user.PasswordHash;
            user.IsActive = true;
        }

        await EnsureAxiCoreRoleAsync(context, user, AxiCoreRoleNames.Student, cancellationToken);
        await EnsureProductAccessAsync(context, user, AxiCoreProductCodes.AxiPlus, cancellationToken);
        await EnsureProductAccessAsync(context, user, AxiCoreProductCodes.AxiForge, cancellationToken);
    }

    private static async Task EnsureAxiCoreRoleAsync(
        AxiCoreDbContext context,
        AxiCoreUser user,
        string roleName,
        CancellationToken cancellationToken)
    {
        var role = await context.Roles
            .FirstOrDefaultAsync(x => x.Name == roleName, cancellationToken);

        if (role == null)
        {
            role = new AxiCoreRole { Name = roleName };
            context.Roles.Add(role);
            await context.SaveChangesAsync(cancellationToken);
        }

        var exists = await context.UserRoles
            .AnyAsync(x => x.UserId == user.Id && x.RoleId == role.Id, cancellationToken);

        if (!exists)
        {
            context.UserRoles.Add(new AxiCoreUserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            });
        }
    }

    private static async Task EnsureProductAccessAsync(
        AxiCoreDbContext context,
        AxiCoreUser user,
        string productCode,
        CancellationToken cancellationToken)
    {
        var exists = await context.UserProductAccess
            .AnyAsync(
                x => x.UserId == user.Id && x.ProductCode == productCode,
                cancellationToken);

        if (!exists)
        {
            context.UserProductAccess.Add(new AxiCoreProductAccess
            {
                UserId = user.Id,
                ProductCode = productCode,
                Status = AxiCoreProductAccessStatuses.Active,
                GrantedAt = DateTime.UtcNow
            });
        }
    }
}
