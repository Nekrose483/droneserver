using System;
namespace DroneServer
{
	public class Tasks
	{
		public Tasks ()
		{
		}
		
		public static void get_tasks (UserData requester) //
		{
			MysqlDB.getTasks();
			
		}
		
		public static string formatXMLTask (UserData sender, int to_unit, int to_number, string title, string body)
		{
			
			string xmlstr = "";
			

			// <x>
			//		<type>task</type>
			//		<from_unit>fromunit</from_unit>
			//		<from_number>fromnumber</from_number>
			//		<to_unit>tounit</to_unit>
			//		<to_number>tonumber</to_number>
			//		<title>task_title</title>
			//		<body>task_body</body>
			// </x>
			
			XMLNode rootnode = new XMLNode (null, "x", ""); 

			rootnode.childNodes.Add (new XMLNode (rootnode, "type", "task"));
			if (sender != null) {
				rootnode.childNodes.Add (new XMLNode (rootnode, "from_unit", sender.unit));
				rootnode.childNodes.Add (new XMLNode (rootnode, "from_number", sender.number)); //fix this in UserData
			}//unit and number
			else
				rootnode.childNodes.Add (new XMLNode(rootnode, "from","unknown"));
			
			rootnode.childNodes.Add (new XMLNode (rootnode, "to", receivername));
			rootnode.childNodes.Add (new XMLNode (rootnode, "channel", channel));
			rootnode.childNodes.Add (new XMLNode (rootnode, "message", message));
				
			
			xmlstr = rootnode.makeXMLString ();
			//rootnode.parseXML (xmlstr);	
			
			return xmlstr;
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

