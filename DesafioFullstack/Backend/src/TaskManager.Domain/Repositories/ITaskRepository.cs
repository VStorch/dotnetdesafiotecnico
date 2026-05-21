using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Repositories
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskItem>> GetByUserIdAsync(Guid userId, bool? isCompleted = null);
        Task<TaskItem?> GetByIdAndUserIdAsync(Guid id, Guid userId);

        Task AddAsync(TaskItem task);
        void Update(TaskItem task);
        void Delete(TaskItem task);
        Task<int> SaveChangesAsync();
    }
}