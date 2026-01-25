public interface ITaskService
{
    Task<List<TaskDto>> GetTasksByEventAsync(int eventId);
    Task<bool> CreateTaskAsync(string login, TaskDto dto);
    Task<UpdateTaskStatusDto> UpdateTaskStatusAsync(UpdateTaskStatusDto dto);
    Task<List<TaskDto>> GetAllUserTasksAsync(int userId, int userRoleId);
}