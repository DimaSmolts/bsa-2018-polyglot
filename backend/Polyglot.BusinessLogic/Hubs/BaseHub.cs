﻿using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Polyglot.BusinessLogic.Hubs
{
    public class BaseHub : Hub
    {
        public virtual async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public virtual async Task LeaveGroup(string groupName)
        {
            try
            {
                // ConnectionId может быть уже недоступен и по истечению time out
                // будет сгенерировано исключение
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            }
            catch (TaskCanceledException ex)
            {

            }
        }
    }
}
