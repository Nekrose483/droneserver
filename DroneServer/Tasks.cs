using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections;
using System.Xml.XPath;
//using System.Xml.XmlReader;
using System.Xml.Serialization;

namespace DroneServer
{
	public class Tasks
	{
		MysqlDB db;
		
		public Tasks (MysqlDB db_)
		{
			db = db_;
		}

		//Here's what we need
		// 1. a data structure to hold a task
		// 2. getUserTasks(User) - returns a list of Tasks for User
		// 3. getCreatorTasks(User) -- returns a list of Tasks created by User
		// 3. formatXMLTask(task) - returns a string representation of a task 
		// 4. interpretTaskXML -- interprets requests for user and creator tasks, new tasks, and task progress updates
		// 4. a function to interpret new tasks from XML (and write to DB)
		// 5. a function to interpret task progress from XML (and update the DB)
		
		public string getUserTasks (UserData requester, XPathNavigator nav) //
		{
			List<TaskData> tasks;
			String ret = "";

			tasks = db.getUserTasks (requester);

			foreach (TaskData task in tasks) {
				ret += task.formatXMLUserTask();
			}

			return ret;
		}


		
		public static void SendTask (UserData FromUser, string Message)
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

