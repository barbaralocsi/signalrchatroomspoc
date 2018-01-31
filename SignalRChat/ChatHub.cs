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
        public static OnlineUserHandler userHandler = new OnlineUserHandler();

        //string systemMessageName = "system";

        public void Send(string roomName, string name, string message)
        {
            //string connectionID = Context.ConnectionId;
            //string username = Context.User.Identity.Name;
            //string userName = Clients.Caller.userName;
            // Call the broadcastMessage method to update clients.
            //Clients.All.broadcastMessage(name, message);
            //Clients.Group(roomName).addChatMessage(name, message + " " + Context.ConnectionId);
            Clients.Group(roomName).addChatMessage(name, message);
        }

        public async Task JoinRoom(string roomName, string userName)
        {
            if (!groupNames.Contains(roomName))
            {
                groupNames.Add(roomName);
                Clients.All.broadcastNewGroupCreated(groupNames.ToList());
            }
            userHandler.AddUserToRoom(Context.ConnectionId, roomName, userName);

            await Groups.Add(Context.ConnectionId, roomName);
            Clients.Group(roomName).onlineListChanged(roomName);
            Clients.Group(roomName).addChatMessage(roomName + " bot ", userName + " joined group: " + roomName);
        }

        public Task LeaveRoom(string roomName, string name)
        {
            userHandler.RemoveUserFromRoom(Context.ConnectionId, roomName);
            Clients.Group(roomName).onlineListChanged(roomName);
            Clients.Group(roomName).addChatMessage(roomName + " bot ", name + " left the group: " + roomName);
            return Groups.Remove(Context.ConnectionId, roomName);
        }

        public List<string> getGroupList()
        {
            return groupNames.ToList();
        }


        public List<User> getOnlineList(string groupName)
        {
            return userHandler.getOnlineList(groupName);
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Clients.All.addChatMessage("Server", userHandler.getUserName(Context.ConnectionId) + " disconnected from the server");
            List<string > rooms = userHandler.getRoomsOfUser(Context.ConnectionId);
            foreach (var item in rooms) {
                Clients.Group(item).onlineListChanged(item);
            }

            userHandler.RemoveUser(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }
    }
}