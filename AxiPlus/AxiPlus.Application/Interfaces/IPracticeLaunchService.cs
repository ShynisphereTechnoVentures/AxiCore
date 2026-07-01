using AxiPlus.Application.DTOs.Practice;

namespace AxiPlus.Application.Interfaces;

public interface IPracticeLaunchService
{
    /// <summary>
    /// Creates a signed AxiForge practice launch URL for an entitled AxiPlus student.
    /// Returns the redirect URL and launch payload so the student can continue from a lesson into matching coding practice.
    /// </summary>
    Task<PracticeLaunchResponseDto> CreateLessonPracticeLaunchAsync(Guid studentUserId, Guid lessonId, CancellationToken cancellationToken = default);
}
