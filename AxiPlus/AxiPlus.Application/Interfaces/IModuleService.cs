using AxiPlus.Application.DTOs.Modules;

namespace AxiPlus.Application.Interfaces;

public interface IModuleService
{     
    Task<List<ModuleDto>> GetAllAsync();

    Task<ModuleDto?> GetByIdAsync(int id);

    Task<ModuleDto> CreateAsync(CreateModuleDto dto);

    Task<bool> DeleteAsync(int id);

    Task<List<StudentModuleDto>> GetStudentModulesAsync(Guid studentUserId);

    Task<ModuleDetailsDto> GetModuleDetailsAsync(Guid studentUserId,int moduleId);
}