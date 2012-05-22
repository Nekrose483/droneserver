using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections;


namespace DroneServer
{
	public static class AdminTools
    {
        //you can disconnect a user with this.
        public static void disconnectUser (string Nick, string reason)
		{
			string nickToKill = Nick;
			foreach (var c in Lists.getConnectionByNick) {
				if (c.Key.ToLower () == Nick.ToLower ()) {
					nickToKill = c.Key;
				}
			}
			if (Lists.getConnectionByNick.ContainsKey (nickToKill)) {
				ChatServer.SendAdminMessage (reason);
				Lists.getConnectionByNick [nickToKill].CloseConnection ();
				Console.WriteLine ("Disconnecting user [" +(string) Nick + "]");
            }
        }
        //mute users with this
        public static void muteUser(string nick, string adminNick)
        {
            string nIck = nick;
            foreach (var c in Lists.getConnectionByNick)
            {
                if (c.Key.ToLower() == nick.ToLower())
                {
                    nIck = c.Key;
                }
            }
            if (Lists.mutedUsers.ContainsKey(nIck))
            {
                ChatServer.SendAdminMessage("MSG:SERVER: " + nIck + " has been muted by "+adminNick);
                Lists.mutedUsers[nIck] = true;
            }
            else
            {
                if (Server.htUsers.ContainsKey(nIck))
                {
                    ChatServer.SendAdminMessage("MSG:SERVER: "+nIck + " has been muted by " + adminNick);
                    Lists.mutedUsers.Add(nIck, true);
                }
            }
        }
        //unmute users with this
        public static void unMuteUser(string nick, string adminNick)
        {
            string nIck = nick;
            foreach (var c in Lists.getConnectionByNick)
            {
                if (c.Key.ToLower() == nick.ToLower())
                {
                    nIck = c.Key;
                }
            }
            if (Lists.mutedUsers.ContainsKey(nIck))
            {
                ChatServer.SendAdminMessage("MSG:SERVER: "+nIck + " has been unmuted by  " + adminNick);
                Lists.mutedUsers.Remove(nIck);
            }
            else
            {
                Lists.getConnectionByNick[adminNick].sendMessageToUser("MSG:SERVER: ---The Nickname "+nick+" Isnt Muted");
            }
        }
        //just another version of SendAdminMessage
        public static void sendNotice(string message)
        {
            ChatServer.SendAdminMessage(message);
        }
        //send a notice to only one user.
        public static void sendPrivateNotice(string nickToNotice, string message)
        {
            if (Lists.getConnectionByNick.ContainsKey(nickToNotice))
                Lists.getConnectionByNick[nickToNotice].sendMessageToUser("---<private notice> " + message);
        }
        //mimic any nickname on the network (or a non existant one)
        public static void mimicUser(string nickToMimic, string message)
        {
			//broken for now
			/*
            string[] args = { };
            ChatServer.OnCommand(Lists.MessageType.Message, "Administrator", "<" + nickToMimic + "> " + message, args);
            */
        }
        //msg all online admins
        public static void msgAllOnlineAdmins(string message)
        {
            foreach (var a in Lists.OnlineAdmins)
            {
                a.Value.sendMessageToUser("---Notice To Admins: " + message);
            }
        }
        //temparaily add an admin
        public static void addTempAdmin(string username, string password)
        {
            Lists.Admins.Add(username, password);
        }
        //temaraily delete an admins permissions.
        public static void tempDelAdmin(string username)
        {
            string adminNick = "";
            string adminUser = username;
            foreach (var a in Lists.OnlineAdmins)
            {
                if (a.Value.currUserAdmin == adminUser)
                {
                    adminNick = a.Value.currUser;
                }
            }
            Lists.Admins.Remove(adminUser);
            Lists.OnlineAdmins.Remove(adminNick);
        }
    }	
}