
using TaskManager.Application.DTOs.Tasks;

namespace TaskManager.Application.Interfaces.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskResponseDto>> GetAllAsync(Guid userId, bool? isCompleted = null);
        Task<TaskResponseDto> GetByIdAsync(Guid id, Guid userId);
        Task<TaskResponseDto> CreateAsync(CreateTaskDto dto, Guid userId);
        Task UpdateAsync(Guid id, UpdateTaskDto dto, Guid userId);
        Task DeleteAsync(Guid id, Guid userId);
    }
}