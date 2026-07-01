using AxiForge.Application.DTOs.Assessments;

namespace AxiForge.Application.Interfaces;

public interface IAssessmentService
{
    Task<List<AssessmentTemplateDto>> GetTemplatesAsync(CancellationToken cancellationToken);

    Task<AssessmentDetailDto?> GetTemplateAsync(Guid assessmentId, CancellationToken cancellationToken);

    Task<AssessmentResultDto> SubmitAsync(Guid accountId, SubmitAssessmentRequestDto request, CancellationToken cancellationToken);

    Task<List<AssessmentResultDto>> GetMyResultsAsync(Guid accountId, CancellationToken cancellationToken);
}
