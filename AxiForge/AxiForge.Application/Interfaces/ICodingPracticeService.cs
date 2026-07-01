using AxiForge.Application.DTOs.Coding;

namespace AxiForge.Application.Interfaces;

public interface ICodingPracticeService
{
    Task<List<CodingProblemSummaryDto>> GetProblemsAsync(
        string? topic,
        string? difficulty,
        string? search,
        CancellationToken cancellationToken);

    Task<CodingProblemDetailDto?> GetProblemAsync(Guid problemId, CancellationToken cancellationToken);

    Task<CodingSubmissionDto> SubmitAsync(
        Guid accountId,
        CreateSubmissionRequestDto request,
        CancellationToken cancellationToken);

    Task<List<CodingSubmissionDto>> GetMySubmissionsAsync(
        Guid accountId,
        CancellationToken cancellationToken);
}
