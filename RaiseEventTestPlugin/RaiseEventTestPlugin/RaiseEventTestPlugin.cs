using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Photon.Hive;
using Photon.Hive.Plugin;

using MySql.Data;
using MySql.Data.MySqlClient;

namespace TestPlugin
{
    /* Body of Plugin - Hooking OnRaiseEvent */
    public class RaiseEventTestPlugin : PluginBase
    {
        // Variables 
        private string connStr;
        private MySqlConnection conn;
        private string sql;

        // Getters and Setters
        public string ServerString { get; private set; }
        public int CallsCount { get; private set; }
        public override string Name
        {
            get
            {
                return this.GetType().Name;
            }
        }

        // Constructor 
        public RaiseEventTestPlugin()
        {
            this.UseStrictMode = true;
            this.ServerString = "ServerMessage";
            this.CallsCount = 0;

            // --- Connect to MySQL.
            ConnectToMySQL();
        }

        public override void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            try
            {
                base.OnRaiseEvent(info);
            }
            catch (Exception e)
            {
                this.PluginHost.BroadcastErrorInfoEvent(e.ToString(), info);
                return;
            }

            // Successful Hook
            if (info.Request.EvCode == 1) // simple plugin test
            {
                /// Convert from string to char array 
                string playerName = Encoding.Default.GetString((byte[])info.Request.Data);
                /// Query statement
                sql = "INSERT INTO users (name, date_created) VALUES ('" + playerName + "', now())";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                /// When Clicked, increase count and annouce
                ++this.CallsCount;
                int cnt = this.CallsCount;
                string ReturnMessage = info.Nickname + " clicked the button. Now the count is " + cnt.ToString();
                this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                               senderActor: 0,
                                               targetGroup: 0,
                                               data: new Dictionary<byte, object>() { { (byte)245, ReturnMessage } },
                                               evCode: info.Request.EvCode,
                                               cacheOp: 0);
            }
            else if (info.Request.EvCode == 2) // Login of Viking 
            {
                /// Getting info from client and saving to SQL
                string playerInfo = Encoding.Default.GetString((byte[])info.Request.Data); /// Convert from string to char array 
                string playerPassword = "", playerName = "";
                bool isPassword = false;

                /// Seperate name and password
                for (int i = 0; i < playerInfo.Length; ++i)
                {
                    if (playerInfo[i] == ',')
                    {
                        isPassword = true;
                        continue;
                    }

                    /// Set name and password
                    if (!isPassword)
                        playerName += playerInfo[i];
                    else
                        playerPassword += playerInfo[i];
                }

                /// Check using playerName for existing accounts
                if (!NameExist(playerName, info.Request.EvCode))
                {
                    /// Query statement
                    sql = "INSERT INTO users (name, password, date_created) VALUES ('" + playerName + "','" + playerPassword + "', now())";
                    ServerString = "New Account created";
                }
                else
                {
                    /// Check if the password is correct
                    if (!PasswordMatch(playerName, playerPassword))
                    {
                        sql = "UPDATE users SET password='" + playerPassword + "' WHERE name='" + playerName + "'";
                        ServerString = "password is updated";
                    }
                    else
                    {
                        /// if username and password exists, send login is ok
                        ServerString = "Login Result = OK";
                    }
                }

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                /// send back message to server
                this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                               senderActor: 0,
                                               targetGroup: 0,
                                               data: new Dictionary<byte, object>() { { (byte)245, ServerString } },
                                               evCode: info.Request.EvCode,
                                               cacheOp: 0);
            }

            else if (info.Request.EvCode == 3) // Variables of Viking
            {
                string playerInfo = Encoding.Default.GetString((byte[])info.Request.Data); /// Convert from string to char array 
                string playerPos = "", playerName = "";
                int playerHealth = 0, playerScore = 0;

                string[] m_storeInfo = playerInfo.Split(',');
                playerName = m_storeInfo[0];
                playerPos = m_storeInfo[1];
                playerHealth = int.Parse(m_storeInfo[2]);
                playerScore = int.Parse(m_storeInfo[3]);

                /// Check using playerName for existing accounts
                if (!NameExist(playerName, info.Request.EvCode))
                {
                    /// Query statement
                    sql = "INSERT INTO user_position (name, position, health, score) VALUES ('" + playerName + "','" + playerPos + "','" + playerHealth + "','" + playerScore + "')";
                }
                else
                {
                    sql = "UPDATE user_position SET position='" + playerPos + "', health='" + playerHealth.ToString() + "', score='" + playerScore.ToString() + "' WHERE name='" + playerName + "'";
                }

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                
                /// send back message to server
                this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                               senderActor: 0,
                                               targetGroup: 0,
                                               data: new Dictionary<byte, object>() { { (byte)245, null } },
                                               evCode: info.Request.EvCode,
                                               cacheOp: 0);

            }

            else if (info.Request.EvCode == 4) // Get position of player
            {
                string playerInfo = Encoding.Default.GetString((byte[])info.Request.Data); /// Convert from string to char array 
                string playerName = playerInfo;

                if (NameExist(playerName, 3))
                {
                    /// Query statement
                    sql = "SELECT position FROM user_position WHERE name='" + playerName + "'";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    object position = cmd.ExecuteScalar();


                    /// send back message to server
                    this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                                   senderActor: 0,
                                                   targetGroup: 0,
                                                   data: new Dictionary<byte, object>() { { (byte)245, position } },
                                                   evCode: info.Request.EvCode,
                                                   cacheOp: 0);
                }
                else
                {
                    /// send back message to server
                    this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                                   senderActor: 0,
                                                   targetGroup: 0,
                                                   data: new Dictionary<byte, object>() { { (byte)245, null } },
                                                   evCode: info.Request.EvCode,
                                                   cacheOp: 0);
                }
            }

            else if (info.Request.EvCode == 5) // get health of player 
            {
                string playerInfo = Encoding.Default.GetString((byte[])info.Request.Data); /// Convert from string to char array 
                string playerName = playerInfo;

                if (NameExist(playerName, 3))
                {
                    /// Query statement
                    sql = "SELECT health FROM user_position WHERE name='" + playerName + "'";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    object hp = cmd.ExecuteScalar();

                    /// send back message to server
                    this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                                   senderActor: 0,
                                                   targetGroup: 0,
                                                   data: new Dictionary<byte, object>() { { (byte)245, hp } },
                                                   evCode: info.Request.EvCode,
                                                   cacheOp: 0);
                }
                else
                {
                    /// send back message to server
                    this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                                   senderActor: 0,
                                                   targetGroup: 0,
                                                   data: new Dictionary<byte, object>() { { (byte)245, null } },
                                                   evCode: info.Request.EvCode,
                                                   cacheOp: 0);
                }
            }

            else if (info.Request.EvCode == 6) // Add friends to vikings
            {
                string playerInfo = Encoding.Default.GetString((byte[])info.Request.Data); /// Convert from string to char array 
                string[] store = playerInfo.Split(',');
                string playerName = store[0];
                string friendName = store[1];

                sql = "INSERT INTO user_friends (name, friend_name) VALUES ('" + playerName + "','" + friendName + "')";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                /// send back message to server
                this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                               senderActor: 0,
                                               targetGroup: 0,
                                               data: new Dictionary<byte, object>() { { (byte)245, null } },
                                               evCode: info.Request.EvCode,
                                               cacheOp: 0);
            }

            else if (info.Request.EvCode == 7) // get score of player
            {
                string playerInfo = Encoding.Default.GetString((byte[])info.Request.Data); /// Convert from string to char array 
                string playerName = playerInfo;

                if (NameExist(playerName, 3))
                {
                    /// Query statement
                    sql = "SELECT score FROM user_position WHERE name='" + playerName + "'";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    object score = cmd.ExecuteScalar();

                    /// send back message to server
                    this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                                   senderActor: 0,
                                                   targetGroup: 0,
                                                   data: new Dictionary<byte, object>() { { (byte)245, score } },
                                                   evCode: info.Request.EvCode,
                                                   cacheOp: 0);
                }
                else
                {
                    /// send back message to server
                    this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                                   senderActor: 0,
                                                   targetGroup: 0,
                                                   data: new Dictionary<byte, object>() { { (byte)245, null } },
                                                   evCode: info.Request.EvCode,
                                                   cacheOp: 0);
                }
            }


            else if (info.Request.EvCode == 8) // Add friends to vikings
            {
                string playerInfo = Encoding.Default.GetString((byte[])info.Request.Data); /// Convert from string to char array 
                string playerName = playerInfo;

                sql = "SELECT GROUP_CONCAT(friend_name) FROM user_friends WHERE name='" + playerName + "'";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                object Friends = cmd.ExecuteScalar();

                /// send back message to server
                this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                               senderActor: 0,
                                               targetGroup: 0,
                                               data: new Dictionary<byte, object>() { { (byte)245, Friends } },
                                               evCode: info.Request.EvCode,
                                               cacheOp: 0);
            }

            else if (info.Request.EvCode == 9) // Add friends to vikings
            {
                string playerInfo = Encoding.Default.GetString((byte[])info.Request.Data); /// Convert from string to char array 
                string[] store = playerInfo.Split(',');
                string playerName = store[0];
                string partyName = store[1];

                sql = "INSERT INTO user_party (name, party_name) VALUES ('" + playerName + "','" + partyName + "')";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                /// send back message to server
                this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                               senderActor: 0,
                                               targetGroup: 0,
                                               data: new Dictionary<byte, object>() { { (byte)245, null } },
                                               evCode: info.Request.EvCode,
                                               cacheOp: 0);
            }

            else if (info.Request.EvCode == 10) // Add friends to vikings
            {
                string playerInfo = Encoding.Default.GetString((byte[])info.Request.Data); /// Convert from string to char array 
                string playerName = playerInfo;

                sql = "SELECT GROUP_CONCAT(party_name) FROM user_party WHERE name='" + playerName + "'";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                object Party = cmd.ExecuteScalar();
                string toreturn = playerName + " " + Party.ToString();

                /// send back message to server
                this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                               senderActor: 0,
                                               targetGroup: 0,
                                               data: new Dictionary<byte, object>() { { (byte)245, toreturn } },
                                               evCode: info.Request.EvCode,
                                               cacheOp: 0);
            }

        }

        /* Linking to SQL */
        public void ConnectToMySQL()
        {
            // Connect to MySQL
            connStr = "server=localhost;user=root;database=photon;port=3306;password=marrot4299";
            conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /* Un-link from Server */
        public void DisconnectFromMySQL()
        {
            conn.Close();
        }

        /* Checking if Name exists */
        public bool NameExist(string _playerName, int _type)
        {
            /// Query Statement
            switch (_type)
            {
                case 2:
                    sql = "SELECT name FROM users WHERE name='" + _playerName + "'";
                    break;
                case 3:
                    sql = "SELECT name FROM user_position WHERE name='" + _playerName + "'";
                    break;
            }
            
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            object obj = cmd.ExecuteScalar();

            if (obj != null)
                return true;

            return false;
        }

        /* Checking if Password exists */
        public bool PasswordMatch(string _playerName, string _password)
        {
            sql = "SELECT password FROM users WHERE name='" + _playerName + "' and password='" + _password + "'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            object obj = cmd.ExecuteScalar();

            if (obj != null)
                return true;

            return false;
        }
    }
}