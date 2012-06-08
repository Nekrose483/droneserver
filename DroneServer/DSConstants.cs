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
		public static string DBConnectionString = "server=208.115.203.104;user=test;database=hypnoflow;port=3306;password=hypn0sh1t;";
		public static string tblUser = "drones";
		public static string tblTasks = "tasks";
	}
}
