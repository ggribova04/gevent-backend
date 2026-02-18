using Microsoft.AspNetCore.SignalR;

public class TendersHub : Hub
{
  public System.Threading.Tasks.Task BroadcastTendersUpdate()
  {
    return Clients.All.SendAsync("TendersUpdated");
  }
}
