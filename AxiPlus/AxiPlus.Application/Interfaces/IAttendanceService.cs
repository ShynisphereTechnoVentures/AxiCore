using AxiPlus.Application.DTOs.Attendance;

namespace AxiPlus.Application.Interfaces;

public interface IAttendanceService
{        
    Task<StudentAttendanceSummaryDto?> GetForStudentAsync(string email);
}
