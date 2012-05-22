using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections;

namespace DroneServer {
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
}
