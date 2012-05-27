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
		public static string DBConnectionString = "server=hypnoflow.org;user=test;database=hypnoflow;port=3306;password=hypn0sh1t;";
		public static string tblUser = "drones";  //I don't know if we really want to go here
		
		public DSConstants ()
		{
			

		}
	}
}

