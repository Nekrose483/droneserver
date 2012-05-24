using System;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace DroneServer
{
	class MysqlDB
	{
		public MySqlConnection conn;
		
		public MysqlDB ()
		{
			conn = new MySqlConnection (DSConstants.DBConnectionString);
			try {
				conn.Open ();
			} catch (Exception ex) {
				Console.WriteLine ("Exception connecting to database: " + ex.ToString ());
			}
		}
		
		public void query ()
		{
			try {
				string sql = "SELECT * FROM " + DSConstants.tblUser;
				
				MySqlCommand cmd = new MySqlCommand (sql, conn);

				//Console.WriteLine ("Enter a continent e.g. 'North America', 'Europe': ");
				//string user_input = "North America";

				//cmd.Parameters.AddWithValue ("@Continent", user_input);

				MySqlDataReader rdr = cmd.ExecuteReader ();

				while (rdr.Read()) {
					Console.WriteLine (rdr ["username"] + " --- " + rdr ["admin"] );
				}
				rdr.Close ();
			} catch (Exception ex) {
				Console.WriteLine (ex.ToString ());
			}
		}
		
		public UserData authenticateUser (string username, string password)
		{
			UserData retUser = null;
			
			try {
				string sql = "SELECT * FROM " + DSConstants.tblUser + " WHERE username=@Username AND hashed_password=sha1(@Password)";
				MySqlCommand cmd = new MySqlCommand (sql, conn);

				cmd.Parameters.AddWithValue ("@Username", username);
				cmd.Parameters.AddWithValue ("@Password", password);

				MySqlDataReader rdr = cmd.ExecuteReader ();
				
				
				while (rdr.Read()) {
					Console.WriteLine (rdr ["username"] + " --- " + rdr ["admin"]);
					
					bool admin = false;
					bool muted = false;
					
					if ((int)rdr ["admin"] == 1)
						admin = true;
					if ((int)rdr ["muted"] == 1)
						muted = true; //naughty
					
					retUser = new UserData ((string)rdr ["username"], password, "", null, null, admin,muted);
					
					rdr.Close ();
					return retUser;
				}
				rdr.Close ();
			} catch (Exception ex) {
				Console.WriteLine (ex.ToString ());
				return null;
			}
			return null;
		}
		
		//some Prototype DB functions
		//public bool setMuteUser(UserData commander, string targetUserName, bool muteStatus) {}
		//public string getUserInfo(UserData commander, string targetUserName) {}
		public string getUserList (UserData commander)
		{
			string ret = "";
			
			try {
				string sql = "SELECT * FROM " + DSConstants.tblUser;
				
				MySqlCommand cmd = new MySqlCommand (sql, conn);
				MySqlDataReader rdr = cmd.ExecuteReader ();

				while (rdr.Read()) {
					string adminStr = "";
					if ((int)rdr ["admin"] == 1)
						adminStr = "(M)";
					
					//I'm not really sure how to format this
					//It occurs to me that it might be easier to format data in xml
					//assuming mono has functions for parsing xml
					ret += "MSG:SERVER: " + (string)rdr ["username"] + adminStr + "\n";
				}
				rdr.Close ();
			} catch (Exception ex) {
				ret += "MSG:SERVER: " + ex.ToString ();
			}
			return ret;
		}
		//public bool modifyUserProfile(UserData commander, string targetUserName, string column, string value) {}
		
		
		public void close ()
		{
			conn.Close ();
			Console.WriteLine ("Done.");
		}
	}
}

