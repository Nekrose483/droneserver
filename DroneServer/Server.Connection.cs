using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections;
using System.Xml;
using System.Xml.XPath;
//using System.Xml.XmlReader;
using System.Xml.Serialization;
using System.Xml.Linq;


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
        private void ProcessMessage (string rawMessage)
		{
			
			
			byte[] byteArray = Encoding.UTF8.GetBytes (rawMessage);
			MemoryStream stream = new MemoryStream (byteArray);
			
			
			XPathDocument xmldoc;
			
			try {
				xmldoc = new XPathDocument (stream);
			} catch (Exception ex) {
				Console.WriteLine ("{0} Exception in xml: ",ex);
				return;
			}
			
			XPathNavigator nav = xmldoc.CreateNavigator ();
			
			nav.MoveToRoot ();
			
		
			nav.MoveToFirstChild ();
			
			
			
			
			do {
				//this code works, but it seems to only comb through 1 dept level
				//Find the first element.
				
				if (nav.NodeType == XPathNodeType.Element) {
					//if children exist
					if (nav.HasChildren == true) {

						//Move to the first child.
						nav.MoveToFirstChild ();

						//Loop through all the children.
						do {
							//loop through the xml and look for type flag
							
							if (nav.Name == "type") {
								//we found the type flag, now figure out
								//where to send this message
								
								if (nav.Value == "chat") {
									//send to chat server
									//Note, I'm bypassing Server.OnCommand
									//why do we need it?
									ChatServer.interpretChatXML (user,nav);
									return;
								}
							}
							
						} while (nav.MoveToNext()); 
					} else {
						
						//Console.Write ("The XML string for this PARENT ");
						//Console.Write ("  " + nav.Name + " ");
						//Console.WriteLine ("is '{0}'", nav.Value);
						
					}
				}
			} while (nav.MoveToNext());
			
			return;
			/*
			//this stuff is obsolete since moving to xml
			List<string> splitArray = new List<string> (rawMessage.Split (new char[] { ':' }));
			string command = splitArray [0];
			splitArray.RemoveAt (0);
			string message = String.Join (":", splitArray.ToArray ());
			splitArray = new List<string> (command.Split (new char[] { ' ' }));
			command = splitArray [0];
			splitArray.RemoveAt (0);
			string[] commandArgs = splitArray.ToArray ();

			if (command == "MSG") {
				Server.OnCommand (Lists.MessageType.Message, user, message, commandArgs);
			} else if (command == "ACTION") {
				Server.OnCommand (Lists.MessageType.Action, user, message, commandArgs);
			} else if (command == "ADMIN") {
				Server.OnCommand (Lists.MessageType.AdminAction, user, message, commandArgs);
			} else if (command == "NOTICE") {
				Server.OnCommand (Lists.MessageType.Notice, user, message, commandArgs);
			} else if (command == "XMLTEST") {
				
				//this is just a test of the xml generator
				string xmlstr = "";
				string jsonstr = "";
				XMLNode rootnode = new XMLNode (null, "clientmsg", "");
				
				rootnode.childNodes.Add (new XMLNode (rootnode, "type", "chat"));
				rootnode.childNodes.Add (new XMLNode (rootnode, "room", "0"));
				rootnode.childNodes.Add (new XMLNode (rootnode, "to", "0"));
				rootnode.childNodes.Add (new XMLNode (rootnode, "message", "wacka flacka"));

				
				
				jsonstr = rootnode.makeJSONString ();
				xmlstr = rootnode.makeXMLString ();
				rootnode.parseXML (xmlstr);
				
				
				ChatServer.SendChatMessage (user, "MSG:* " + user.username + " " + xmlstr);
				ChatServer.SendChatMessage (user, "MSG:* " + user.username + " " + jsonstr);
			}
			*/
        }
    }
}
