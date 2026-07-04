using AxiHire.Domain.Entities;
using AxiHire.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AxiHire.Infrastructure.Seed;

public static class AxiHireSeedData
{
    public static async Task SeedAsync(
        AxiHireDbContext context,
        CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Entering -> AxiHire.Infrastructure -> Seed -> AxiHireSeedData.cs -> SeedAsync");
        try
        {
            if (await context.CandidatePassportSnapshots.AnyAsync(cancellationToken))
            {
                return;
            }

            var candidate = new CandidatePassportSnapshot
            {
                AxiCoreUserId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                DisplayName = "Regression Candidate",
                Headline = "Junior full-stack learner ready for verification",
                PrimarySkill = "C# / Blazor",
                SkillSummary = "Recruiter-safe summary generated from verified learning activity. Raw LMS history stays outside AxiHire.",
                ReadinessScore = 72,
                VerificationStatus = "Verified",
                LastSyncedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            var organization = new RecruiterOrganization
            {
                Name = "Axionora Demo Recruiter",
                ContactEmail = "recruiter.demo@axionora.com",
                IsVerified = true,
                CreatedAt = DateTime.UtcNow
            };

            context.CandidatePassportSnapshots.Add(candidate);
            context.RecruiterOrganizations.Add(organization);
            context.CandidateVerificationInvites.Add(new CandidateVerificationInvite
            {
                CandidatePassportSnapshot = candidate,
                RecruiterOrganization = organization,
                Status = "Invited",
                InvitedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(14)
            });

            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception -> AxiHire.Infrastructure -> Seed -> AxiHireSeedData.cs -> SeedAsync -> {ex.Message}");
            throw;
        }
        finally
        {
            Console.WriteLine("Exiting -> AxiHire.Infrastructure -> Seed -> AxiHireSeedData.cs -> SeedAsync");
        }
    }
}
