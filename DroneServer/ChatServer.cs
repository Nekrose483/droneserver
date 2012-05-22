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
        
        public static void SendChatMessage (UserData FromUser, string Message)
		{
			if (FromUser.muted) {

				FromUser.connection.sendMessageToUser ("MSG:NOTICE:You are muted, so you cannot talk.");
				
				/* Wtf?
				e = new StatusChangedEventArgs ("MSG:NOTICE: Muted user> " + Message);
				OnStatusChanged (e);
				*/	
				
				//so, muted users can still spam admins?
				//AdminTools.msgAllOnlineAdmins ("Muted user> " + Message);
			} else {
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
						if (Message.Trim () == "" || tcpClients [i] == null) {
							continue;
						}
						swSenderSender = new StreamWriter (tcpClients [i].GetStream ());
						
						swSenderSender.WriteLine (Message);
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
