using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace DroneServer
{
	class MainClass
	{
		private delegate void UpdateStatusCallback(string strMessage);
		public static void Main (string[] args)
		{
			IPAddress ipAddr = IPAddress.Parse ("127.0.0.1");
			Server mainServer = new Server (ipAddr);
			Console.WriteLine ("Drone Control System: ONLINE");
			mainServer.StartListening ();
			
			while (1 == 1) { }
		}
	}
}
