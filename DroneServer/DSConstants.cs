using System;
namespace DroneServer
{
	public class DSConstants
	{
		//Network Constants
		public static int maxConnections = 100;
	    public static string acceptedNicknameCharicters =@"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-_\[]{}^";
        public static int maxCharictersInNickname = 10;
	
		//Database Constants
		//Hypnoflow Server
		//public static string DBConnectionString = "server=localhost;user=test;database=hypnoflow;port=3306;password=hypn0sh1t;";
		//Enzo's Machine
		public static string DBConnectionString = "server=localhost;user=root;database=test;port=3306;password=motion;";
		//Katja's Machine
		//public static string DBConnectionString = "server=localhost;user=root;database=hypnoflow;port=3306;password=tsunami;";
		
		public static string tblUser = "drones";  //I don't know if we really want to go here but i will anyway
		
		public DSConstants ()
		{
			

		}
	}
}
