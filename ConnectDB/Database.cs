using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;


namespace ConnectDB
{
    public class Database // create more class in database, try to use abstract class
    {
        string createTableUser, createTableWinLose;
        private string Password;
        //string UserNameID, PassWord, UserName;
        public void connectDatabase()
        {
            createTableUser = @"CREATE TABLE IF NOT EXISTS [User] (
                          [ID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                          [FirstName] VARCHAR(2048)  NULL,
                          [LastName] VARCHAR(2048)  NULL,
                          [Username] VARCHAR(2048) NULL,
                          [Password] VARCHAR(2048)  NULL,
                          [UsernameID] VARCHAR(4) NULL
                          )";

            createTableWinLose = @"CREATE TABLE IF NOT EXISTS [WinLose] (
                          [UsernameID] VARCHAR(4) NULL,
                          [WonWords] VARCHAR(2048) NULL,
                          [LostWords] VARCHAR(2048) NULL
                          )";

            string file = @".\databaseFile.db3";

            if (File.Exists(file) == false)
            {
                SQLiteConnection.CreateFile("databaseFile.db3");  // Create the file which will be hosting our database
            }

            using (SQLiteConnection con = new SQLiteConnection("data source=databaseFile.db3"))
            {
                using (SQLiteCommand com = new SQLiteCommand(con))
                {


                    con.Open();                             // Open the connection to the database

                    com.CommandText = createTableUser;     // Set CommandText to our query that will create the table
                    com.ExecuteNonQuery();                  // Execute the query

                    com.CommandText = createTableWinLose;     // Set CommandText to our query that will create the table
                    com.ExecuteNonQuery();

                    con.Close();        // Close the connection to the database
                }
            }

        }

