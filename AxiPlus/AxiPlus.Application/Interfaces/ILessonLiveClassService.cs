using AxiPlus.Application.DTOs.LessonLiveClasses;

namespace AxiPlus.Application.Interfaces;

public interface ILessonLiveClassService
{        
    Task<List<LessonLiveClassDto>>
        GetByLessonAsync(Guid lessonId);
}