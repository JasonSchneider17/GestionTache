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

        public PriorityDAO PriorityDAO
        {
            get { return this.priorityDAO; }
        }

    }
}
