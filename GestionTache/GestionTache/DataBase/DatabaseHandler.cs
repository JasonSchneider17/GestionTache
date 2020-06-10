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
        private ListDAO listDAO;


        public static readonly String TASK_KEY= "id_task";
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
                        TASK_STATE + " INTEGER,"+
                         TASK_ID_LIST+ " INTEGER ) ";
        public static readonly String TASK_TABLE_DROP = "DROP TABLE IF EXISTS " + TASK_TABLE_NAME + ";";


        public static readonly String LIST_KEY = "id_list";
        public static readonly String LIST_NAME = "name_list";
        public static readonly String LIST_TABLE_NAME = "List";
        public static readonly String LIST_TABLE_CREATE =
                "CREATE TABLE " + LIST_TABLE_NAME + " (" +
                        LIST_KEY + " INTEGER PRIMARY KEY AUTOINCREMENT, " +
                        LIST_NAME + " TEXT )";
        public static readonly String LIST_TABLE_DROP = "DROP TABLE IF EXISTS " + LIST_TABLE_NAME + ";";








        /// <summary>
        /// constructeur
        /// </summary>
        /// <param name="database"></param>
        public DatabaseHandler(Database database)
        {
           
            this.databaseObject = database;
            DAOCreator();

        }

        /// <summary>
        /// Crée les DAO permettant d'interagir avec les tables
        /// </summary>
        public void DAOCreator()
        {
            taskDAO = new TaskDAO(databaseObject);
            listDAO = new ListDAO(databaseObject);
           
        }

        /// <summary>
        /// Crée les tâbles de la base de données
        /// </summary>
        public void CreateTables()
        {
            databaseObject.OpenConnection();
            SQLiteCommand myCommand = new SQLiteCommand(TASK_TABLE_CREATE, databaseObject.myConnection);
            SQLiteCommand myCommand2 = new SQLiteCommand(LIST_TABLE_CREATE, databaseObject.myConnection);
            myCommand.ExecuteNonQuery();
            myCommand2.ExecuteNonQuery();
            databaseObject.CloseConnection();
        }

        /// <summary>
        /// supprime les tables de la base de données
        /// </summary>
        public void RemoveTables()
        {
            databaseObject.OpenConnection();
            SQLiteCommand myCommand = new SQLiteCommand(TASK_TABLE_DROP, databaseObject.myConnection);
            SQLiteCommand myCommand2 = new SQLiteCommand(LIST_TABLE_DROP, databaseObject.myConnection);
            myCommand.ExecuteNonQuery();
            myCommand2.ExecuteNonQuery();
            databaseObject.CloseConnection();
        }


        //getter setter

        public TaskDAO TaskDAO
        {
            get { return this.taskDAO; }
            // set { _myProperty = value; }
        }

        public ListDAO ListDAO
        {
            get { return this.listDAO; }
        }


    }
}
