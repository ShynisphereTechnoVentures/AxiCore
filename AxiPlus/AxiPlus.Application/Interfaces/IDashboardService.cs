using AxiPlus.Application.DTOs.Dashboard;

namespace AxiPlus.Application.Interfaces;

public interface IDashboardService
{       
    Task<StudentDashboardDto> GetStudentDashboardAsync(Guid studentId);
}