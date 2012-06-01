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

    
   
    
	
	
	class Server
	{
		//I hate these structures, they should both be replaced by something more robust
		public static Hashtable htUsers = new Hashtable(DSConstants.maxConnections);
        public static Hashtable htConnections = new Hashtable(DSConstants.maxConnections);
		
        private IPAddress ipAddress;
        private TcpClient tcpClient;
        //public static event StatusChangedEventHandler StatusChanged;
        //private static StatusChangedEventArgs e;
		public static MysqlDB sqldb;
		public static ChatServer chatserver;
		
        public Server (IPAddress address)
		{
			ipAddress = address;
			sqldb = new MysqlDB ();
			sqldb.query ();
			ChatServer chatserver = new ChatServer (this);
			//sqldb.close (); <-- we should only do this before we shut down, but i'm not sure how to capture that
        }
        private Thread thrListener;
        private TcpListener tlsClient;
        bool ServRunning = false;
		
		public static void AddUser (TcpClient tcpUser, string strUsername, string pass, string key, Connection connect, bool admin)
		{
			//this function deprecated and have been replaced by AddUser(UserData newuser)
			ChatServer.SendAdminMessage ((string)"MSG:SERVER: "+strUsername + " connected.");
			htUsers.Add (strUsername, tcpUser);
			htConnections.Add (tcpUser, strUsername);
			//Lists.addConnection (strUsername, pass, key, connect, admin);
			if (admin) Lists.OnlineAdmins.Add (strUsername, connect); //Add to admin list
			Console.WriteLine ("Connecting user ["+ (string)strUsername+ "]");
        }
		
		public static void AddUser (UserData newuser)
		{
			ChatServer.SendAdminMessage ("MSG:SERVER: "+newuser.username + " connected.");
			Lists.addConnectedUser (newuser);
			
			//For how much longer do we need these 3 lines?
			htUsers.Add (newuser.username, newuser.tcpUser);
			htConnections.Add (newuser.tcpUser, newuser.username);
			if (newuser.admin)
				Lists.OnlineAdmins.Add (newuser.username, newuser.connection); //Add to admin list
			
			Console.WriteLine ("Connecting user [" + (string)newuser.username + "]");
		}
        public static void RemoveUser (TcpClient tcpUser)
		{
			if (htConnections [tcpUser] != null) {
				string nick = (string)htConnections [tcpUser];
				htUsers.Remove (htConnections [tcpUser]);
				htConnections.Remove (tcpUser);
				foreach (var c in Lists.getConnectionByNick) {
					if (c.Value.tcpClient == tcpUser) {
						Lists.removeConnection (c.Value);
						break;
					}
				}
				ChatServer.SendAdminMessage ("MSG:SERVER: " + nick + " disconnected.");
				Console.WriteLine ("Removing user [" + (string)nick + "]");
			}
			
        }
		public static void OnCommand (Lists.MessageType messageType, UserData user, string message, string[] args)
		{
			if (messageType == Lists.MessageType.Message) { //move this to sendmessage
            	
				ChatServer.SendChatMessage (user, message);
				
			} else if (messageType == Lists.MessageType.Action) {
				
				string adminflag = "";
					
				if (user.admin)
					adminflag = " (M)";
						
				ChatServer.SendChatMessage (user, "MSG:* " + user.username + adminflag + " " + message);
				
			} else if (messageType == Lists.MessageType.AdminAction) {
				
				if (user.admin)
					AdminAction (user.username, message, args);
				else {
					user.connection.sendMessageToUser ("MSG:SERVER: Command not available. You are not an admin.");
					AdminTools.msgAllOnlineAdmins ("MSG:SERVER: Failed Admin Command by " + user.username + " using user '" + args [0] + "'.");
				}
				
			} else if (messageType == Lists.MessageType.Notice) {
				string adminflag = "";
					
				if (user.admin)
					adminflag = " (M)";
				
                ChatServer.SendChatMessage(user, "* " + user.username + adminflag + ": " + message+" *");
            }
        }
        private static void AdminAction(string adminNick, string action, string[] args)
        {
            List<string> splitArray = new List<string>(action.Split(new char[] { ' ' }));
            string command = splitArray[0].ToUpper();
            splitArray.RemoveAt(0);
            if (command == "KILL")
            {
                string nick = splitArray[0];
                splitArray.RemoveAt(0);
                string reason = String.Join(" ",splitArray.ToArray());
                AdminTools.disconnectUser(nick, "User "+nick+" has been Disconnected by "+adminNick+" ("+reason+").");
            }
            else if (command == "MUTE")
            {
                string nick = splitArray[0];
                splitArray.RemoveAt(0);
                string reason = String.Join(" ", splitArray.ToArray());
                AdminTools.muteUser(nick, adminNick);
            }
            else if (command == "UNMUTE")
            {
                string nick = splitArray[0];
                splitArray.RemoveAt(0);
                string reason = String.Join(" ", splitArray.ToArray());
                AdminTools.unMuteUser(nick, adminNick);
            }
        }
        public void StartListening()
        {
            IPAddress ipaLocal = ipAddress;
            tlsClient = new TcpListener(1986);
            tlsClient.Start();
			/*
			 * Deprecated since transition to mysql db
            Lists.Admins.Add("Admin", "123987");
            Lists.Admins.Add("user2", "password"); //sql look up if admin = 1
            Lists.Admins.Add("user3", "password");
            */
            ServRunning = true;
            thrListener = new Thread(KeepListening);
            thrListener.Start();
        }
        private void KeepListening()
        {
            while (ServRunning == true)
            {
                tcpClient = tlsClient.AcceptTcpClient();
                Connection newConnection = new Connection(tcpClient);
            }
        }
	}
	
    
    
    //#endregion //wtf is this?
	
}

