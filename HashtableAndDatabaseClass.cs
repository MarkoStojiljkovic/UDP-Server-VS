using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace UDPServer
{
    class HashtableAndDatabaseClass
    {

        static Hashtable ht = new Hashtable();

        public static string FetchIPFromMac(string s)
        {
            if (!ht.ContainsKey(s))
            {
                return null;
            }
            return ht[s].ToString();
        }

        public static void AddToHashTable(string key, string value)
        {
            if (ht.ContainsKey(key))
            {
                return; // Already have that MAC address, ignore
            } 

            ht.Add(key, value);
            return;
        }

        public static void FetchIPFromDatabase(string connectionString)
        {

            OracleConnection conn = new OracleConnection(connectionString);
            conn.Open();
            OracleCommand cmd = new OracleCommand("SELECT * FROM device");

            cmd.Connection = conn;
            cmd.CommandType = System.Data.CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();

            //StringBuilder sMAC = new StringBuilder();
            //StringBuilder sIP = new StringBuilder();

            //int bufferSize = 0;
            while (dr.Read())
            {
                // Add read values to hashtable, MAC is key, IP is value
                AddToHashTable(dr.GetValue(4).ToString(), dr.GetValue(1).ToString());
            }

            conn.Close();
        }
    }
}
