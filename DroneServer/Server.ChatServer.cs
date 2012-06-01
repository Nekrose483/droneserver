using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections;
using System.Text.RegularExpressions; //regular expressions, duh
using System.Web; //needs to be added as a reference
using System.Xml.XPath;
//using System.Xml.XmlReader;
using System.Xml.Serialization;

namespace DroneServer
{
	//This file contains just the classes and functions specific to chat
	class ChatServer
    {
		Server server; //Parent server
		
        public ChatServer (Server server_)
		{
			// do we need anything here ?
			server = server_;
		}
		
        //wtf does this do?
		/*
        public static void OnStatusChanged(StatusChangedEventArgs e)
        {
            StatusChangedEventHandler statusHandler = StatusChanged;
            if (statusHandler != null)
            {
                statusHandler(null, e);
            }
        }
		*/
        public static void SendAdminMessage (string Message)
		{
			StreamWriter swSenderSender;
			/*
			 * wtf?
			e = new StatusChangedEventArgs ("--- " + Message);
			OnStatusChanged (e);
			*/
			TcpClient[] tcpClients = new TcpClient[Server.htUsers.Count];
			Server.htUsers.Values.CopyTo (tcpClients, 0);
			for (int i = 0; i < tcpClients.Length; i++) {
				try {
					if (Message.Trim () == "" || tcpClients [i] == null) {
						continue;
					}
					swSenderSender = new StreamWriter (tcpClients [i].GetStream ());
					//swSenderSender.WriteLine("--- " + Message);
					swSenderSender.WriteLine (Message);
                    swSenderSender.Flush();
                    swSenderSender = null;
                }
                catch
                {
                    Server.RemoveUser(tcpClients[i]);
                }
            }
        }
		
        public static string formatXMLChatMessage (UserData sender, string override_sendername, string receivername, string channel, string message)
		{
			//this function only constructs the XML chat message
			//from server to clients
			
			string xmlstr = "";
			
			//I actually don't know how to get the name of the root node
			//so I'll just name it . to keep it small
			// here's what this should look like
			// <.>
			//		<type>chat</type>
			//		<admin>1</admin>   				(if applicable)
			//		<from>sender_name</from>
			//		<to>recipient_name</to>
			//		<channel>channel_name</channel>
			//		<message>message_text</message>
			// </.>
			
			XMLNode rootnode = new XMLNode (null, "x", ""); 
				
			rootnode.childNodes.Add (new XMLNode (rootnode, "type", "chat"));
			
			if (sender != null && sender.admin) 
				rootnode.childNodes.Add (new XMLNode (rootnode, "admin", "1"));
			if (override_sendername != "") 
				rootnode.childNodes.Add (new XMLNode (rootnode, "from", override_sendername));
			else if (sender != null)
				rootnode.childNodes.Add (new XMLNode (rootnode, "from", sender.username));
			else
				rootnode.childNodes.Add (new XMLNode(rootnode, "from","unknown"));
			
			rootnode.childNodes.Add (new XMLNode (rootnode, "to", receivername));
			rootnode.childNodes.Add (new XMLNode (rootnode, "channel", channel));
			rootnode.childNodes.Add (new XMLNode (rootnode, "message", message));
				
			
			xmlstr = rootnode.makeXMLString ();
			//rootnode.parseXML (xmlstr);	
			
			return xmlstr;
		}
		
		public static void interpretChatXML (UserData fromuser, XPathNavigator nav)
		{
			Console.WriteLine ("Interpret chat XML");
			
			string recepient_username = "";
			string channel_name = "";
			string message = "";
			
			nav.MoveToRoot ();			
			nav.MoveToFirstChild ();
			
			do {
				if (nav.NodeType == XPathNodeType.Element) {
					if (nav.HasChildren == true) {
						nav.MoveToFirstChild ();
						do {
							
							if (nav.Name == "from") {
								recepient_username = nav.Value;
							} else if (nav.Name == "channel") {
								channel_name = nav.Value;
							} else if (nav.Name == "message") {
								message = nav.Value;
							}
							
						} while (nav.MoveToNext()); 
					} else {
						/*
						Console.Write ("The XML string for this PARENT ");
						Console.Write ("  " + nav.Name + " ");
						Console.WriteLine ("is '{0}'", nav.Value);
						*/
					}
				}
			} while (nav.MoveToNext());
			
			Console.WriteLine ("Interpret chat XML - done");
			
			//Hopefully, we don't get stuck in an infinate loop
			SendChatMessage (fromuser, message);
		}
		
        public static void SendChatMessage (UserData FromUser, string Message)
		{
			if (FromUser.muted) {

				FromUser.connection.sendMessageToUser (
					formatXMLChatMessage(null,"NOTICE","","","You are muted, so you cannot talk.")
					);
				
				/* Wtf?
				e = new StatusChangedEventArgs ("MSG:NOTICE: Muted user> " + Message);
				OnStatusChanged (e);
				*/	
				
				//so, muted users can still spam admins?
				//AdminTools.msgAllOnlineAdmins ("Muted user> " + Message);
			} else {
				if (Message.Trim () == "")
					return;
				
				//Escape Message (chevrons will break xml) first
				//because they'll break our xml
				
				string safe_message = HttpUtility.HtmlEncode (Message.Trim ());  //Regex.Escape (Message.Trim ());
				string xml_message = formatXMLChatMessage(FromUser,"","0","0",safe_message);
				
				StreamWriter swSenderSender;
				/*
				 * wtf?
				 * e = new StatusChangedEventArgs (Message);
				OnStatusChanged (e);
				*/
				TcpClient[] tcpClients = new TcpClient[Server.htUsers.Count];
				Server.htUsers.Values.CopyTo (tcpClients, 0);
				for (int i = 0; i < tcpClients.Length; i++) {
					try {
						if (tcpClients [i] == null) {
							continue;
						}
						swSenderSender = new StreamWriter (tcpClients [i].GetStream ());
						
						swSenderSender.WriteLine (xml_message);
						swSenderSender.Flush ();
						swSenderSender = null;
					} catch {
						//wtf? Why doesn't this work?
						Server.RemoveUser (tcpClients [i]);
						
					}
				}
            }
        }
		
        
    }
}
