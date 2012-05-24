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
		public static string DBConnectionString = "server=localhost;user=root;database=test;port=3306;password=motion;";
		public static string tblUser = "user";  //I don't know if we really want to go here
		
		public DSConstants ()
		{
			

		}
	}
}

