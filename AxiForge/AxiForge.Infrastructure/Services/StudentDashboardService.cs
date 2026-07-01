using AxiCore.Diagnostics;
using AxiCore.Persistence;
using AxiForge.Application.DTOs.Dashboard;
using AxiForge.Application.Interfaces;
using AxiForge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AxiForge.Infrastructure.Services;

public sealed class StudentDashboardService : IStudentDashboardService
{
    private readonly AxiForgeDbContext _context;
    private readonly AxiCoreDbContext _coreContext;
    private readonly ILogger<StudentDashboardService> _logger;

    public StudentDashboardService(
        AxiForgeDbContext context,
        AxiCoreDbContext coreContext,
        ILogger<StudentDashboardService> logger)
    {
        _context = context;
        _coreContext = coreContext;
        _logger = logger;
    }

    /// <summary>
    /// Gets the authenticated student's AxiForge dashboard shell data.
    /// Returns initial readiness metrics so the portal has a stable Phase 2 landing surface.
    /// </summary>
    public async Task<StudentDashboardDto> GetDashboardAsync(Guid accountId, CancellationToken cancellationToken)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(StudentDashboardService), nameof(GetDashboardAsync));
        try
        {
            var account = await _coreContext.Users
                .FirstOrDefaultAsync(x => x.Id == accountId, cancellationToken);

            return new StudentDashboardDto
            {
                StudentName = account?.FullName ?? "AxiForge Student",
                SolvedProblems = 0,
                ActiveRoadmaps = 0,
                PendingAssessments = 0,
                CurrentFocus = "Set up coding practice"
            };
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }
}
