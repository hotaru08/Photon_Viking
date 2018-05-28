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
                if (!NameExist(playerName))
                {
                    /// Query statement
                    sql = "INSERT INTO users (name, password, date_created) VALUES ('" + playerName + "','" + playerPassword + "', now())";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    /// Check if the password is correct
                    if (!PasswordMatch(playerName, playerPassword))
                    {
                        sql = "UPDATE users SET password='" + playerPassword + "' WHERE name='" + playerName + "'";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                    }
                }

                /// if username exists, send login is ok
                ServerString = "Login Result = OK";

                /// send back message to server
                this.PluginHost.BroadcastEvent(target: ReciverGroup.All,
                                               senderActor: 0,
                                               targetGroup: 0,
                                               data: new Dictionary<byte, object>() { { (byte)245, ServerString } },
                                               evCode: info.Request.EvCode,
                                               cacheOp: 0);
            }
        }

        /* Linking to SQL */
        public void ConnectToMySQL()
        {
            // Connect to MySQL
            connStr = "server=localhost;user=root;database=photon;port=3306;password=<3LiveSIP";
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
        public bool NameExist(string _playerName)
        {
            /// Query Statement
            sql = "SELECT name FROM users WHERE name='" + _playerName + "'";
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