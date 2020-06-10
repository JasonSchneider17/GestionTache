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


        public static readonly String TASK_KEY = "id_task";
        public static readonly String TASK_NAME = "name_task";
        public static readonly String TASK_COMMENT = "comment_task";
        public static readonly String TASK_STATE = "state_task";
        public static readonly String TASK_ID_LIST = "id_list";

        public static readonly String TASK_TABLE_NAME = "Task";
        public static readonly String TASK_TABLE_CREATE =
                "CREATE TABLE " + TASK_TABLE_NAME + " (" +
                        TASK_KEY + " INTEGER PRIMARY KEY AUTOINCREMENT, " +
                        TASK_NAME + " TEXT, " +
                        TASK_COMMENT + " TEXT, " +
                        TASK_STATE + " INTEGER," +
                         TASK_ID_LIST + "INTEGER   ) ";
        public static readonly String TASK_TABLE_DROP = "DROP TABLE IF EXISTS " + TASK_TABLE_NAME + ";";


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
            string query = String.Format("INSERT INTO {0} ( {1} , {2} , {3} ) VALUES (@name,@comment,@state)",TASK_TABLE_NAME, TASK_NAME,TASK_COMMENT,TASK_STATE);

            SQLiteCommand myCommand = new SQLiteCommand(query, database.myConnection);
            database.OpenConnection();
            myCommand.Parameters.AddWithValue("@name", task.Name);
            myCommand.Parameters.AddWithValue("@comment", task.Comment);
            myCommand.Parameters.AddWithValue("@state", 1);
            myCommand.ExecuteNonQuery();
            database.CloseConnection();
        }


        /// <summary>
        /// Btient toutes les tâche sur la base de donnée
        /// </summary>
        /// <returns></returns>
        public List<Task> getAllTask()
        {

            string query = "SELECT * FROM "+TASK_TABLE_NAME;
            List<Task> listTask = new List<Task>();
            SQLiteCommand myCommand = new SQLiteCommand(query, database.myConnection);
            database.OpenConnection();
            SQLiteDataReader result = myCommand.ExecuteReader();
            if (result.HasRows)
            {
                while (result.Read())
                {
                    //listClient.Add(result[CLIENT_NAME].ToString());
                    listTask.Add(new Task(result[TASK_NAME].ToString(),"test comment",true));
                }
            }
            database.CloseConnection();



            return listTask;
        }


    }
}
