using AxiForge.Application.DTOs.Dashboard;

namespace AxiForge.Application.Interfaces;

public interface IStudentDashboardService
{
    Task<StudentDashboardDto> GetDashboardAsync(Guid accountId, CancellationToken cancellationToken);
}
