using AxiHire.Application.DTOs;

namespace AxiHire.Application.Interfaces;

public interface ICandidateVerificationService
{
    Task<List<CandidateSummaryDto>> GetCandidatesAsync(
        CancellationToken cancellationToken = default);

    Task<CandidateDetailDto?> GetCandidateAsync(
        Guid candidateId,
        CancellationToken cancellationToken = default);
}
