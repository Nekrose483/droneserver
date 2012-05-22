using System;
namespace DroneServer
{
	public class DSConstants
	{
		public static int maxConnections = 100;
        	
		//all the accepted charicters that can be in a nickname
	    public static string acceptedNicknameCharicters =@"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-_\[]{}^";
        	
		//self explainitory
        public static int maxCharictersInNickname = 10;
	
		public DSConstants ()
		{
			

		}
	}
}

