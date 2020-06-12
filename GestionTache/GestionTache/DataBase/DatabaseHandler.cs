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
        private PriorityDAO priorityDAO;


        /*public static readonly String TASK_KEY= "id_task";
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


        public static readonly String PRIORITY_KEY = "id_priority";
        public static readonly String PRIORITY_NAME = "name_priority";
        public static readonly String PRIORITY_DEGREE = "degree_priority";
        public static readonly String PRIORITY_TABLE_NAME = "Priority";
        public static readonly String PRIORITY_TABLE_CREATE =
                "CREATE TABLE " + PRIORITY_TABLE_NAME + " (" +
                        PRIORITY_KEY + " INTEGER PRIMARY KEY AUTOINCREMENT, " +
                        PRIORITY_NAME + " TEXT, "+
                        PRIORITY_DEGREE+" INTEGER )";
        public static readonly String PRIORITY_TABLE_DROP = "DROP TABLE IF EXISTS " + PRIORITY_TABLE_NAME + ";";*/









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
            priorityDAO = new PriorityDAO(databaseObject);
           
           
        }

        /// <summary>
        /// Crée les tâbles de la base de données
        /// </summary>
        public void CreateTables()
        {
            databaseObject.OpenConnection();
            SQLiteCommand myCommand = new SQLiteCommand(DatabaseBuild.TASK_TABLE_CREATE, databaseObject.myConnection);
            SQLiteCommand myCommand2 = new SQLiteCommand(DatabaseBuild.LIST_TABLE_CREATE, databaseObject.myConnection);
            SQLiteCommand myCommand3 = new SQLiteCommand(DatabaseBuild.PRIORITY_TABLE_CREATE, databaseObject.myConnection);
            myCommand.ExecuteNonQuery();
            myCommand2.ExecuteNonQuery();
            myCommand3.ExecuteNonQuery();
            databaseObject.CloseConnection();
        }

        /// <summary>
        /// supprime les tables de la base de données
        /// </summary>
        public void RemoveTables()
        {
            databaseObject.OpenConnection();
            SQLiteCommand myCommand = new SQLiteCommand(DatabaseBuild.TASK_TABLE_DROP, databaseObject.myConnection);
            SQLiteCommand myCommand2 = new SQLiteCommand(DatabaseBuild.LIST_TABLE_DROP, databaseObject.myConnection);
            SQLiteCommand myCommand3 = new SQLiteCommand(DatabaseBuild.PRIORITY_TABLE_DROP, databaseObject.myConnection);
            myCommand.ExecuteNonQuery();
            myCommand2.ExecuteNonQuery();
            myCommand3.ExecuteNonQuery();
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
