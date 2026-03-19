using Portehobe.src.PorteHobe.API.DTOs;
using PorteHobe.API.DTOs;
namespace PorteHobe.API.Services;

public interface ISubjectService
{
    Task<SubjectDto> CreateSubjectAsync(CreateSubjectDto dto);
    Task<SubjectDto?> GetSubjectByIdAsync(int id);
    // Implementing pagination as requested in the issue
    Task<IEnumerable<SubjectDto>> GetAllSubjectsAsync(int pageNumber = 1, int pageSize = 10);
    Task<SubjectDto?> UpdateSubjectAsync(int id, UpdateSubjectDto dto);
    Task<bool> DeleteSubjectAsync(int id);
}
