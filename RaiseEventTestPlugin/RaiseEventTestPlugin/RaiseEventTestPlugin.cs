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
            else if (info.Request.EvCode == 2) // Viking 
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

            else if (info.Request.EvCode == 3)
            {
                string playerInfo = Encoding.Default.GetString((byte[])info.Request.Data); /// Convert from string to char array 
                string playerPos = "", playerName = "";
                bool isPosition = false;

                for (int i = 0; i < playerInfo.Length; ++i)
                {
                    if (playerInfo[i] == ',')
                    {
                        isPosition = true;
                        continue;
                    }

                    /// Set name and password
                    if (!isPosition)
                        playerName += playerInfo[i];
                    else
                        playerPos += playerInfo[i];
                }

                /// Check using playerName for existing accounts
                if (!NameExist(playerName, info.Request.EvCode))
                {
                    /// Query statement
                    sql = "INSERT INTO user_position (name, position) VALUES ('" + playerName + "','" + playerPos + "')";
                }
                else
                {
                    sql = "UPDATE user_position SET position='" + playerPos + "' WHERE name='" + playerName + "'";
                }

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();


                /// send back message to server
                this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                               senderActor: 0,
                                               targetGroup: 0,
                                               data: new Dictionary<byte, object>() { { (byte)245, playerPos } },
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