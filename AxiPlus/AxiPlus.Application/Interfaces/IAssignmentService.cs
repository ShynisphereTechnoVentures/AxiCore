using AxiPlus.Application.DTOs.Assignments;

namespace AxiPlus.Application.Interfaces;

public interface IAssignmentService
{       
    Task<List<StudentAssignmentDto>> GetForStudentAsync(string email);

    Task<StudentAssignmentDto?> SubmitAsync(
        string email,
        Guid assignmentId,
        SubmitAssignmentDto dto);
}
