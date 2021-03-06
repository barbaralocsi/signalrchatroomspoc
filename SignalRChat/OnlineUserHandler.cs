﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRChat
{
    public class OnlineUserHandler
    {
        private Dictionary<string, List<User>> onlineUsersOfGroups = new Dictionary<string, List<User>>();
        private HashSet<User> allUsers = new HashSet<User>();

        public void RemoveUserFromRoom(string connectionId, string roomName) {

            List<User> userList;
            if (!onlineUsersOfGroups.TryGetValue(roomName, out userList))
            {
                userList = new List<User>();
            }

            var itemToRemove = allUsers.FirstOrDefault(r => r.ConnectionID == connectionId);
            userList.Remove(itemToRemove);

        }

        public void AddUserToRoom(string connectionId, string roomName, string userName)
        {
            User user = allUsers.FirstOrDefault(u => u.ConnectionID == connectionId);
            if (user == null) {
                user = new User(userName, connectionId);
                allUsers.Add(user);
            }

            List<User> onlines;
            if (!onlineUsersOfGroups.TryGetValue(roomName, out onlines))
            {
                onlines = new List<User>();
            }
            onlines.Add(user);
            onlineUsersOfGroups.Remove(roomName);
            onlineUsersOfGroups.Add(roomName, onlines);
        }

        public List<User> getOnlineList(string groupName)
        {
            List<User> onlines;
            if (onlineUsersOfGroups.TryGetValue(groupName, out onlines))
            {
                return onlines;
            }
            return null;
        }

        public void RemoveUser(string connectionId)
        {
            allUsers.RemoveWhere(u => u.ConnectionID == connectionId);
            foreach (var item in onlineUsersOfGroups)
            {
                item.Value.RemoveAll(x => x.ConnectionID == connectionId);
            }
        }

        public string getUserName(string connectionId) {
            return allUsers.FirstOrDefault(x => x.ConnectionID == connectionId).Name;
        }

        public List<string> getRoomsOfUser(string connectionId) {
            List<string> rooms = new List<string>();
            foreach (var item in onlineUsersOfGroups)
            {
                if (item.Value.Any(x => x.ConnectionID == connectionId)) {
                    rooms.Add(item.Key);
                }
            }
            return rooms;
        }
    }
}