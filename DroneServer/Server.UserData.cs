
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections;

namespace DroneServer {
	public class UserData {
		//This class will be used to store data particular for each user
		// username, connection, password, and encryptionkey stuffs
		// and anything else we need
		public string username;
		public string password;
		public string key;
		public int unit;
		public int number;
		public int position_id;
		public int rank;

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
}