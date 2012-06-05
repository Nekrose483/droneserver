/*
 * TASKS: tasks should be sent from mstrclient to server to drnclient
 * when tasks are finished, they should be sent back as a pop up
 * "Username has finished task: Task"
 * 
 * requests should be sent from drnclient to mstrclient as a popup
 * "Username wishes to: some_task_here. will you allow this?" [yes, no]
 * 
 * TASKS: if section_leader then you can send requests to others with the same position, or drones.
 * Master can send anyone orders.
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
 * 		  New table, task_attempts:
 * 			[this table stores successful task completions and unsuccessful attempts.
 * 			  Certain tasks may not be completed on the first attempt. So this table
 * 			  can store each one ]
 * 			id							- long int (unique key)
 * 			taskid						- long int (refers to unique key of associated task)
 * 			attempt_date					- datetime
 * 			proof						- text (link to proof, e.g., image, text, forum post, etc)
 * 			outcome						- small int( 0 = pending, 1 = approved, 2 = rejected )
 * 			comments					- text (feedback from reviewer)
 * 			decision_date				- datetime
 * 
 * TABLE: positions
 * 
 * 1  Drone
 * 2  Targeted
 * 3  Seducer
 * 4  Hunter
 * 5  Muscle
 * 6  Caretaker
 * 7  Security
 * 8  Conditioner
 * 9  Engineer
 * 10  Beta
 * 11  Alpha
 * 12  Alpha's and Master's assistant
 * 0  temp account for testing
 * -1  Master
 * 
 * website todo: 
 * 
 * fix where it says "drone" to family member
 * assign everyone temporary IDs
 * remove rank
 * get new icons
 * get everyone's ages
 * 
 */