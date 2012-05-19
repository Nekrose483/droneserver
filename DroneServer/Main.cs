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
			//how do you keep it up and running?
			IPAddress ipAddr = IPAddress.Parse ("127.0.0.1");
			ChatServer mainServer = new ChatServer (ipAddr);
			//   ChatServer.StatusChanged += new StatusChangedEventHandler(mainServer_StatusChanged);
			Console.WriteLine ("Drone Control System: ONLINE");
			mainServer.StartListening (); //This starts a thread to listen
			
			while (1 == 1) { } //Main loop
		}
		public void mainServer_StatusChanged(object sender, StatusChangedEventArgs e)
        {
           // this.Invoke(new UpdateStatusCallback(this.UpdateStatus), new object[] { e.EventMessage });
        }
        private void UpdateStatus(string strMessage)
        {
           // txtLog.AppendText(strMessage + "\r\n");
        }
	}
}
