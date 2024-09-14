using System;
using System.Collections.Generic;
using System.Net;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Data.SqlClient;
using System.Collections;
using Newtonsoft.Json;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;

class Program
{
    private static List<string> OnlineList = new List<string>();
    private static List<Socket> clients = new List<Socket>();
    private static List<Socket> users = new List<Socket>();
    private static Socket serverSocket;
    private static SqlConnection sqlctn = new SqlConnection(""); //your sql connection string
    private static Dictionary<Socket, int> clientRoomMapping = new Dictionary<Socket, int>();
    //Announcement
    private static string duyuru = "Hoşgeldiniz Resmi Sunucudasınız! \nGiriş yapabilir veya Kayıt Olabilirsiniz. \nKeyfinize Bakın! \n \nWelcome, you are on the Official Server! \nYou can Login or Register. \nEnjoy!";

    static void Main(string[] args)
    {
        try
        {
            int port = 8080;
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            serverSocket.Listen(20); //Max Clients
            Console.WriteLine("Server initialized. Waiting for connections...");
        }
        catch { Console.WriteLine("Socket Failed to Start"); }

        Thread acceptThread = new Thread(AcceptClients);
        acceptThread.Start();
        Console.WriteLine("Command List for 'help' : ");
        while (true)
        {
            string input = Console.ReadLine();

            if (input.ToLower() == "stop")
            {
                Console.WriteLine("Stopping Server");
                Environment.Exit(0);
            }

            string result = Com(input);
            Console.WriteLine(result);
        }

        static string Com(string input)
        {
            string command = input.ToLower();
            if (command.StartsWith("help"))
            {
                string output = "Commands \nListing Commands - 'help' \nBan User - 'ban username' \nUnBan User - 'unban usernamme' \nAdminned User - 'admin username' \nStopping Server - 'stop' ";
                return output;
            }
            else if (command.StartsWith("ban "))
            {
                string username = command.Substring(4);
                UserLevel(username,0);
                string output = $"Banned {username}";
                return output;
            }
            else if (command.StartsWith("unban "))
            {
                string username = command.Substring(6);
                UserLevel(username, 1);
                string output = $"Un Banned {username}";
                return output;
            }
            else if (command.StartsWith("admin "))
            {
                string username = command.Substring(6);
                UserLevel(username, 3);
                string output = $"Adminned {username}";
                return output;
            }
            else
            {
                return "Invalid Command try 'help' : ";
            }
        }

}

    
    private static void AcceptClients()
    {
        while (true)
        {
            Socket clientSocket = serverSocket.Accept();
            clients.Add(clientSocket);
            IPEndPoint remoteEndPoint = (IPEndPoint)clientSocket.RemoteEndPoint;
            string clientip = remoteEndPoint.Address.ToString();

            try 
            { 
                sqlctn.Open();
                using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Table_BannedIPs WHERE ip = @IPAddress", sqlctn))
                {
                    command.Parameters.AddWithValue("@IPAddress", clientip);
                    int count = (int)command.ExecuteScalar();

                    if (count > 0) //check ban
                    {
                        Console.WriteLine($"{clientip} is banned. Closing connection.");
                        clientSocket.Close(); 
                        continue; 
                    }
                    else 
                    {
                        Console.WriteLine($"{clientip} Connected");
                        Thread clientThread = new Thread(() => HandleClient(clientSocket, clientip));
                        clientThread.Start(); 
                        var announcement = new { Type = "[ANN]", Message = $"{duyuru}" }; string jannouncement = JsonConvert.SerializeObject(announcement); 
                        SingleMessage(jannouncement, clientSocket); 
                    }
                }
                
            }
            catch (Exception ex) { Console.WriteLine(ex); }
            finally { sqlctn.Close(); }
            
        }
    }

    static void SendOnline(string user, bool x)
    {
        if (x)
        {
            
            if (!OnlineList.Contains(user)) 
            {
                OnlineList.Add(user);
            }
        }
        else
        {
            OnlineList.Remove(user);
        }
        var Onlines = new { Type = "[ONLINE]", OnlinesList = OnlineList };
        foreach (Socket u in users) 
        {
            string jlogin = JsonConvert.SerializeObject(Onlines);
            SingleMessage(jlogin, u);
        }
    }


