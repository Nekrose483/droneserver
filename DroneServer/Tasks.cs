using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections;
using System.Xml.XPath;
//using System.Xml.XmlReader;
using System.Xml.Serialization;

namespace DroneServer
{
	public class Tasks
	{
		MysqlDB db;
		
		public Tasks (MysqlDB db_)
		{
			db = db_;
		}

		//Here's what we need
		// 1. a data structure to hold a task
		// 2. getUserTasks(User) - returns a list of Tasks for User
		// 3. getCreatorTasks(User) -- returns a list of Tasks created by User
		// 3. formatXMLTask(task) - returns a string representation of a task 
		// 4. interpretTaskXML -- interprets requests for user and creator tasks, new tasks, and task progress updates
		// 4. a function to interpret new tasks from XML (and write to DB)
		// 5. a function to interpret task progress from XML (and update the DB)
		
		public string getUserTasks (UserData requester, XPathNavigator nav) //
		{
			List<TaskData> tasks;
			String ret = "";

			tasks = db.getUserTasks (requester);

			foreach (TaskData task in tasks) {
				ret += task.formatXMLUserTask();
			}

			return ret;
		}


		

	}
	

}

