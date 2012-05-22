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
 * 
 * 
 * 
 * EXAMPLE SERVER:
 * 
 *
  class SimpleServer{
  Server server;
  ClientInfo client;
  void Start(){
    server = new Server(2345, new ClientEvent(ClientConnect));
  }
  
  bool ClientConnect(Server serv, ClientInfo new_client){
    new_client.Delimiter = '\n';
    new_client.OnRead += new ConnectionRead(ReadData);
    return true; // allow this connection
  }

  void ReadData(ClientInfo ci, String text){
    Console.WriteLine("Received from "+ci.ID+": "+text);
    if(text[0] == '!')
     server.Broadcast(Encoding.UTF8.GetBytes(text));
    else ci.Send(text);
  }
}
 * 00:16 <auroriumoxide> its ok
00:16 <auroriumoxide> i can change them
00:16 <auroriumoxide> in understand that code ^^
00:17 <auroriumoxide> oh, and you can remove the mainServer_StatusChanged()
00:17 <auroriumoxide> and UpdateStatus()
00:17 <auroriumoxide> in Main.cs
00:17 <auroriumoxide> those are unneeded
00:18 <auroriumoxide> ok. in Drone server.cs
00:18 <auroriumoxide> there are a few classes
00:18 <auroriumoxide> Options						[moved to DSConstants]
00:18 <auroriumoxide> AdminTools					[done]
00:19 <auroriumoxide> StatusChangedEventArgs
00:19 <auroriumoxide> Lists
00:19 <auroriumoxide> UserData
00:19 <auroriumoxide> Server
00:20 <auroriumoxide> Connection
00:20 <auroriumoxide> and thats it
00:20 <auroriumoxide> i want those all in separate files
00:20 <auroriumoxide> just to clean up stuff
00:21 <auroriumoxide> i'm a bit dislexic.. so it's much easier for me to keep track of things when they are all sorted
00:21 <auroriumoxide> i'm sorry to push my disability on you.. but it will also make life much easier when we come to update the program with new code
00:21 <auroriumoxide> making the program "modular" is a very nice thing
00:21 <enzo> heh naw, it's no problem
00:22 <auroriumoxide> plus i think it's just a good skill to have over all
00:22 <enzo> i'll do it, though I may not be able to do another push until tomorrow
00:22 <auroriumoxide> keeping code clean and modular
00:22 <auroriumoxide> it's perfectly fine
00:22 <auroriumoxide> ^^
00:22 <enzo> Server and Connection seemed pretty related
00:22 <auroriumoxide> you've done a very good job
00:22 <enzo> but, I"ll separate them if you like
00:23 <auroriumoxide> yes, separate them. later, if we both feel the need, they can always be combined
00:23 <enzo> true that
00:23 <auroriumoxide> and see if you can't start putting variables in the DSConstants.cs file
00:24 <auroriumoxide> it will make life easier to get in that habbit early in the game
00:24 <enzo> right

 * 
 * 
 * 
 * 
 * 
 */