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
	
	public class UserData {
		//This class will be used to store data particular for each user
		// username, connection, password, and encryptionkey stuffs
		// and anything else we need
		public string username;
		public string password;
		public string key;
		public Connection connection;
		public TcpClient tcpUser;
		public bool admin;
		public bool muted;
		
		
		public UserData (string username_,
		                 string password_,
		                 string key_,
		                 TcpClient tcpUser_,
		                 Connection connection_,
		                 bool admin_,
		                 bool muted_)
		{
			username = username_;
			password = password_;
			key = key_;
			tcpUser = tcpUser_;
			connection = connection_;
			admin = admin_;
			muted = muted_;
		}
	}
	
	class Server
	{
		//I hate these structures, they should all be replaced by something more robust
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
			//deprecated
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
             
				string adminflag = "";
					
				if (user.admin)
					adminflag = " (M)";
						
				ChatServer.SendChatMessage (user, "MSG:" + user.username + adminflag + ":" + message);
				
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
	
    
    public class Connection
    {
        public TcpClient tcpClient;
        private Thread thrSender;
        private StreamReader srReceiver;
        private StreamWriter swSender;
        public string currUser;
        public string currUserAdmin;
		public string currPassword;
		public string currKey;         			//for future encryption options
		public UserData user;					//use this throughout, so we can deprecate the above stuff
        private bool hasConnectedYet = false;
        private string strResponse;
		
        public Connection(TcpClient tcpCon)
        {
            tcpClient = tcpCon;
            thrSender = new Thread(AcceptClient);
            thrSender.Start();
        }
        public void CloseConnection()
        {
            if (hasConnectedYet)
            {
                if (Lists.OnlineAdmins.ContainsKey(currUser))
                {
                    Lists.OnlineAdmins.Remove(currUser);
                    ChatServer.SendAdminMessage("MSG:SERVER: Room Admin " + currUser + " Left and Logged Out");
                }
                Server.RemoveUser(tcpClient);
            }
            tcpClient.Close();
            srReceiver.Close();
            swSender.Close();
            thrSender.Abort();
        }
        private bool isValidNickname(string nick)
        {
            string AcceptedCharicters = DSConstants.acceptedNicknameCharicters;
            bool isValid = true;
            char[] array = nick.ToCharArray();
            foreach (char c in array)
            {
                if (AcceptedCharicters.Contains(c.ToString())) { }
                else
                {
                    isValid = false;
                }
            }
            return isValid;
        }
        public void sendMessageToUser(string rawMessage)
        {
            swSender.WriteLine(rawMessage);
            swSender.Flush();
        }
        private void AcceptClient ()
		{
			string loginstr;
			srReceiver = new System.IO.StreamReader (tcpClient.GetStream ());
			swSender = new System.IO.StreamWriter (tcpClient.GetStream ());
			loginstr = srReceiver.ReadLine ();
			List<string> splitArray = new List<string> (loginstr.Split (new char[] { ':' }));
			
			if (splitArray.Count != 3) {
				//Login credentials should look like this <username>:<password>:<key>\n
				//These error messages should be removed during production for security
				//reasons
				swSender.WriteLine ("0|Login credentials should take the following form:");
				swSender.WriteLine ("0|<username>:<password>:<key>");
				swSender.Flush ();
				CloseConnection ();
				return;
			}
			
			currUser = splitArray [0];
			currPassword = splitArray [1];
			currKey = splitArray [2];
			
			//These values are arbitrary right now, we can fix them latr
			if (currUser.Length <= 1 || currUser.Length >= 15) {
				swSender.WriteLine ("0|Invalid username. [" + currUser + "]");
				swSender.Flush ();
				CloseConnection ();
				return;
			} else if (currPassword.Length <= 1 || currPassword.Length >= 15) {
				swSender.WriteLine ("0|Invalid password.");
				swSender.Flush ();
				CloseConnection ();
				return;
			} else if (currKey.Length <= 1 || currKey.Length >= 30) {
				swSender.WriteLine ("0|Invalid key.");
				swSender.Flush ();
				CloseConnection ();
				return;
			}
			
			splitArray.RemoveAt (0); //I think we can ditch this
			//currUser = srReceiver.ReadLine();
			
			if (currUser != "") {
				if (Server.htUsers.Contains (currUser) == true) {
					swSender.WriteLine ("0|This nickname is in use.");
					swSender.Flush ();
					CloseConnection ();
					return;
				} /*else if (currUser.ToLower () == "administrator" || currUser.ToLower () == "server" || currUser.ToLower () == "console" || currUser.ToLower () == "owner" || currUser.ToLower () == "admin") {
					swSender.WriteLine ("0|This nickname is unavailable.");
					swSender.Flush ();
					CloseConnection ();
					return;
				} */ else if (isValidNickname (currUser) == false) {
					swSender.WriteLine ("0|This nickname contains invalid characters.");
					swSender.Flush ();
					CloseConnection ();
					return;
				} else if (currUser.Length > DSConstants.maxCharictersInNickname) {
					swSender.WriteLine ("0|This nickname has too many characters.");
					swSender.Flush ();
					CloseConnection ();
					return;
				} else {
					//This seems just as good a place as any to compare login credentials
					//to those stored in the data base
					
					UserData loginAttempt = Server.sqldb.authenticateUser (currUser, currPassword);
					if (loginAttempt == null) {
						swSender.WriteLine ("0|Invalid username or password.");
						swSender.Flush ();
						CloseConnection ();
						return;
					}
					
					/*  Deprecated admin login check
					bool isAdmin = false;
					if (Lists.Admins.ContainsKey (currUser)) {
						if (Lists.Admins [currUser] == currPassword)
							isAdmin = true;
						else {
							swSender.WriteLine ("0|Invalid username or password.");
							swSender.Flush ();
							CloseConnection ();
							return;
						}
					}
					*/
					loginAttempt.tcpUser = tcpClient;
					loginAttempt.connection = this;
					user = loginAttempt;
					
					hasConnectedYet = true;
					Server.AddUser (loginAttempt);
                    swSender.WriteLine("1");
                    swSender.Flush();
                    swSender.WriteLine("Welcome To The Network. Make Sure You Don't Spam");
                    swSender.WriteLine("--- You Have Successfully Connected ---");
                    swSender.Flush();
                }
            }
            else
            {
                CloseConnection();
                return;
            }

            try
            {
                while ((strResponse = srReceiver.ReadLine()) != "")
                {
                    if (strResponse == null)
                    {
                        CloseConnection();
                    }
                    else
                    {
                        ProcessMessage(strResponse);
                    }
                }
            }
            catch
            {
                CloseConnection();
            }
        }
        private void ProcessMessage(string rawMessage)
        {
            List<string> splitArray = new List<string>(rawMessage.Split(new char[] { ':' }));
            string command = splitArray[0];
            splitArray.RemoveAt(0);
            string message = String.Join(":", splitArray.ToArray());
            splitArray = new List<string>(command.Split(new char[] { ' ' }));
            command = splitArray[0];
            splitArray.RemoveAt(0);
            string[] commandArgs = splitArray.ToArray();

            if (command == "MSG")
            {
                Server.OnCommand(Lists.MessageType.Message, user, message, commandArgs);
            }
            else if (command == "ACTION")
            {
                Server.OnCommand(Lists.MessageType.Action, user, message, commandArgs);
            }
            else if (command == "ADMIN")
            {
                Server.OnCommand(Lists.MessageType.AdminAction, user, message, commandArgs);
            }
            else if (command == "NOTICE")
            {
                Server.OnCommand(Lists.MessageType.Notice, user, message, commandArgs);
            }
        }
    }
    #endregion
	
}

