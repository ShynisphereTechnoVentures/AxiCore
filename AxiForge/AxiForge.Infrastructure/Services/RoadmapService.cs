using AxiCore.Diagnostics;
using AxiForge.Application.DTOs.Roadmaps;
using AxiForge.Application.Interfaces;
using AxiForge.Domain.Entities;
using AxiForge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AxiForge.Infrastructure.Services;

public sealed class RoadmapService : IRoadmapService
{
    private readonly AxiForgeDbContext _context;
    private readonly ILogger<RoadmapService> _logger;

    public RoadmapService(AxiForgeDbContext context, ILogger<RoadmapService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Gets published roadmap templates.
    /// Returns roadmap cards so students can choose a learning path.
    /// </summary>
    public async Task<List<RoadmapTemplateDto>> GetTemplatesAsync(CancellationToken cancellationToken)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(RoadmapService), nameof(GetTemplatesAsync));
        try
        {
            return await _context.RoadmapTemplates
                .Include(x => x.Steps)
                .Where(x => x.IsPublished)
                .OrderBy(x => x.Title)
                .Select(x => new RoadmapTemplateDto
                {
                    Id = x.Id,
                    Slug = x.Slug,
                    Title = x.Title,
                    Description = x.Description,
                    Level = x.Level,
                    StepCount = x.Steps.Count
                })
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }

    /// <summary>
    /// Gets one roadmap template with steps.
    /// Returns roadmap detail so the student can review the path before enrolling.
    /// </summary>
    public async Task<RoadmapDetailDto?> GetTemplateAsync(Guid roadmapId, CancellationToken cancellationToken)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(RoadmapService), nameof(GetTemplateAsync));
        try
        {
            var roadmap = await _context.RoadmapTemplates
                .Include(x => x.Steps)
                .FirstOrDefaultAsync(x => x.Id == roadmapId && x.IsPublished, cancellationToken);

            return roadmap == null ? null : ToDetail(roadmap);
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }

    /// <summary>
    /// Enrolls the authenticated student into a roadmap.
    /// Returns the student roadmap record so the dashboard can show active progress.
    /// </summary>
    public async Task<StudentRoadmapDto> EnrollAsync(Guid accountId, Guid roadmapId, CancellationToken cancellationToken)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(RoadmapService), nameof(EnrollAsync));
        try
        {
            var existing = await _context.StudentRoadmaps
                .Include(x => x.RoadmapTemplate)
                .FirstOrDefaultAsync(x => x.AccountId == accountId && x.RoadmapTemplateId == roadmapId, cancellationToken);

            if (existing != null)
            {
                return ToStudentDto(existing);
            }

            var roadmap = await _context.RoadmapTemplates
                .FirstAsync(x => x.Id == roadmapId && x.IsPublished, cancellationToken);

            var studentRoadmap = new StudentRoadmap
            {
                AccountId = accountId,
                RoadmapTemplateId = roadmap.Id,
                RoadmapTemplate = roadmap,
                ProgressPercentage = 0,
                EnrolledAt = DateTime.UtcNow
            };

            _context.StudentRoadmaps.Add(studentRoadmap);
            await _context.SaveChangesAsync(cancellationToken);

            return ToStudentDto(studentRoadmap);
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }

    /// <summary>
    /// Gets roadmaps enrolled by the authenticated student.
    /// Returns active roadmap progress for dashboard and roadmap views.
    /// </summary>
    public async Task<List<StudentRoadmapDto>> GetMyRoadmapsAsync(Guid accountId, CancellationToken cancellationToken)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(RoadmapService), nameof(GetMyRoadmapsAsync));
        try
        {
            return await _context.StudentRoadmaps
                .Include(x => x.RoadmapTemplate)
                .Where(x => x.AccountId == accountId)
                .OrderByDescending(x => x.EnrolledAt)
                .Select(x => new StudentRoadmapDto
                {
                    Id = x.Id,
                    RoadmapTemplateId = x.RoadmapTemplateId,
                    Title = x.RoadmapTemplate.Title,
                    ProgressPercentage = x.ProgressPercentage,
                    EnrolledAt = x.EnrolledAt
                })
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }

    private static RoadmapDetailDto ToDetail(RoadmapTemplate roadmap)
    {
        Console.WriteLine("Entering -> RoadmapService.cs -> ToDetail");
        try
        {
            return new RoadmapDetailDto
            {
                Id = roadmap.Id,
                Slug = roadmap.Slug,
                Title = roadmap.Title,
                Description = roadmap.Description,
                Level = roadmap.Level,
                StepCount = roadmap.Steps.Count,
                Steps = roadmap.Steps
                    .OrderBy(x => x.Order)
                    .Select(x => new RoadmapStepDto
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Description = x.Description,
                        Order = x.Order
                    })
                    .ToList()
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> RoadmapService.cs -> ToDetail -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> RoadmapService.cs -> ToDetail");
        }
    }

    private static StudentRoadmapDto ToStudentDto(StudentRoadmap roadmap)
    {
        Console.WriteLine("Entering -> RoadmapService.cs -> ToStudentDto");
        try
        {
            return new StudentRoadmapDto
            {
                Id = roadmap.Id,
                RoadmapTemplateId = roadmap.RoadmapTemplateId,
                Title = roadmap.RoadmapTemplate.Title,
                ProgressPercentage = roadmap.ProgressPercentage,
                EnrolledAt = roadmap.EnrolledAt
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> RoadmapService.cs -> ToStudentDto -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> RoadmapService.cs -> ToStudentDto");
        }
    }
}
