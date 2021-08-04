using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data.SqlClient;
using System.Collections;
using Server.DataSetTableAdapters;

namespace Server
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        public string GetData(string value)
        {
            return string.Format("Hello {0}", value);
        }

        public string GetDate()
        {
            return string.Format("{0:HH:mm:ss tt}", DateTime.Now);
        }


        /// <summary>
        /// Return Vector of the SType for the company
        /// </summary>
        /// <param name="comp"></param> Company's name 
        /// <returns></returns> Dictionary<string, int>
        public Dictionary<string, int> GetCompagnyActive(string comp)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            //string ConnectionString = @"Data Source=MSEDGEWIN10\SQLEXPRESS;Initial Catalog=CyberglobesApp;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            //SqlConnection conn;
            try
            {
                SocialUsersTableAdapter socialUsersTableAdapter = new SocialUsersTableAdapter();
                DataSet.SocialUsersDataTable socialUsersTable = socialUsersTableAdapter.GetDataByCompany(comp);

                if (socialUsersTable != null && socialUsersTable.Count > 0)
                {
                    foreach (var row in socialUsersTable)
                    {
                        
                        if (dict.ContainsKey(row.SType))
                        {
                            dict[row.SType] += row.Active;
                        }
                        else
                        {
                            dict.Add(row.SType,row.Active);
                        }
                    }
                }

                /**conn = new SqlConnection(ConnectionString);
                conn.Open();
                string SqlCommand = "SELECT * FROM SocialUsers WHERE Company = @zip";
                SqlCommand cmd = new SqlCommand(SqlCommand, conn);
                cmd.Parameters.AddWithValue("@zip", comp);
                using (SqlDataReader READER = cmd.ExecuteReader())
                {
                    int i = 0;
                    while (READER.Read())
                    {
                        string key = READER["SType"].ToString();
                        if (dict.ContainsKey(key))
                        {
                            dict[key] += int.Parse(READER["Active"].ToString());
                        }
                        else
                        {
                            dict.Add(key, int.Parse(READER["Active"].ToString()));
                        }
                        i++;
                    }
                }**/
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            if (dict.Count != 9)
            {
                FillMissingSType(dict);
            }
            return dict;
        }

        /// <summary>
        /// Check if dict contains all the 9 Stypes, otherwise fill the missing Stype with value 0
        /// </summary>
        /// <param name="dict"></param> dict
        private void FillMissingSType(Dictionary<string, int> dict)
        {
            for (int i = 1; i < 10; i++)
            {
                if (! dict.ContainsKey(("P" + i.ToString())))
                {
                    dict.Add("P" + i.ToString(), 0);
                }
            }
        }
    }
}
