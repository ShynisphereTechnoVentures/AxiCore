using AxiForge.Application.DTOs.Roadmaps;

namespace AxiForge.Application.Interfaces;

public interface IRoadmapService
{
    Task<List<RoadmapTemplateDto>> GetTemplatesAsync(CancellationToken cancellationToken);

    Task<RoadmapDetailDto?> GetTemplateAsync(Guid roadmapId, CancellationToken cancellationToken);

    Task<StudentRoadmapDto> EnrollAsync(Guid accountId, Guid roadmapId, CancellationToken cancellationToken);

    Task<List<StudentRoadmapDto>> GetMyRoadmapsAsync(Guid accountId, CancellationToken cancellationToken);
}
