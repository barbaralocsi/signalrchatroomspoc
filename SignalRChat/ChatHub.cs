using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace SignalRChat
{
    public class ChatHub : Hub
    {

        public static HashSet<string> groupNames = new HashSet<string>();

        string systemMessageName = "System";

        public void Send(string roomName, string name, string message)
        {
            //string connectionID = Context.ConnectionId;
            //string username = Context.User.Identity.Name;
            //string userName = Clients.Caller.userName;
            // Call the broadcastMessage method to update clients.
            //Clients.All.broadcastMessage(name, message);
            Clients.Group(roomName).addChatMessage(name, message + " " + Context.ConnectionId);
        }

        public async Task JoinRoom(string roomName, string name)
        {
            if (!groupNames.Contains(roomName))
            {
                groupNames.Add(roomName);
                Clients.All.broadcastNewGroupCreated(groupNames.ToList());
            }
            //Context.ConnectionId
            await Groups.Add(Context.ConnectionId, roomName);
            Clients.Group(roomName).addChatMessage(systemMessageName, name + " added to group");
        }

        public Task LeaveRoom(string roomName, string name)
        {
            Clients.Group(roomName).addChatMessage(systemMessageName, name + " left the group");
            return Groups.Remove(Context.ConnectionId, roomName);
        }

        public List<string> getGroupList(){
            return groupNames.ToList();
        }


    }
}