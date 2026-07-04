using AxiCore.Diagnostics;
using AxiHire.Application.DTOs;
using AxiHire.Application.Interfaces;
using AxiHire.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AxiHire.Infrastructure.Services;

public sealed class CandidateVerificationService : ICandidateVerificationService
{
    private readonly AxiHireDbContext _context;
    private readonly ILogger<CandidateVerificationService> _logger;

    public CandidateVerificationService(
        AxiHireDbContext context,
        ILogger<CandidateVerificationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Gets recruiter-safe candidate passport summaries.
    /// Returns only verification-ready summary data and avoids raw LMS or practice history.
    /// </summary>
    public async Task<List<CandidateSummaryDto>> GetCandidatesAsync(
        CancellationToken cancellationToken = default)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(CandidateVerificationService), nameof(GetCandidatesAsync));
        try
        {
            return await _context.CandidatePassportSnapshots
                .AsNoTracking()
                .OrderByDescending(x => x.ReadinessScore)
                .Select(x => new CandidateSummaryDto
                {
                    Id = x.Id,
                    DisplayName = x.DisplayName,
                    Headline = x.Headline,
                    PrimarySkill = x.PrimarySkill,
                    ReadinessScore = x.ReadinessScore,
                    VerificationStatus = x.VerificationStatus,
                    LastSyncedAt = x.LastSyncedAt
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
    /// Gets one recruiter-safe candidate passport with verification invite history.
    /// Returns null when the candidate summary is not available to AxiHire.
    /// </summary>
    public async Task<CandidateDetailDto?> GetCandidateAsync(
        Guid candidateId,
        CancellationToken cancellationToken = default)
    {
        using var trace = FunctionTrace.Enter(_logger, nameof(CandidateVerificationService), nameof(GetCandidateAsync));
        try
        {
            var candidate = await _context.CandidatePassportSnapshots
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == candidateId, cancellationToken);

            if (candidate == null)
            {
                return null;
            }

            var invites = await _context.CandidateVerificationInvites
                .AsNoTracking()
                .Include(x => x.RecruiterOrganization)
                .Where(x => x.CandidatePassportSnapshotId == candidate.Id)
                .OrderByDescending(x => x.InvitedAt)
                .Select(x => new CandidateInviteDto
                {
                    Id = x.Id,
                    RecruiterOrganization = x.RecruiterOrganization.Name,
                    Status = x.Status,
                    InvitedAt = x.InvitedAt,
                    ExpiresAt = x.ExpiresAt
                })
                .ToListAsync(cancellationToken);

            return new CandidateDetailDto
            {
                Id = candidate.Id,
                DisplayName = candidate.DisplayName,
                Headline = candidate.Headline,
                PrimarySkill = candidate.PrimarySkill,
                SkillSummary = candidate.SkillSummary,
                ReadinessScore = candidate.ReadinessScore,
                VerificationStatus = candidate.VerificationStatus,
                LastSyncedAt = candidate.LastSyncedAt,
                Invites = invites
            };
        }
        catch (Exception ex)
        {
            trace.Exception(ex);
            throw;
        }
    }
}
