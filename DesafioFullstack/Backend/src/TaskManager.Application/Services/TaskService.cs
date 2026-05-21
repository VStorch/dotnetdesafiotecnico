using AutoMapper;
using TaskManager.Application.DTOs.Tasks;
using TaskManager.Application.Interfaces.Services;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;
using TaskManager.Domain.Repositories;

namespace TaskManager.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public TaskService(ITaskRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TaskResponseDto>> GetAllAsync(Guid userId, bool? isCompleted = null)
        {
            var tasks = await _taskRepository.GetByUserIdAsync(userId, isCompleted);
            return _mapper.Map<IEnumerable<TaskResponseDto>>(tasks);
        }

        public async Task<TaskResponseDto> GetByIdAsync(Guid id, Guid userId)
        {
            var task = await _taskRepository.GetByIdAndUserIdAsync(id, userId);
            if (task == null) throw new TaskNotFoundException();

            return _mapper.Map<TaskResponseDto>(task);
        }

        public async Task<TaskResponseDto> CreateAsync(CreateTaskDto dto, Guid userId)
        {
            var task = new TaskItem(dto.Title, dto.DueDate, userId)
            {
                Description = dto.Description
            };

            await _taskRepository.AddAsync(task);
            await _taskRepository.SaveChangesAsync();

            return _mapper.Map<TaskResponseDto>(task);
        }

        public async Task UpdateAsync(Guid id, UpdateTaskDto dto, Guid userId)
        {
            var task = await _taskRepository.GetByIdAndUserIdAsync(id, userId);
            if (task == null) throw new TaskNotFoundException();

            _mapper.Map(dto, task);
            _taskRepository.Update(task);
            await _taskRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id, Guid userId)
        {
            var task = await _taskRepository.GetByIdAndUserIdAsync(id, userId);
            if (task == null) throw new TaskNotFoundException();

            _taskRepository.Delete(task);
            await _taskRepository.SaveChangesAsync();
        }
    }
}