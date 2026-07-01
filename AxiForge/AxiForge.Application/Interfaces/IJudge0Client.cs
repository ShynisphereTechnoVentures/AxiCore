using AxiForge.Application.DTOs.Coding;

namespace AxiForge.Application.Interfaces;

public interface IJudge0Client
{
    Task<CodingSubmissionDto> ExecuteAsync(
        CreateSubmissionRequestDto request,
        IReadOnlyList<CodingTestCaseDto> testCases,
        CancellationToken cancellationToken);
}
