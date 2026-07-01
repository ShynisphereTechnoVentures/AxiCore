using AxiPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AxiPlus.Infrastructure.Data;

public class AppDbContext : DbContext
{     
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
   {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Student> Students => Set<Student>();

    public DbSet<Batch> Batches => Set<Batch>();

    public DbSet<Session> Sessions => Set<Session>();

    public DbSet<Attendance> Attendances => Set<Attendance>();

    public DbSet<Assignment> Assignments => Set<Assignment>();

    public DbSet<AssignmentSubmission> AssignmentSubmissions => Set<AssignmentSubmission>();

    public DbSet<StudentNotification> StudentNotifications => Set<StudentNotification>();

    public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();

    public DbSet<MentorProfile> MentorProfiles => Set<MentorProfile>();

    public DbSet<SalarySlip> SalarySlips => Set<SalarySlip>();

    public DbSet<MeetingRequest> MeetingRequests => Set<MeetingRequest>();

    public DbSet<AttendanceDiscrepancy> AttendanceDiscrepancies =>
        Set<AttendanceDiscrepancy>();

    public DbSet<StudentBillingAccount> StudentBillingAccounts =>
        Set<StudentBillingAccount>();

    public DbSet<StudentPayment> StudentPayments => Set<StudentPayment>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<Track> Tracks => Set<Track>();

    public DbSet<Module> Modules => Set<Module>();

    public DbSet<Lesson> Lessons => Set<Lesson>();

    public DbSet<ModuleResource> ModuleResources => Set<ModuleResource>();

    public DbSet<TrackModule> TrackModules => Set<TrackModule>();

    protected override void OnModelCreating(ModelBuilder builder)
   {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly( typeof(AppDbContext).Assembly);
    }

    public DbSet<LessonLiveClass> LessonLiveClasses => Set<LessonLiveClass>();

    public DbSet<StudentLessonProgress> StudentLessonProgresses => Set<StudentLessonProgress>();

    public DbSet<StudentModule>StudentModules => Set<StudentModule>();


}
