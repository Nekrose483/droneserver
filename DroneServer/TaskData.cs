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

