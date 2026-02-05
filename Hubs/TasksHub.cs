using Microsoft.AspNetCore.SignalR;

public class TasksHub : Hub
{
  public System.Threading.Tasks.Task BroadcastTasksUpdate()
  {
    return Clients.All.SendAsync("TasksUpdated");
  }
}
