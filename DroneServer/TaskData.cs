
using System;

namespace DroneServer
{
	public class TaskData
	{
		public int id;
		public int from_unit;
		public int from_number;
		public int to_unit;
		public int to_number;
		public string taskcontent;
		public DateTime creation_date; 
		public int tasktype;
		public bool requires_review;
		public bool completed;
		public bool denied;
		public bool failed;
		public DateTime end_date;
		
				/* 
		 * DateTime value = new DateTime(2010, 1, 18);
	Console.WriteLine(value);
	Console.WriteLine(value == DateTime.Today);
		 Console.WriteLine(yesturday == DateTime.Today.AddDays(-1)); */
		
		//change server task date to datetime
				
		public TaskData (int id_,
		                 int from_unit_,
 						 int from_number_,
						 int to_unit_,
						 int to_number_,
						 string task_,
        				 DateTime creation_date_,
						 int type_,
						 bool requires_review_,
						 bool completed_,
        				 bool denied_,
						 bool failed_,
						 DateTime end_date_)
		{
			id = id_;
			from_unit = from_unit_;
			from_number = from_number_;
			to_unit = to_unit_;
			to_number = to_number_;
			taskcontent = task_;
			creation_date = creation_date_;
			tasktype = type_;
			requires_review = requires_review_;
			completed = completed_;
			denied = denied_;
			failed = failed_;
			end_date = end_date_;
		}
		
		public TaskData() {
		}
		
		public string formatXMLUserTask ()
		{
			//make this a method of TaskData
			string xmlstr = "";
			
			
			// <x>
			//		<type>usertask</type>
			//		<from_unit>fromunit</from_unit>
			//		<from_number>fromnumber</from_number>
			//		<to_unit>tounit</to_unit>
			//		<to_number>tonumber</to_number>
			//		<title>task_title</title>
			//		<body>task_body</body>
			// </x>
			
			
			XMLNode rootnode = new XMLNode (null, "x", ""); 

			//NOTE: the usertask designation is used to communicate tasks to the user who
			// is expected to do them.
			// Alternatively, the creatortask designation is used to communcate tasks
			// and task progress to other users (e.g., the creator, master, etc)
			rootnode.childNodes.Add (new XMLNode (rootnode, "type", "usertask"));
			
			rootnode.childNodes.Add (new XMLNode (rootnode, "id", id.ToString ()));
			rootnode.childNodes.Add (new XMLNode (rootnode, "from_unit", from_unit.ToString()));
			rootnode.childNodes.Add (new XMLNode (rootnode, "from_number", from_number.ToString())); //fix this in UserData

			rootnode.childNodes.Add (new XMLNode (rootnode, "to_unit", to_unit.ToString()));
			rootnode.childNodes.Add (new XMLNode (rootnode, "to_number", to_number.ToString()));

			
			rootnode.childNodes.Add (new XMLNode (rootnode, "to_unit", to_unit.ToString()));
			rootnode.childNodes.Add (new XMLNode (rootnode, "task_type", tasktype.ToString()));
			
			rootnode.childNodes.Add (new XMLNode (rootnode, "task_content", taskcontent));
			
			rootnode.childNodes.Add (new XMLNode (rootnode, "creation_date", creation_date.ToString()));
			rootnode.childNodes.Add (new XMLNode (rootnode, "task_type", tasktype.ToString ()));
			xmlstr = rootnode.makeXMLString ();
			
			return xmlstr;
		}
		
		public string formatXMLCreatorTask ()
		{
			string xmlstr = "";
			
			XMLNode rootnode = new XMLNode (null, "x", ""); 

			//NOTE: the usertask designation is used to communicate tasks to the user who
			// is expected to do them.
			// Alternatively, the creatortask designation is used to communcate tasks
			// and task progress to other users (e.g., the creator, master, etc)
			rootnode.childNodes.Add (new XMLNode (rootnode, "type", "creatortask"));
			
			rootnode.childNodes.Add (new XMLNode (rootnode, "id", id.ToString ()));
			rootnode.childNodes.Add (new XMLNode (rootnode, "from_unit", from_unit.ToString ()));
			rootnode.childNodes.Add (new XMLNode (rootnode, "from_number", from_number.ToString ())); //fix this in UserData

			rootnode.childNodes.Add (new XMLNode (rootnode, "to_unit", to_unit.ToString ()));
			rootnode.childNodes.Add (new XMLNode (rootnode, "to_number", to_number.ToString ()));

			
			rootnode.childNodes.Add (new XMLNode (rootnode, "to_unit", to_unit.ToString ()));
			rootnode.childNodes.Add (new XMLNode (rootnode, "task_type", tasktype.ToString ()));
			
			rootnode.childNodes.Add (new XMLNode (rootnode, "task_content", taskcontent));
			
			rootnode.childNodes.Add (new XMLNode (rootnode, "creation_date", creation_date.ToString ()));
			rootnode.childNodes.Add (new XMLNode (rootnode, "task_type", tasktype.ToString ()));
			xmlstr = rootnode.makeXMLString ();
			
			return xmlstr;
		}
	}
}

=======
using System;
namespace DroneServer
{
	public class TaskData
	{
		public int from_unit;
		public int from_number;
		public int to_unit;
		public int to_number;
		public string task;
		public DateTime creation_date; 
		public int type;
		public bool requires_review;
		public bool completed;
		public bool denied;
		public bool failed;
		public DateTime end_date;
		
				/* 
		 * DateTime value = new DateTime(2010, 1, 18);
	Console.WriteLine(value);
	Console.WriteLine(value == DateTime.Today);
		 Console.WriteLine(yesturday == DateTime.Today.AddDays(-1)); */
		
				
		public TaskData (int from_unit_,
 						 int from_number_,
						 int to_unit_,
						 int to_number_,
						 string task_,
        				 DateTime creation_date_,
						 int type_,
						 bool requires_review_,
						 bool completed_,
        				 bool denied_,
						 bool failed_,
						 DateTime end_date_)
		{
			from_unit = from_unit_;
			from_number = from_number_;
			to_unit = to_unit_;
			to_number = to_number_;
			task = task_;
			creation_date = creation_date_;
			type = type_;
			requires_review = requires_review_;
			completed = completed_;
			denied = denied_;
			failed = failed_;
			end_date = end_date_;
		}
	}
}


