using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections;

namespace DroneServer {
	
	public static class Lists
    {
        public static Dictionary<string, Connection> getConnectionByNick = new Dictionary<string, Connection>();
        public static Dictionary<Connection, string> getNickByConnection = new Dictionary<Connection, string>();
        public static Dictionary<string, bool> mutedUsers = new Dictionary<string, bool>();
		
        public static void addConnectionDeprecated(string nickname, Connection connect)
        {
            getConnectionByNick.Add(nickname, connect);
            getNickByConnection.Add(connect, nickname);
        }
		
		public static void addConnectedUser (UserData newuser)
		{
			UserList.Add (new UserData (newuser.username,
			                           newuser.password,
			                           newuser.key,
			                           newuser.tcpUser,
			                           newuser.connection,
			                           newuser.sqldb,
			                           newuser.admin,
			                           newuser.muted)
			);
			
			//The following lines may become obsolete in the future (the above line should take care of it)
			getConnectionByNick.Add (newuser.username, newuser.connection);
			getNickByConnection.Add (newuser.connection, newuser.username);
		}
		
        public static void removeConnection (Connection connect)
		{
			try {
				getConnectionByNick.Remove (getNickByConnection [connect]);
				getNickByConnection.Remove (connect);
				
				for (int i = 0; i < UserList.Count - 1; i++) {
					if (UserList [i].connection.GetHashCode() == connect.GetHashCode()) {
						UserList.Remove (UserList [i]);
					}
				}
            }
            catch { }
        }
        public enum MessageType { Action, Message, PrivateMessage, AdminAction, Notice }
        public static Dictionary<string, string> Admins = new Dictionary<string, string>();
        public static Dictionary<string, Connection> OnlineAdmins = new Dictionary<string, Connection>();
		public static List<UserData> UserList = new List<UserData> ();
    }
	
}