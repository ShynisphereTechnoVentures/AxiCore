using AxiPlus.Application.DTOs.Lessons;

namespace AxiPlus.Application.Interfaces;

public interface ILessonService
{
    Task<List<LessonDto>> GetByModuleAsync(int moduleId);

    Task<LessonDto?> GetByIdAsync(Guid id);

    Task<LessonDto> CreateAsync(CreateLessonDto dto);

    Task<LessonDetailsDto> GetLessonDetailsAsync(Guid studentUserId,Guid lessonId);

    Task StartLessonAsync(Guid studentUserId,Guid lessonId);

    Task CompleteLessonAsync(Guid studentUserId,Guid lessonId);

}