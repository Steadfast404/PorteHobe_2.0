using Portehobe.src.PorteHobe.API.DTOs;
using PorteHobe.API.DTOs;
namespace PorteHobe.API.Services;

public interface ISubjectService
{
    Task<SubjectDto> CreateSubjectAsync(CreateSubjectDto dto, string userId);
    Task<SubjectDto?> GetSubjectByIdAsync(int id, string userId);
    // Implementing pagination as requested in the issue
    Task<IEnumerable<SubjectDto>> GetAllSubjectsAsync(int pageNumber, int pageSize, string userId);
    Task<SubjectDto?> UpdateSubjectAsync(int id, UpdateSubjectDto dto, string userId);
    Task<bool> DeleteSubjectAsync(int id, string userId);
}