    private static void HandleClient(Socket clientSocket,string ip)
    {
        byte[] buffer = new byte[1024*600];
        int receivedBytes;
        bool loginned = false;
        int userID = -1;
        int Room = -1;
        int userType = -1;
        var lastmsg = new {Type = "[MSG]" , Message = ""};
        string username = null;
        
        while (true)
        {
            if (loginned && (userType = GetUserType(username)) < 1)
            {
                Console.WriteLine($"Banned user {username} - {ip} is banned");
                try
                {
                    sqlctn.Open();
                    using (SqlCommand addbanip = new SqlCommand("INSERT INTO Table_BannedIPs(ip) VALUES(@ip)", sqlctn))
                    {
                        addbanip.Parameters.AddWithValue("@ip", ip);


                        int rowsAffected = addbanip.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Console.WriteLine($"{ip} was successfully banned.");
                        }
                        else
                        {
                            Console.WriteLine($"{ip} could not be banned.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                finally { sqlctn.Close(); }
                break;
            }

            try
            {
                receivedBytes = clientSocket.Receive(buffer);
                if (receivedBytes == 0) break; 
                string text = Encoding.UTF8.GetString(buffer, 0, receivedBytes);
                Console.WriteLine($"{ip} - {text.ToString()}");
                JObject messageData = JObject.Parse(text);
                string message = messageData["Type"].ToString();
                JArray messagesArray = (JArray)messageData["Messages"];

                if (message.StartsWith("[LOGIN]"))
                {
                    username = messageData["Username"].ToString();
                    string password = messageData["Password"].ToString();

                    //SQL

                    try
                    {
                        sqlctn.Open();
                        using (SqlCommand command = new SqlCommand("SELECT usertype FROM Table_User WHERE username = @username AND userpassword = @password", sqlctn))
                        {
                            command.Parameters.AddWithValue("@username", username);
                            command.Parameters.AddWithValue("@password", password);
                            object result = command.ExecuteScalar();
                            if (result != null)
                            {
                                userType = Convert.ToInt32(result);
                                if (userType > 0)
                                {
                                    using (SqlCommand getidcmd = new SqlCommand("SELECT userID FROM Table_User WHERE username = @username AND userpassword = @password", sqlctn))
                                    {
                                        getidcmd.Parameters.AddWithValue("@username", username);
                                        getidcmd.Parameters.AddWithValue("@password", password);

                                        using (SqlDataReader reader = getidcmd.ExecuteReader())
                                        {
                                            if (reader.Read())
                                            {
                                                userID = reader.GetInt32(0);

                                            }
                                            reader.Close();
                                        }

                                        var rooms = new List<object>();

                                        SqlCommand getrooms = new SqlCommand("SELECT RoomID, RoomName FROM Table_Room WHERE RoomLevel <= @p1", sqlctn);
                                        getrooms.Parameters.AddWithValue("@p1", userType);

                                        using (SqlDataReader roomreader = getrooms.ExecuteReader())
                                        {
                                            while (roomreader.Read())
                                            {
                                                int roomId = roomreader.GetInt32(0);
                                                string roomName = roomreader.GetString(1);
                                                rooms.Add(new { RoomID = roomId, RoomName = roomName });
                                            }
                                            roomreader.Close();
                                        }

                                        var login = new
                                        {
                                            Type = "[LGN1]",
                                            Username = username,
                                            Rooms = rooms
                                        };

                                        string jlogin = JsonConvert.SerializeObject(login);
                                        SingleMessage(jlogin, clientSocket);
                                        loginned = true;
                                        users.Add(clientSocket);
                                        SendOnline(username, true);
                                        Console.WriteLine($"{username} giriş yaptı {ip}");

                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"Banned user {username} - {ip} is banned");
                                    using (SqlCommand addbanip = new SqlCommand("INSERT INTO Table_BannedIPs(ip) VALUES(@ip)", sqlctn))
                                    {
                                        addbanip.Parameters.AddWithValue("@ip", ip);
                                        try
                                        {
                                            int rowsAffected = addbanip.ExecuteNonQuery();
                                            if (rowsAffected > 0)
                                            {
                                                Console.WriteLine($"{ip} was successfully banned.");
                                            }
                                            else
                                            {
                                                Console.WriteLine($"{ip} could not be banned.");
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"Error: {ex.Message}");
                                        }
                                    }
                                    var msg = new { Type = "[LGN2]", Message = "You are banned from this server!!!" };
                                    string jsonMessage = JsonConvert.SerializeObject(msg);
                                    SingleMessage(jsonMessage, clientSocket);
                                    break;
                                }
                            }
                            else
                            {
                                var msg = new { Type = "[LGN3]", Message = "Username or password incorrect." };
                                string jsonMessage = JsonConvert.SerializeObject(msg);
                                SingleMessage(jsonMessage, clientSocket);
                                Console.WriteLine($"{ip} Username or password incorrect.");
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                    finally { sqlctn.Close(); }

                }
                else if (message.StartsWith("[REGISTER]"))
                {

                    username = messageData["Username"].ToString();
                    string password = messageData["Password"].ToString();
                    string email = messageData["Email"].ToString();

                    //SQL 

                    using (SqlCommand checkCommand = new SqlCommand("SELECT COUNT(*) FROM Table_User WHERE username = @username OR useremail = @useremail", sqlctn))
                    {

                        checkCommand.Parameters.AddWithValue("@username", username);
                        checkCommand.Parameters.AddWithValue("@useremail", email);
                        try
                        {
                            sqlctn.Open();
                            int userCount = (int)checkCommand.ExecuteScalar();

                            if (userCount > 0)
                            {
                                var msg = new { Type = "[REG]", Message = "Username or email already registered." };
                                string jsonMessage = JsonConvert.SerializeObject(msg);
                                Console.WriteLine($"{ip} Username or email already registered.");
                                SingleMessage(jsonMessage, clientSocket);
                            }

                            else
                            {
                                try
                                {
                                    using (SqlCommand command = new SqlCommand("INSERT INTO Table_User(username, userpassword, useremail, usertype) VALUES(@username, @password, @email, 1)", sqlctn))
                                    {
                                        command.Parameters.AddWithValue("@username", username);
                                        command.Parameters.AddWithValue("@password", password);
                                        command.Parameters.AddWithValue("@email", email);
                                        int rowsAffected = command.ExecuteNonQuery();
                                        if (rowsAffected > 0)
                                        {

                                            var msg = new { Type = "[REG]", Message = "You have successfully registered. Please log in." };
                                            string jsonMessage = JsonConvert.SerializeObject(msg);
                                            Console.WriteLine($"User {username} at {ip} has been successfully registered!");
                                            SingleMessage(jsonMessage, clientSocket);

                                        }
                                        else
                                        {
                                            var msg = new { Type = "[REG]", Message = "Registration Error!" };
                                            string jsonMessage = JsonConvert.SerializeObject(msg);
                                            Console.WriteLine($"{ip} User could not be registered.");
                                            SingleMessage(jsonMessage, clientSocket);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.ToString());
                                }

                            }

                        }
                        catch { Console.WriteLine("No SQL Server"); }
                        finally { sqlctn.Close(); }

                    }
                    username = null;

                }
                else if (message.StartsWith("[MSG]") && loginned)
                {
                    string msg = messageData["Message"].ToString();
                    string image = messageData["Image"].ToString();
                    int roomvalue = Convert.ToInt32(messageData["RoomID"]);

                    //SQL

                    SendSQLMsg(msg, image, userID, roomvalue);

                    var onemsg = new
                    {
                        Type = "[ROOM]",
                        Messages = GetRoomMsg(Room, 1)
                    };

                    if (onemsg.Messages.Count > 0)
                    {
                        var lastMessage = onemsg.Messages.Last();
                        lastmsg = new { Type = "[MSG]", Message = (string)((dynamic)lastMessage).Content };
                    }

                    SendRoom(users, roomvalue, onemsg);
                }
                else if (message.StartsWith("[REQ]") && loginned)
                {
                    int roomID = Convert.ToInt32(messageData["RoomID"]);
                    Room = Convert.ToInt32(roomID);

                    if (clientRoomMapping.ContainsKey(clientSocket))
                    {
                        clientRoomMapping[clientSocket] = Room;
                    }
                    else
                    {
                        clientRoomMapping.Add(clientSocket, Room);
                    }

                    // SQL

                    var msg = new
                    {
                        Type = "[ROOM]",
                        Messages = GetRoomMsg(roomID, 10)
                    };


                    if (msg.Messages.Count > 0)
                    {
                        var lastMessage = msg.Messages.Last();
                        lastmsg = new { Type = "[MSG]", Message = (string)((dynamic)lastMessage).Content };
                    }

                    string jmsg = JsonConvert.SerializeObject(msg);
                    SingleMessage(jmsg, clientSocket);
                }
                else break;

                
            }
            catch (SocketException)
            {
                break;
            }
        }

        if (loginned) { users.Remove(clientSocket); SendOnline(username, false); }
        clients.Remove(clientSocket);
        clientSocket.Close();
        Console.WriteLine($"{ip} Disconnected");
    }

    private static void SendRoom(List<Socket> userSockets, int roomId, dynamic msg)
    {
        foreach (Socket user in userSockets) 
        {
            if (clientRoomMapping.TryGetValue(user, out int userRoomId))
            {
                if (userRoomId == roomId)
                {
                    string jmsg = JsonConvert.SerializeObject(msg);
                    SingleMessage(jmsg, user);
                }
                
            }
        }
    }

    private static void SingleMessage(string j, Socket c)
    {
        byte[] data = Encoding.UTF8.GetBytes(j);
        c.Send(data);

    }

    private static void SendSQLMsg(string msg, string image, int ID, int RmID)
    {
        try
        {
            DateTime now = DateTime.Now;
            sqlctn.Open();

            if (string.IsNullOrEmpty(image) || image == "NULL")
            {
                using (SqlCommand command = new SqlCommand("INSERT INTO Table_Message(message, MDate, RoomID, userID) VALUES(@message, @MDate, @RoomID, @userID)", sqlctn))
                {
                    command.Parameters.AddWithValue("@message", msg);
                    command.Parameters.AddWithValue("@MDate", now);
                    command.Parameters.AddWithValue("@RoomID", RmID);
                    command.Parameters.AddWithValue("@userID", ID);
                    command.ExecuteNonQuery();
                }
            }

            else
            {
                using (SqlCommand command = new SqlCommand("INSERT INTO Table_Message(image, MDate, RoomID, userID) VALUES(@image, @MDate, @RoomID, @userID)", sqlctn))
                {
                    command.Parameters.AddWithValue("@MDate", now);
                    command.Parameters.AddWithValue("@RoomID", RmID);
                    command.Parameters.AddWithValue("@userID", ID);

                    byte[] imageData = Convert.FromBase64String(image);
                    command.Parameters.AddWithValue("@image", imageData);
                    command.ExecuteNonQuery();
                }
            }
            
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            sqlctn.Close();
        }
    }



    private static List<object> GetRoomMsg(int rmid, int c)
    {
        List<object> messages = new List<object>();

        try
        {
            sqlctn.Open();
            using (SqlCommand command = new SqlCommand("SELECT TOP (@c) * FROM Table_Message WHERE RoomID = @p1 ORDER BY MDate DESC", sqlctn))
            {
                command.Parameters.AddWithValue("@p1", rmid);
                command.Parameters.AddWithValue("@c", c);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        
                        string msgID = reader["msgID"].ToString();
                        string messageContent = reader["message"] != DBNull.Value ? reader["message"].ToString() : "NULL"; 
                        string imageBase64 = reader["image"] != DBNull.Value ? Convert.ToBase64String((byte[])reader["image"]) : "NULL"; 
                        DateTime mDate = (DateTime)reader["MDate"]; 
                        string roomID = reader["RoomID"].ToString(); 
                        string userID = reader["userID"].ToString();
                        string username = GetUsernameByUserId(userID);

                        var message = new
                        {
                            MsgID = msgID,
                            Content = messageContent,
                            Image = imageBase64,
                            Date = mDate,
                            RoomID = roomID,
                            UserID = userID,
                            Username = username
                        };

                        
                        messages.Add(message);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            sqlctn.Close();
        }

        
        return messages.OrderBy(m => ((dynamic)m).Date).ToList();
    }


    private static string GetUsernameByUserId(string userId)
    {
        string username = "Unknown";
        using (SqlConnection newSqlConnection = new SqlConnection(sqlctn.ConnectionString))
        {
            try
            {

                newSqlConnection.Open();
                using (SqlCommand command = new SqlCommand("SELECT username FROM Table_User WHERE userID = @userID", newSqlConnection))
                {
                    command.Parameters.AddWithValue("@userID", userId);
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        username = result.ToString();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally { newSqlConnection.Close(); }
        }
        return username;
    }



    private static void DeleteAllRecords()
    {
        try
        {
            sqlctn.Open();

            using (SqlCommand command = new SqlCommand("DELETE FROM Table_Message", sqlctn))
            {
                // Execute the command to delete all records
                command.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally { sqlctn.Close(); }
    }

    private static void UserLevel(string username,int level)
    {
        try
        {
            sqlctn.Open();
            using (SqlCommand command = new SqlCommand("UPDATE Table_User SET usertype = @level WHERE username = @username", sqlctn))
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@level", level);
                command.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            sqlctn.Close();
        }
    }

    private static int GetUserType(string username)
    {
        int userType = -1;
        try
        {
            sqlctn.Open();
            string query = "SELECT usertype FROM Table_User WHERE username = @username";

            using (SqlCommand command = new SqlCommand(query, sqlctn))
            {
                command.Parameters.AddWithValue("@username", username);
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    userType = Convert.ToInt32(result);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            sqlctn.Close();
        }

        return userType;
    }

}
