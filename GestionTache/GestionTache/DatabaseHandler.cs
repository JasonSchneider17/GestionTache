using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionTache
{
    public class DatabaseHandler
    {
        private Database databaseObject;
        private TaskDAO taskDAO;


        public static readonly String TASK_KEY= "id_task";
        public static readonly String TASK_NAME = "name_task";
        public static readonly String TASK_COMMENT = "comment_task";
        public static readonly String TASK_STATE = "state_task";

        public static readonly String TASK_TABLE_NAME = "Task";
        public static readonly String TASK_TABLE_CREATE =
                "CREATE TABLE " + TASK_TABLE_NAME + " (" +
                        TASK_KEY + " INTEGER PRIMARY KEY AUTOINCREMENT, " +
                        TASK_NAME + " TEXT, " +
                        TASK_COMMENT + " TEXT, " +
                        TASK_STATE + " INTEGER )";
        public static readonly String TASK_TABLE_DROP = "DROP TABLE IF EXISTS " + TASK_TABLE_NAME + ";";


        public DatabaseHandler(Database database)
        {
            this.databaseObject = database;
            DAOCreator();

        }

        public void DAOCreator()
        {
            taskDAO = new TaskDAO(databaseObject);
        }

        public void CreateTables()
        {
            databaseObject.OpenConnection();
            SQLiteCommand myCommand = new SQLiteCommand(TASK_TABLE_CREATE, databaseObject.myConnection);
            myCommand.ExecuteNonQuery();
            databaseObject.CloseConnection();
        }

        public TaskDAO TaskDAO
        {
            get { return this.taskDAO; }
            // set { _myProperty = value; }
        }


    }
}
