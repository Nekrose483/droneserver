/*
 * 
 * 
 * 
 * 
 * 
 * create a login system. right now it just uses the username.
 * admin system should be if admin = 1 in the sql db
 * 
 * comm with SQL db...
 * 
 * how do you call sha1 hash?
 * 
 * compare hashed passwd to hash in db to auth user
 * 
 * 
 * 
 * TASKS: tasks should be sent from mstrclient to server to drnclient
 * when tasks are finished, they should be sent back as a pop up
 * "Username has finished task: Task"
 * 
 * requests should be sent from drnclient to mstrclient as a popup
 * "Username wishes to: some_task_here. will you allow this?" [yes, no]
 * 
 * 
 * when sending requests, they should be asked to ask either master or mistress
 * that should be presented in a pop up window when they click "ask"
 * 
 *  Enzo: Ok.  It seems like we should also build in a way to obtain the current
 *  	  tasks assigned to each user.  Talking to Charles, he wants to distinguish
 * 		  between two kinds of tasks: requests and orders.  Anyone can make
 * 		  requests of anyone else.  Orders can only be made from section leader
 * 		  to section leader, or from section leader to drones of the same section.
 * 		  There is also a special condition in which drones in a section and order
 * 		  the leader of their section given a vote (so, we'll have to have a system
 * 		  for proposing votes for things).  According to Charles, orders do not
 * 		  need to be obeyed, but if they are not, they need to be explicitly denied.
 * 		  Given that, the db might be modified in the following way:
 * 		  
 * 		  Additions to user table:
 * 			section leader - boolean (or small int)
 * 
 * 		  New table, tasks:
 * 			[this table stores tasks (requests or orders) ]
 * 			 id							- long int (unique key)
 * 			 sender		   				- long int? (what ever the key value is in the user table)
 * 			 recipient     				- [same as above]
 * 			 task          				- text
 *           creation_date 				- datetime
 * 											This way we could calculate time outstanding, to find negligent drones
 * 			 type		   				- int (0 = order, 1 = request)
 * 			 requires_review 			- boolean (or small int)
 * 											For some tasks, it may be sufficient for the done to simply
 * 											say they have been completed.  Others may require proof.  This
 * 											variable indicates which kind of task this is.
 * 			 completed     				- boolean (or small int)
 *           denied		   				- boolean (or small int)
 * 			 failed						- boolean (or small int)
 * 											If the recipient simply gives up
 * 			 end_date	   				- datetime of completion or denial
 * 
 * 		  New table, taskAttempts:
 * 			[this table stores successful task completions and unsuccessful attempts.
 * 			  Certain tasks may not be completed on the first attempt. So this table
 * 			  can store each one ]
 * 			id							- long int (unique key)
 * 			taskid						- long int (refers to unique key of associated task)
 * 			attemptdate					- datetime
 * 			proof						- text (link to proof, e.g., image, text, forum post, etc)
 * 			outcome						- small int( 0 = pending, 1 = approved, 2 = rejected )
 * 			comments					- text (feedback from reviewer)
 * 			decisiondate				- datetime
 * 
 */