using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionTache
{
    public class ListDAO
    {
        private Database database; //base de données


        public static readonly String LIST_KEY = "id_list";
        public static readonly String LIST_NAME = "name_list";
        public static readonly String LIST_TABLE_NAME = "List";
        public static readonly String LIST_TABLE_CREATE =
                "CREATE TABLE " + LIST_TABLE_NAME + " (" +
                        LIST_KEY + " INTEGER PRIMARY KEY AUTOINCREMENT, " +
                        LIST_NAME + " TEXT, )";
        public static readonly String LIST_TABLE_DROP = "DROP TABLE IF EXISTS " + LIST_TABLE_NAME + ";";


        /// <summary>
        /// constructeur
        /// </summary>
        /// <param name="database"></param>
        public ListDAO(Database database)
        {
            this.database = database;
        }

        /// <summary>
        /// Ajout d'une liste sur la base de donnée
        /// </summary>
        /// <param name="list"></param>
        public void Add(ListOfTasks list)
        {
            string query = String.Format("INSERT INTO {0} ( {1} ) VALUES (@name)", LIST_TABLE_NAME, LIST_NAME);

            SQLiteCommand myCommand = new SQLiteCommand(query, database.myConnection);
            database.OpenConnection();
            myCommand.Parameters.AddWithValue("@name", list.Name);

            myCommand.ExecuteNonQuery();
            database.CloseConnection();
        }

        /// <summary>
        /// obtient toutes les liste présentent sur la base de donnée
        /// </summary>
        /// <returns></returns>
        public List<ListOfTasks> getAllList()
        {

            string query = "SELECT * FROM " + LIST_TABLE_NAME;
            List<ListOfTasks> lists= new List<ListOfTasks>();
            SQLiteCommand myCommand = new SQLiteCommand(query, database.myConnection);
            database.OpenConnection();
            SQLiteDataReader result = myCommand.ExecuteReader();
            if (result.HasRows)
            {
                while (result.Read())
                {
                    //listClient.Add(result[CLIENT_NAME].ToString());
                    lists.Add(new ListOfTasks(result[LIST_NAME].ToString(),Int32.Parse(result[LIST_KEY].ToString())));
                }
            }
            database.CloseConnection();



            return lists;
        }
    }
}
