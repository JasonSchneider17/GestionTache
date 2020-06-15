using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionTache
{
   public class PriorityDAO
    {
        private Database database; //base de données

       
        public PriorityDAO(Database database)
        {
            this.database = database;
        }



        public void AddDefaultPriority()
        {
            Priority priorityLOW = new Priority("LOW",1);
            Priority priorityMEDIUM = new Priority("MEDIUM", 2);
            Priority priorityHIGH = new Priority("HIGH",3);

            Priority[] tablePriority = new Priority[] { priorityLOW, priorityMEDIUM, priorityHIGH };
            database.OpenConnection();
            foreach (Priority priority in tablePriority) {
                string query = String.Format("INSERT INTO {0} ( {1} , {2} ) VALUES ( @name , @degree )", 
                    DatabaseBuild.PRIORITY_TABLE_NAME, DatabaseBuild.PRIORITY_NAME,DatabaseBuild.PRIORITY_DEGREE);

                SQLiteCommand myCommand = new SQLiteCommand(query, database.myConnection);
                
                myCommand.Parameters.AddWithValue("@name", priority.Name);
                myCommand.Parameters.AddWithValue("@degree", priority.DegreePriority);

                myCommand.ExecuteNonQuery();
                
            }

            database.CloseConnection();
        }

        /// <summary>
        /// obtient toutes les priorité présentent sur la base de donnée
        /// </summary>
        /// <returns></returns>
        public List<Priority> getAllPriority()
        {

            string query = "SELECT * FROM " + DatabaseBuild.PRIORITY_TABLE_NAME;
            List<Priority> priorities  = new List<Priority>();
            SQLiteCommand myCommand = new SQLiteCommand(query, database.myConnection);
            database.OpenConnection();
            SQLiteDataReader result = myCommand.ExecuteReader();
            if (result.HasRows)
            {
                while (result.Read())
                {
                    //listClient.Add(result[CLIENT_NAME].ToString());
                    priorities.Add(new Priority(result[DatabaseBuild.PRIORITY_NAME].ToString(), 
                        Int32.Parse(result[DatabaseBuild.PRIORITY_DEGREE].ToString()),
                        Int32.Parse(result[DatabaseBuild.PRIORITY_KEY].ToString())));
                }
            }
            database.CloseConnection();



            return priorities;
        }
    }
}