        public void enterData()
        {
            //Random newRan = new Random();
            //int UsernameID = newRan.Next(1000, 9999);

            Console.Write("First Name: ");
            string FirstName = Console.ReadLine();

            Console.Write("Last Name: ");
            string LastName = Console.ReadLine();

            string Username = $"{LastName.ToLower()}{FirstName.ToLower()}";
            UserName = Username;

            Regex newRegex = new Regex(@"^[A-Za-z0-9]{1,15}$");

            while (true)
            {
                Console.Write("Please make a password, max of 15 characters: ");
                Password = Console.ReadLine();
                PassWord = Password;
                if (newRegex.IsMatch(Password) == true)
                {
                    break;
                }
                else if (newRegex.IsMatch(Password) == false)
                {
                    Console.WriteLine("The password can only contain letters and numbers");
                }

            }
            //TODO make below code into other method -- make first and last name as properties
            using (SQLiteConnection con = new SQLiteConnection("data source=databaseFile.db3"))
            {
                using (SQLiteCommand com = new SQLiteCommand(con))
                {


                    con.Open();                             // Open the connection to the database

                    com.CommandText = "Select * FROM User";

                    Random newRan = new Random();
                    int makeUsernameID = newRan.Next(1000, 9999);
                    //UserNameID = makeUsernameID.ToString();

                    //make sure UsernameID is unique
                    using (SQLiteDataReader reader = com.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["UsernameID"].ToString() == makeUsernameID.ToString())
                            {
                                makeUsernameID = newRan.Next(1000, 9999);
                                //UserNameID = makeUsernameID.ToString();
                            }
                        }
                    }

                    com.CommandText = $"INSERT INTO User (FirstName,LastName,UserName,Password,UsernameID) Values ('{FirstName}','{LastName}','{Username}','{Password}','{makeUsernameID.ToString()}')";     // Add the first entry into our database 
                    com.ExecuteNonQuery();      // Execute the query

                    //com.CommandText = $"insert into WinLose (UsernameID) values ('{UserNameID}')";
                    //com.ExecuteNonQuery();
                    //com.CommandText = "Select * FROM User";

                    //using (SQLiteDataReader reader = com.ExecuteReader())
                    //{
                    //    while (reader.Read())
                    //    {
                    //        Console.WriteLine(reader["UsernameID"] + " | " + reader["Username"]);
                    //    }
                    //}
                    personExists();

                    Console.ReadLine();
                    con.Close();        // Close the connection to the database
                }
            }
        }

        public void personExists()
        {
            begin:
            Console.Write("Please type username:\t");
            UserName = Console.ReadLine();

            Console.Write("Please type password:\t");
            PassWord = Console.ReadLine();

            using (SQLiteConnection con = new SQLiteConnection("data source=databaseFile.db3"))
            {
                con.Open();
                
                using (SQLiteCommand com = new SQLiteCommand(con))
                {
                    //Need to have query that goes to property for UsernameID to reference other table
                    string usernameQuery = $"Select Username from User where Username like '%{UserName}%';";
                    string passwordQuery = $"Select Password from User where Password like '%{PassWord}%' and Username like '%{UserName}%';"; // can make into array
                    
                    string UsernameQueryResult;
                    string PasswordQueryResult;
                    try
                    {
                        
                        com.CommandText = usernameQuery;
                        UsernameQueryResult = com.ExecuteScalar().ToString();

                        com.CommandText = passwordQuery;
                        PasswordQueryResult = com.ExecuteScalar().ToString();

                        

                        if (UsernameQueryResult == UserName && PasswordQueryResult == PassWord)
                        {
                            Console.WriteLine("Login Successful");
                        }
                        else
                        {
                            Console.WriteLine("Login Unsuccessful. Please try again");
                            goto begin;
                        }
                    }
                    catch (NullReferenceException)
                    {
                        Console.WriteLine("Error, please try again");
                        goto begin;
                    }
                    //Console.ReadLine(); // Make sure the code can be read before continuing -- write line for people to continue
                    con.Close();        // Close the connection to the database
                }
            }
        }

        public void winWord(string win)
        {
            using (SQLiteConnection con = new SQLiteConnection("data source=databaseFile.db3"))
            {
                using (SQLiteCommand com = new SQLiteCommand(con))
                {
                    string winQuery = $"select WonWords from WinLose where UsernameID like '{UserNameID}'";
                    string UsernameIDQuery = $"Select UsernameID from User where Password like '%{PassWord}%' and Username like '%{UserName}%';";
                    con.Open();


                    com.CommandText = UsernameIDQuery;
                    UserNameID = com.ExecuteScalar().ToString();

                    com.CommandText = $"Insert into WinLose (UsernameID,WonWords) values ('{UserNameID}','{win}')";
                    com.ExecuteNonQuery();

                    //com.CommandText = winQuery;
                    com.CommandText = $"select * from WinLose where UsernameID like '{UserNameID}'";
                    using (SQLiteDataReader reader = com.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine(reader["UsernameID"] + " | " + reader["WonWords"]);
                        }
                    }

                    con.Close();
                }
            }
        }

        public void lostWord(string lost)
        {
            using (SQLiteConnection con = new SQLiteConnection("data source=databaseFile.db3"))
            {
                using (SQLiteCommand com = new SQLiteCommand(con))
                {
                    string LoseQuery = $"select LostWords from WinLose where UsernameID like '{UserNameID}'";
                    string UsernameIDQuery = $"Select UsernameID from User where Password like '%{PassWord}%' and Username like '%{UserName}%';";
                    con.Open();

                    com.CommandText = UsernameIDQuery;
                    UserNameID = com.ExecuteScalar().ToString();

                    com.CommandText = $"Insert into WinLose (UsernameID,LostWords) values ('{UserNameID}','{lost}')";
                    com.ExecuteNonQuery();

                    com.CommandText = LoseQuery;
                    using (SQLiteDataReader reader = com.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine(reader["UsernameID"] + " | " + reader["LostWords"]);
                        }
                    }

                    con.Close();
                }
            }
        }

        public void checkWinWords(string word) // use linq statment to query
        {
            using (SQLiteConnection con = new SQLiteConnection("data source=databaseFile.db3"))
            {
                using (SQLiteCommand com = new SQLiteCommand(con))
                {
                    string query = $"select UsernameID from User where Username like '{UserName}'";
                    string winQuery = $"select WonWords from WinLose where UsernameID like '{UserNameID}'";
                    List<string> winWordsList = new List<string>();
                    con.Open();
                    com.CommandText = query;
                    UserNameID = com.ExecuteScalar().ToString();
                    com.CommandText = winQuery;
                    //using (SQLiteDataReader reader = com.ExecuteReader())
                    //{
                    //    while (reader.Read())
                    //    {
                    //        //Console.WriteLine(reader["UsernameID"] + " | " + reader["WonWords"]);
                    //        winWordsList.Add(reader["WonWords"].ToString());
                            
                    //    }
                    //}
                    //IEnumerable<string> matchingvalues = winQuery.Where(stringToCheck => stringToCheck.Contains(word));
                    //Console.WriteLine(matchingvalues.ToString());
                    Console.ReadLine();
                    
                    try
                    {
                        //IEnumerable<string> matchingvalues = winWordsList.Where(stringToCheck => stringToCheck.Contains(word));
                        //Console.WriteLine(matchingvalues.ToString());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        //throw;
                    }
                    con.Close();
                    //if(matchingvalues.ToString() == word)
                    //{

                    //}
                }
            }
        }

        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string UserNameID { get; set; }
    }
}
