using System;
using System.Collections.Generic;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace DroneServer
{
	public class MysqlDB
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
					//Console.WriteLine (rdr ["username"] + " --- " + rdr ["admin"]);
					
					bool admin = false;
					bool muted = false;
					int unit = -1;
					int number = -1;
					int position_id = -1;
					int rank = -1;

					if ((int)rdr ["admin"] == 1)
						admin = true;
					if ((int)rdr ["muted"] == 1)
						muted = true;
					if ((int)rdr["unit"] != null)
						unit = (int)rdr["unit"];
					if ((int)rdr["number"] != null)
						number = (int)rdr["number"];
					if ((int)rdr["position_id"] != null)
						position_id = (int)rdr["position_id"];
					if ((int)rdr["rank"] != null)
						rank = (int)rdr["rank"];

					retUser = new UserData ((string)rdr ["username"], password, "", null, null, this, admin,muted);
					
					rdr.Close ();
					return retUser;
				}
				rdr.Close ();
			} catch (Exception ex) {
				//Console.WriteLine (ex.ToString ());
				return null;
			}
			return null;
		}

		public UserData getUserData (string username)
		{
			UserData retUser = null;
			
			try {
				string sql = "SELECT * FROM " + DSConstants.tblUser + " WHERE username=@Username";
				MySqlCommand cmd = new MySqlCommand (sql, conn);

				cmd.Parameters.AddWithValue ("@Username", username);
			
				MySqlDataReader rdr = cmd.ExecuteReader ();
				
				
				while (rdr.Read()) {
			
					
					bool admin = false;
					bool muted = false;
					int unit = -1;
					int number = -1;
					int position_id = -1;
					int rank = -1;

					if ((int)rdr ["admin"] == 1)
						admin = true;
					if ((int)rdr ["muted"] == 1)
						muted = true;
					if ((int)rdr["unit"] != null)
						unit = (int)rdr["unit"];
					if ((int)rdr["number"] != null)
						number = (int)rdr["number"];
					if ((int)rdr["position_id"] != null)
						position_id = (int)rdr["position_id"];
					if ((int)rdr["rank"] != null)
						rank = (int)rdr["rank"];

					retUser = new UserData ((string)rdr ["username"], null, "", null, null, this,admin,muted);
					
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
		public string getUserList (UserData commander) //get user list from db to send to drone client
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
		
		public List<TaskData> getUserTasks (string username)
		{
			//Overload function, expects username, finds a matching UserData obj
			// and calls getUserTasks(userdata), returning the results
			UserData user = getUserData (username);
			List<TaskData> tasks = getUserTasks (user);
			return tasks;
		}

		public List<TaskData> getUserTasks (UserData requester)
		{
			string ret = "";
			List<TaskData> tasks = new List<TaskData>();
			TaskData task;
		
			try {
				string sql = "SELECT * FROM " + DSConstants.tblTasks + " WHERE to_unit = " + requester.unit + " AND to_number = " + requester.number + ";";
				
				MySqlCommand cmd = new MySqlCommand (sql, conn);
				MySqlDataReader rdr = cmd.ExecuteReader ();

				while (rdr.Read()) {
					task = new TaskData ();

					task.id = (int)rdr["taskid"];
					task.from_unit = (int)rdr ["from_unit"];
					task.from_number = (int)rdr ["from_number"];
					task.to_unit = (int)rdr ["to_unit"];
					task.to_number = (int)rdr ["to_number"];
					task.taskcontent = (string)rdr ["task"];
					task.creation_date = System.DateTime.Parse ((string)rdr ["create_date"]);
					task.end_date = System.DateTime.Parse ((string)rdr ["end_date"]);
					task.tasktype = (int)rdr ["type"];
					
					if ((int)rdr ["requires_review"] == 0) {
						task.requires_review = false;
					} else {
						task.requires_review = true;
					}
					
					if ((int)rdr ["completed"] == 0) {
						task.completed = false;
					} else {
						task.completed = true;
					}
					
					if ((int)rdr ["denied"] == 0) {
						task.denied = false;
					} else {
						task.denied = true;
					}
					
					if ((int)rdr ["failed"] == 0) {
						task.failed = false;
					} else {
						task.failed = true;
					}
					
					tasks.Add (task);
				}
				rdr.Close ();
			} catch (Exception ex) {
				Console.WriteLine ("{0} Exception.", ex);
				 
			}
			return tasks; 
		}
		
		
		public void close ()
		{
			conn.Close ();
			Console.WriteLine ("Done.");
		}
	}
}

