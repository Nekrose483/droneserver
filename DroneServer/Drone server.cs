using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections;

namespace DroneServer
{
//AdminTools, and Server options. v
    public static class Options
    {
        //lets you set how many users can be connected at once.
        public static int maxConnections = 100;
        //all the accepted charicters that can be in a nickname
        public static string acceptedNicknameCharicters =@"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-_\[]{}^";
        //self explainitory
        public static int maxCharictersInNickname = 10;

    }
    public static class AdminTools
    {
        //you can disconnect a user with this.
        public static void disconnectUser(string Nick, string reason)
        {
            string nickToKill = Nick;
            foreach (var c in Lists.getConnectionByNick)
            {
                if (c.Key.ToLower() == Nick.ToLower())
                {
                    nickToKill = c.Key;
                }
            }
            if (Lists.getConnectionByNick.ContainsKey(nickToKill))
            {
                ChatServer.SendAdminMessage(reason);
                Lists.getConnectionByNick[nickToKill].CloseConnection();
            }
        }
        //mute users with this
        public static void muteUser(string nick, string adminNick)
        {
            string nIck = nick;
            foreach (var c in Lists.getConnectionByNick)
            {
                if (c.Key.ToLower() == nick.ToLower())
                {
                    nIck = c.Key;
                }
            }
            if (Lists.mutedUsers.ContainsKey(nIck))
            {
                ChatServer.SendAdminMessage(nIck + " has been muted by "+adminNick);
                Lists.mutedUsers[nIck] = true;
            }
            else
            {
                if (ChatServer.htUsers.ContainsKey(nIck))
                {
                    ChatServer.SendAdminMessage(nIck + " has been muted by " + adminNick);
                    Lists.mutedUsers.Add(nIck, true);
                }
            }
        }
        //unmute users with this
        public static void unMuteUser(string nick, string adminNick)
        {
            string nIck = nick;
            foreach (var c in Lists.getConnectionByNick)
            {
                if (c.Key.ToLower() == nick.ToLower())
                {
                    nIck = c.Key;
                }
            }
            if (Lists.mutedUsers.ContainsKey(nIck))
            {
                ChatServer.SendAdminMessage(nIck + " has been unmuted by  " + adminNick);
                Lists.mutedUsers.Remove(nIck);
            }
            else
            {
                Lists.getConnectionByNick[adminNick].sendMessageToUser("---The Nickname Isnt Muted");
            }
        }
        //just another version of SendAdminMessage
        public static void sendNotice(string message)
        {
            ChatServer.SendAdminMessage(message);
        }
        //send a notice to only one user.
        public static void sendPrivateNotice(string nickToNotice, string message)
        {
            if (Lists.getConnectionByNick.ContainsKey(nickToNotice))
                Lists.getConnectionByNick[nickToNotice].sendMessageToUser("---<private notice> " + message);
        }
        //mimic any nickname on the network (or a non existant one)
        public static void mimicUser(string nickToMimic, string message)
        {
            string[] args = { };
            ChatServer.OnCommand(Lists.MessageType.Message, "Administrator", "<" + nickToMimic + "> " + message, args);
        }
        //msg all online admins
        public static void msgAllOnlineAdmins(string message)
        {
            foreach (var a in Lists.OnlineAdmins)
            {
                a.Value.sendMessageToUser("---Notice To Admins: " + message);
            }
        }
        //temparaily add an admin
        public static void addTempAdmin(string username, string password)
        {
            Lists.Admins.Add(username, password);
        }
        //temaraily delete an admins permissions.
        public static void tempDelAdmin(string username)
        {
            string adminNick = "";
            string adminUser = username;
            foreach (var a in Lists.OnlineAdmins)
            {
                if (a.Value.currUserAdmin == adminUser)
                {
                    adminNick = a.Value.currUser;
                }
            }
            Lists.Admins.Remove(adminUser);
            Lists.OnlineAdmins.Remove(adminNick);
        }
    }
    //AdminTools, and Server options. ^
	
