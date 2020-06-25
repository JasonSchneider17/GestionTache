using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionTache
{
    public class ListDAO
    {
        private Database database; //base de données


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
        /// <param name="list">liste à ajouté</param>
        public void Add(ListOfTasks list)
        {
            string query = String.Format("INSERT INTO {0} ( {1} ) VALUES (@name)", DatabaseBuild.LIST_TABLE_NAME, DatabaseBuild.LIST_NAME);

            SQLiteCommand myCommand = new SQLiteCommand(query, database.myConnection);
            database.OpenConnection();
            myCommand.Parameters.AddWithValue("@name", list.Name);

            myCommand.ExecuteNonQuery();
            database.CloseConnection();
        }

        /// <summary>
        /// Met à jour le nom de la lite sur la base de données
        /// </summary>
        /// <param name="list">liste à modifier</param>
        public void UpdateListName(ListOfTasks list)
        {
            string query = String.Format("UPDATE {0} SET {1} = @nameList WHERE {2} = @listID ", DatabaseBuild.LIST_TABLE_NAME, DatabaseBuild.LIST_NAME, DatabaseBuild.LIST_KEY);
            SQLiteCommand myCommand = new SQLiteCommand(query, database.myConnection);
            database.OpenConnection();
            myCommand.Parameters.AddWithValue("@nameList", list.Name);
            myCommand.Parameters.AddWithValue("@listID", list.ID);

            myCommand.ExecuteNonQuery();
            database.CloseConnection();
        }


        public void DeleteList(ListOfTasks list)
        {
            string query = String.Format("DELETE FROM {0}  WHERE {1} = @listID ", DatabaseBuild.LIST_TABLE_NAME, DatabaseBuild.LIST_KEY);
            SQLiteCommand myCommand = new SQLiteCommand(query, database.myConnection);
            database.OpenConnection();
            myCommand.Parameters.AddWithValue("@listID", list.ID);
            myCommand.ExecuteNonQuery();
            database.CloseConnection();
        }

        /// <summary>
        /// obtient toutes les liste présentent sur la base de donnée
        /// </summary>
        /// <returns> liste de liste</returns>
        public ObservableCollection<ListOfTasks> getAllList()
        {

            string query = string.Format("SELECT * FROM {0} ", DatabaseBuild.LIST_TABLE_NAME);
            /*string query = string.Format("SELECT COUNT(*) TotalCount , {2}.{0} , {2}.{1} TotalCount,* FROM {2} JOIN {3} ON {2}.{4} = {3}.{5} "
               ,DatabaseBuild.LIST_NAME,DatabaseBuild.LIST_KEY,DatabaseBuild.LIST_TABLE_NAME,DatabaseBuild.TASK_TABLE_NAME,DatabaseBuild.LIST_KEY,DatabaseBuild.TASK_ID_LIST);
            */
            ObservableCollection<ListOfTasks> lists= new ObservableCollection<ListOfTasks>();
            SQLiteCommand myCommand = new SQLiteCommand(query, database.myConnection);
            database.OpenConnection();
            SQLiteDataReader result = myCommand.ExecuteReader();
            if (result.HasRows)
            {
                while (result.Read())
                {
                    //listClient.Add(result[CLIENT_NAME].ToString());
                    //Console.WriteLine(result["TotalCount"]);
                    lists.Add(new ListOfTasks(result[DatabaseBuild.LIST_NAME].ToString(),Int32.Parse(result[DatabaseBuild.LIST_KEY].ToString())));
                }
            }
            database.CloseConnection();



            return lists;
        }

        

        /// <summary>
        /// Obtient l'id de la liste récemment ajouté 
        /// </summary>
        /// <returns>id liste</returns>
        public int getLastAddedListID()
        {
            string query = string.Format("SELECT * FROM {0} ORDER BY {1} DESC LIMIT 1", DatabaseBuild.LIST_TABLE_NAME, DatabaseBuild.LIST_KEY);
            int id = -1;
            SQLiteCommand myCommand = new SQLiteCommand(query, database.myConnection);
            database.OpenConnection();
            SQLiteDataReader result = myCommand.ExecuteReader();
            if (result.HasRows)
            {
                while (result.Read())
                {
                    id = int.Parse(result[DatabaseBuild.LIST_KEY].ToString());
                }
            }
            database.CloseConnection();
            return id;
        }
    }
}
