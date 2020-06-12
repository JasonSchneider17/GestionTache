using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionTache
{
    public class TaskDAO
    {
        private Database database;  //base de données


        /// <summary>
        /// constructeur
        /// </summary>
        /// <param name="database"></param>
        public TaskDAO(Database database)
        {
            this.database = database;
        }



        /// <summary>
        /// ajout d'une tâche sur la base de donnée
        /// </summary>
        /// <param name="task"></param>
        public void Add(Task task)
        {
            string query = String.Format("INSERT INTO {0} ( {1} , {2} , {3}, {4} ) VALUES (@name,@comment,@state,@listID)", 
                DatabaseBuild.TASK_TABLE_NAME, DatabaseBuild.TASK_NAME, DatabaseBuild.TASK_COMMENT, 
                DatabaseBuild.TASK_STATE, DatabaseBuild.TASK_ID_LIST);

            SQLiteCommand myCommand = new SQLiteCommand(query, database.myConnection);
            database.OpenConnection();
            myCommand.Parameters.AddWithValue("@name", task.Name);
            myCommand.Parameters.AddWithValue("@comment", task.Comment);
            myCommand.Parameters.AddWithValue("@state", task.State);
            myCommand.Parameters.AddWithValue("@listID", task.ListID);


            myCommand.ExecuteNonQuery();
            database.CloseConnection();
        }


        /// <summary>
        /// Btient toutes les tâche sur la base de donnée
        /// </summary>
        /// <returns></returns>
        public List<Task> getAllTask()
        {

            string query = "SELECT * FROM "+ DatabaseBuild.TASK_TABLE_NAME;
            List<Task> listTask = new List<Task>();
            SQLiteCommand myCommand = new SQLiteCommand(query, database.myConnection);
            database.OpenConnection();
            SQLiteDataReader result = myCommand.ExecuteReader();
            if (result.HasRows)
            {
                while (result.Read())
                {
                    //listClient.Add(result[CLIENT_NAME].ToString());
                    listTask.Add(new Task(result[DatabaseBuild.TASK_NAME].ToString(),"test comment",true));
                }
            }
            database.CloseConnection();



            return listTask;
        }



        public List<Task> getAllTaskByListID(int listID)
        {
            string query = string.Format( "SELECT * FROM {0} WHERE {1} = {2} ", DatabaseBuild.TASK_TABLE_NAME, DatabaseBuild.TASK_ID_LIST,listID);
            List<Task> listTask = new List<Task>();
            SQLiteCommand myCommand = new SQLiteCommand(query, database.myConnection);
            database.OpenConnection();
            SQLiteDataReader result = myCommand.ExecuteReader();
            if (result.HasRows)
            {
                while (result.Read())
                {
                    string name = result[DatabaseBuild.TASK_NAME].ToString();
                    string comment = result[DatabaseBuild.TASK_COMMENT].ToString();
                    bool state= int.Parse(result[DatabaseBuild.TASK_STATE].ToString()) == 1;
                    int idList = int.Parse(result[DatabaseBuild.TASK_ID_LIST].ToString());

                    listTask.Add(new Task(name, comment, state,idList));
                }
            }
            database.CloseConnection();



            return listTask;

        }


    }
}