	   #region ConnectionStuff
    public class StatusChangedEventArgs : EventArgs
    {
        private string EventMsg;
        public string EventMessage
        {
            get
            {
                return EventMsg;
            }
            set
            {
                EventMsg = value;
            }
        }
        public StatusChangedEventArgs(string strEventMsg)
        {
            EventMsg = strEventMsg;
        }
    }
    public delegate void StatusChangedEventHandler(object sender, StatusChangedEventArgs e);
    public static class Lists
    {
        public static Dictionary<string, Connection> getConnectionByNick = new Dictionary<string, Connection>();
        public static Dictionary<Connection, string> getNickByConnection = new Dictionary<Connection, string>();
        public static Dictionary<string, bool> mutedUsers = new Dictionary<string, bool>();
        public static void addConnection(string nickname, Connection connect)
        {
            getConnectionByNick.Add(nickname, connect);
            getNickByConnection.Add(connect, nickname);
        }
        public static void removeConnection(Connection connect)
        {
            try
            {
                getConnectionByNick.Remove(getNickByConnection[connect]);
                getNickByConnection.Remove(connect);
            }
            catch { }
        }
        public enum MessageType { Action, Message, PrivateMessage, AdminAction, Notice }
        public static Dictionary<string, string> Admins = new Dictionary<string, string>();
        public static Dictionary<string, Connection> OnlineAdmins = new Dictionary<string, Connection>();
    }
    class ChatServer
    {
        public static Hashtable htUsers = new Hashtable(Options.maxConnections);
        public static Hashtable htConnections = new Hashtable(Options.maxConnections);
        private IPAddress ipAddress;
        private TcpClient tcpClient;
        public static event StatusChangedEventHandler StatusChanged;
        private static StatusChangedEventArgs e;
        public ChatServer(IPAddress address)
        {
            ipAddress = address;
        }
        private Thread thrListener;
        private TcpListener tlsClient;
        bool ServRunning = false;
        public static void AddUser(TcpClient tcpUser, string strUsername, Connection connect)
        {
            SendAdminMessage(strUsername + " connected.");
            ChatServer.htUsers.Add(strUsername, tcpUser);
            ChatServer.htConnections.Add(tcpUser, strUsername);
            Lists.addConnection(strUsername, connect);
        }
        public static void RemoveUser(TcpClient tcpUser)
        {
            if (htConnections[tcpUser] != null)
            {
                string nick = (string)htConnections[tcpUser];
                htUsers.Remove(htConnections[tcpUser]);
                htConnections.Remove(tcpUser);
                foreach (var c in Lists.getConnectionByNick)
                {
                    if (c.Value.tcpClient == tcpUser)
                    {
                        Lists.removeConnection(c.Value);
                        break;
                    }
                }
                SendAdminMessage(nick+" disconnected.");
            }
        }
        public static void OnStatusChanged(StatusChangedEventArgs e)
        {
            StatusChangedEventHandler statusHandler = StatusChanged;
            if (statusHandler != null)
            {
                statusHandler(null, e);
            }
        }
        public static void SendAdminMessage(string Message)
        {
            StreamWriter swSenderSender;
            e = new StatusChangedEventArgs("--- " + Message);
            OnStatusChanged(e);
            TcpClient[] tcpClients = new TcpClient[ChatServer.htUsers.Count];
            ChatServer.htUsers.Values.CopyTo(tcpClients, 0);
            for (int i = 0; i < tcpClients.Length; i++)
            {
                try
                {
                    if (Message.Trim() == "" || tcpClients[i] == null)
                    {
                        continue;
                    }
                    swSenderSender = new StreamWriter(tcpClients[i].GetStream());
                    swSenderSender.WriteLine("--- " + Message);
                    swSenderSender.Flush();
                    swSenderSender = null;
                }
                catch
                {
                    RemoveUser(tcpClients[i]);
                }
            }
        }
        private static bool isMuted(string nickname)
        {
            if (Lists.mutedUsers.ContainsKey(nickname))
            {
                if (Lists.mutedUsers[nickname] == true)
                {
                    if (htUsers.ContainsKey(nickname))
                    {
                        return true;
                    }
                    else
                    {
                        Lists.mutedUsers.Remove(nickname);
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        private static void SendMessage(string From, string Message)
        {
            if (isMuted(From))
            {
                if (Lists.getConnectionByNick.ContainsKey(From))
                {
                    Lists.getConnectionByNick[From].sendMessageToUser("You are muted, so you cannot talk.");
                    e = new StatusChangedEventArgs("Muted user> " + Message);
                    OnStatusChanged(e);
                    AdminTools.msgAllOnlineAdmins("Muted user> " + Message);
                }
            }
            else
            {
                StreamWriter swSenderSender;
                e = new StatusChangedEventArgs(Message);
                OnStatusChanged(e);
                TcpClient[] tcpClients = new TcpClient[ChatServer.htUsers.Count];
                ChatServer.htUsers.Values.CopyTo(tcpClients, 0);
                for (int i = 0; i < tcpClients.Length; i++)
                {
                    try
                    {
                        if (Message.Trim() == "" || tcpClients[i] == null)
                        {
                            continue;
                        }
                        swSenderSender = new StreamWriter(tcpClients[i].GetStream());
                        swSenderSender.WriteLine(Message);
                        swSenderSender.Flush();
                        swSenderSender = null;
                    }
                    catch
                    {
                        RemoveUser(tcpClients[i]);
                    }
                }
            }
        }
        public static void OnCommand(Lists.MessageType messageType, string nick, string message, string[] args)
        {
            if (messageType == Lists.MessageType.Message)
            {
                if (Lists.OnlineAdmins.ContainsKey(nick))
                    SendMessage(nick, "<" + nick + " (M)> " + message);
                else
                    SendMessage(nick, "<" + nick + "> " + message);
            }
            else if (messageType == Lists.MessageType.Action)
            {
                SendMessage(nick, "* " + nick + " " + message);
            }
            else if (messageType == Lists.MessageType.AdminAction)
            {
                if(Lists.Admins.ContainsKey(args[0]))
                {
                    if (Lists.Admins[args[0]] == args[1])
                    {
                        if (Lists.OnlineAdmins.ContainsKey(nick)){}
                        else
                        {
                            Lists.getConnectionByNick[nick].sendMessageToUser("---You have successfully logged in as "+args[0]);
                            SendAdminMessage("Room Admin " + args[0] + " now online (" + nick + ")");
                            Lists.OnlineAdmins.Add(nick, Lists.getConnectionByNick[nick]);
                            Lists.getConnectionByNick[nick].currUserAdmin = args[0];
                        }
                        AdminAction(nick, message, args);
                        return;
                    }
                }
                Lists.getConnectionByNick[nick].sendMessageToUser("---Login Incorrect");
                AdminTools.msgAllOnlineAdmins("Failed Administrative Login by " + nick + " using user '" + args[0] + "'.");
            }
            else if (messageType == Lists.MessageType.Notice)
            {
                SendMessage(nick, "* " + nick + ": " + message+" *");
            }
        }
        private static void AdminAction(string adminNick, string action, string[] args)
        {
            List<string> splitArray = new List<string>(action.Split(new char[] { ' ' }));
            string command = splitArray[0].ToUpper();
            splitArray.RemoveAt(0);
            if (command == "KILL")
            {
                string nick = splitArray[0];
                splitArray.RemoveAt(0);
                string reason = String.Join(" ",splitArray.ToArray());
                AdminTools.disconnectUser(nick, "User "+nick+" has been Disconnected by "+adminNick+" ("+reason+").");
            }
            else if (command == "MUTE")
            {
                string nick = splitArray[0];
                splitArray.RemoveAt(0);
                string reason = String.Join(" ", splitArray.ToArray());
                AdminTools.muteUser(nick, adminNick);
            }
            else if (command == "UNMUTE")
            {
                string nick = splitArray[0];
                splitArray.RemoveAt(0);
                string reason = String.Join(" ", splitArray.ToArray());
                AdminTools.unMuteUser(nick, adminNick);
            }
        }
        public void StartListening()
        {
            IPAddress ipaLocal = ipAddress;
            tlsClient = new TcpListener(1986);
            tlsClient.Start();
            Lists.Admins.Add("Admin", "123987");
            Lists.Admins.Add("user2", "password"); //sql look up if admin = 1
            Lists.Admins.Add("user3", "password");
            ServRunning = true;
            thrListener = new Thread(KeepListening);
            thrListener.Start();
        }
        private void KeepListening()
        {
            while (ServRunning == true)
            {
                tcpClient = tlsClient.AcceptTcpClient();
                Connection newConnection = new Connection(tcpClient);
            }
        }
    }
    public class Connection
    {
        public TcpClient tcpClient;
        private Thread thrSender;
        private StreamReader srReceiver;
        private StreamWriter swSender;
        public string currUser;
        public string currUserAdmin;
        private bool hasConnectedYet = false;
        private string strResponse;
        public Connection(TcpClient tcpCon)
        {
            tcpClient = tcpCon;
            thrSender = new Thread(AcceptClient);
            thrSender.Start();
        }
        public void CloseConnection()
        {
            if (hasConnectedYet)
            {
                if (Lists.OnlineAdmins.ContainsKey(currUser))
                {
                    Lists.OnlineAdmins.Remove(currUser);
                    ChatServer.SendAdminMessage("Room Admin " + currUser + " Left and Logged Out");
                }
                ChatServer.RemoveUser(tcpClient);
            }
            tcpClient.Close();
            srReceiver.Close();
            swSender.Close();
            thrSender.Abort();
        }
        private bool isValidNickname(string nick)
        {
            string AcceptedCharicters = Options.acceptedNicknameCharicters;
            bool isValid = true;
            char[] array = nick.ToCharArray();
            foreach (char c in array)
            {
                if (AcceptedCharicters.Contains(c.ToString())) { }
                else
                {
                    isValid = false;
                }
            }
            return isValid;
        }
        public void sendMessageToUser(string rawMessage)
        {
            swSender.WriteLine(rawMessage);
            swSender.Flush();
        }
        private void AcceptClient()
        {
            srReceiver = new System.IO.StreamReader(tcpClient.GetStream());
            swSender = new System.IO.StreamWriter(tcpClient.GetStream());
            currUser = srReceiver.ReadLine();
            if (currUser != "")
            {
                if (ChatServer.htUsers.Contains(currUser) == true)
                {
                    swSender.WriteLine("0|This nickname is in use.");
                    swSender.Flush();
                    CloseConnection();
                    return;
                }
                else if (currUser.ToLower() == "administrator" || currUser.ToLower() == "server" || currUser.ToLower() == "console" || currUser.ToLower() == "owner" || currUser.ToLower() == "admin")
                {
                    swSender.WriteLine("0|This nickname is unavailable.");
                    swSender.Flush();
                    CloseConnection();
                    return;
                }
                else if (isValidNickname(currUser) == false)
                {
                    swSender.WriteLine("0|This nickname contains invalid characters.");
                    swSender.Flush();
                    CloseConnection();
                    return;
                }
                else if (currUser.Length > Options.maxCharictersInNickname)
                {
                    swSender.WriteLine("0|This nickname has too many characters.");
                    swSender.Flush();
                    CloseConnection();
                    return;
                }
                else
                {
                    hasConnectedYet = true;
                    ChatServer.AddUser(tcpClient, currUser, this);
                    swSender.WriteLine("1");
                    swSender.Flush();
                    swSender.WriteLine("Welcome To The Network. Make Sure You Don't Spam");
                    swSender.WriteLine("--- You Have Successfully Connected ---");
                    swSender.Flush();
                }
            }
            else
            {
                CloseConnection();
                return;
            }

            try
            {
                while ((strResponse = srReceiver.ReadLine()) != "")
                {
                    if (strResponse == null)
                    {
                        CloseConnection();
                    }
                    else
                    {
                        ProcessMessage(strResponse);
                    }
                }
            }
            catch
            {
                CloseConnection();
            }
        }
        private void ProcessMessage(string rawMessage)
        {
            List<string> splitArray = new List<string>(rawMessage.Split(new char[] { ':' }));
            string command = splitArray[0];
            splitArray.RemoveAt(0);
            string message = String.Join(":", splitArray.ToArray());
            splitArray = new List<string>(command.Split(new char[] { ' ' }));
            command = splitArray[0];
            splitArray.RemoveAt(0);
            string[] commandArgs = splitArray.ToArray();

            if (command == "MSG")
            {
                DroneServer.OnCommand(Lists.MessageType.Message, currUser, message, commandArgs);
            }
            else if (command == "ACTION")
            {
                DroneServer.OnCommand(Lists.MessageType.Action, currUser, message, commandArgs);
            }
            else if (command == "ADMIN")
            {
                DroneServer.OnCommand(Lists.MessageType.AdminAction, currUser, message, commandArgs);
            }
            else if (command == "NOTICE")
            {
                DroneServer.OnCommand(Lists.MessageType.Notice, currUser, message, commandArgs);
            }
        }
    }
    #endregion
	
}

