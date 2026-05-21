namespace TaskManager.Domain.Exceptions
{
    public class TaskNotFoundException : DomainException
    {
        public TaskNotFoundException()
            : base("Task not found.") { }
    }
}