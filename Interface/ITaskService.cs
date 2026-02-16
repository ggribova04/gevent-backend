public interface ITaskService
{
  Task<List<TaskDto>> GetStep2TasksAsync(int eventId);
  Task<List<TaskDto>> GetStep3TasksAsync(int eventId);
  Task<List<TaskDto>> GetTasksByEventAsync(int eventId);
  Task<bool> CreateTaskAsync(string login, int roleId, TaskDto dto);
  Task<UpdateTaskStatusDto> UpdateTaskStatusAsync(UpdateTaskStatusDto dto);
  Task<List<TaskDto>> GetAllUserTasksAsync(int userId, int userRoleId);
  Task<bool> DeleteTaskAsync(int taskId);
  Task<List<User>> SearchPerformersAsync(PerformerSearchDto dto);
  System.Threading.Tasks.Task CreateTaskFromTenderAsync(Tender tender, TenderResponse winner);
}
