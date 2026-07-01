using AxiPlus.Application.DTOs.Attendance;
using AxiPlus.Application.Interfaces;
using AxiPlus.Domain.Enums;
using AxiPlus.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AxiPlus.Infrastructure.Services;

public class AttendanceService : IAttendanceService
{       
    private readonly AppDbContext _context;

    public AttendanceService(AppDbContext context)
   {
        _context = context;
    }

    public async Task<StudentAttendanceSummaryDto?> GetForStudentAsync(
        string email)
   {
        var student = await _context.Students
            .FirstOrDefaultAsync(x => x.Email == email);

        if (student == null)
       {
            return null;
        }

        var records = await _context.Attendances
            .Where(x => x.StudentId == student.Id)
            .Join(
                _context.Sessions,
                attendance => attendance.SessionId,
                session => session.Id,
                (attendance, session) => new
               {        
                    attendance.Status,
                    Session = session
                })
            .OrderByDescending(x => x.Session.StartTime)
            .ToListAsync();

        var total = records.Count;
        var present = records.Count(x =>
            x.Status == AttendanceStatus.Present);
        var late = records.Count(x =>
            x.Status == AttendanceStatus.Late);
        var absent = records.Count(x =>
            x.Status == AttendanceStatus.Absent);

        var percentage = total == 0
            ? 0
            : Math.Round(((present + late) * 100m) / total, 2);

        return new StudentAttendanceSummaryDto
       {      
            TotalClasses = total,
            PresentCount = present,
            AbsentCount = absent,
            LateCount = late,
            AttendancePercentage = percentage,
            RecentClasses = records
                .Take(10)
                .Select(x => new AttendanceRecordDto
               {       
                    SessionId = x.Session.Id,
                    Title = x.Session.Title,
                    StartTime = x.Session.StartTime,
                    EndTime = x.Session.EndTime,
                    Status = x.Status.ToString()
                })
                .ToList()
        };
    }
}
