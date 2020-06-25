using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        /// <param name="task">Tâche à ajouter</param>
        public void Add(Task task)
        {
            string query = String.Format("INSERT INTO {0} ( {1} , {2} , {3}, {4}, {5} ) VALUES (@name,@comment,@state,@listID, @priorityID)", 
                DatabaseBuild.TASK_TABLE_NAME, DatabaseBuild.TASK_NAME, DatabaseBuild.TASK_COMMENT, 
                DatabaseBuild.TASK_STATE, DatabaseBuild.TASK_ID_LIST,DatabaseBuild.TASK_ID_PRIORITY);

            SQLiteCommand myCommand = new SQLiteCommand(query, database.myConnection);
            database.OpenConnection();
            myCommand.Parameters.AddWithValue("@name", task.Name);
            myCommand.Parameters.AddWithValue("@comment", task.Comment);
            myCommand.Parameters.AddWithValue("@state", task.State);
            myCommand.Parameters.AddWithValue("@listID", task.ListID);
            myCommand.Parameters.AddWithValue("@priorityID", task.Priority.IDPriority);



            myCommand.ExecuteNonQuery();
            database.CloseConnection();
        }

        /// <summary>
        /// Change la priorité de la tâche
        /// </summary>
        /// <param name="task">Tâche contenant la nouvelle prioritée</param>
        public void UpdateTaskPriorityId(Task task)
        {
            string query = String.Format("UPDATE {0} SET {1} = @priorityID WHERE {2} = @taskID ", DatabaseBuild.TASK_TABLE_NAME, DatabaseBuild.TASK_ID_PRIORITY,DatabaseBuild.TASK_KEY);
            SQLiteCommand myCommand = new SQLiteCommand(query, database.myConnection);
            database.OpenConnection();
            myCommand.Parameters.AddWithValue("@priorityID", task.Priority.IDPriority);
            myCommand.Parameters.AddWithValue("@taskID", task.IDTask);

            myCommand.ExecuteNonQuery();
            database.CloseConnection();
        }


        /// <summary>
        /// Change le nom de la tâche
        /// </summary>
        /// <param name="task">Tâche contenant le nouveau nom</param>
        public void UpdateTaskName(Task task)
        {
            string query = String.Format("UPDATE {0} SET {1} = @nameTask WHERE {2} = @taskID ", DatabaseBuild.TASK_TABLE_NAME, DatabaseBuild.TASK_NAME, DatabaseBuild.TASK_KEY);
            SQLiteCommand myCommand = new SQLiteCommand(query, database.myConnection);
            database.OpenConnection();
            myCommand.Parameters.AddWithValue("nameTask", task.Name);
            myCommand.Parameters.AddWithValue("@taskID", task.IDTask);

            myCommand.ExecuteNonQuery();
            database.CloseConnection();
        }

        /// <summary>
        /// Change l'état de la tâche
        /// </summary>
        /// <param name="task">Tâche contenant le nouvel état</param>
        public void UpdateTaskState(Task task)
        {
            string query = String.Format("UPDATE {0} SET {1} = @taskState WHERE {2} = @taskID ", DatabaseBuild.TASK_TABLE_NAME, DatabaseBuild.TASK_STATE, DatabaseBuild.TASK_KEY);
            SQLiteCommand myCommand = new SQLiteCommand(query, database.myConnection);
            database.OpenConnection();


            myCommand.Parameters.AddWithValue("@taskState", task.State);
            myCommand.Parameters.AddWithValue("@taskID", task.IDTask);

            myCommand.ExecuteNonQuery();
            database.CloseConnection();
        }

        /// <summary>
        /// Change le commentaire de la tâche
        /// </summary>
        /// <param name="task">Tâche contenant le nouveau commentaire</param>
        public void UpdateTaskComment(Task task)
        {
            string query = String.Format("UPDATE {0} SET {1} = @taskComment WHERE {2} = @taskID ", DatabaseBuild.TASK_TABLE_NAME, DatabaseBuild.TASK_COMMENT, DatabaseBuild.TASK_KEY);
            SQLiteCommand myCommand = new SQLiteCommand(query, database.myConnection);
            database.OpenConnection();


            myCommand.Parameters.AddWithValue("@taskComment", task.Comment);
            myCommand.Parameters.AddWithValue("@taskID", task.IDTask);

            myCommand.ExecuteNonQuery();
            database.CloseConnection();
        }

        /// <summary>
        /// Supprime la tâche
        /// </summary>
        /// <param name="task">Tâche à supprimer</param>
        public void DeletedTask(Task task)
        {
            string query = String.Format("DELETE FROM {0} WHERE {1} = @taskID ", DatabaseBuild.TASK_TABLE_NAME,DatabaseBuild.TASK_KEY);
            SQLiteCommand myCommand = new SQLiteCommand(query, database.myConnection);
            database.OpenConnection();
            myCommand.Parameters.AddWithValue("@taskID", task.IDTask);
            myCommand.ExecuteNonQuery();
            database.CloseConnection();
        }

        public void DeletedTaksByListID(int listID)
        {
            string query = String.Format("DELETE FROM {0} WHERE {1} = @listID ", DatabaseBuild.TASK_TABLE_NAME, DatabaseBuild.TASK_ID_LIST);
            SQLiteCommand myCommand = new SQLiteCommand(query, database.myConnection);
            database.OpenConnection();
            myCommand.Parameters.AddWithValue("@listID", listID);
            myCommand.ExecuteNonQuery();
            database.CloseConnection();
        }


        /// <summary>
        /// Obtient toutes les tâche sur la base de donnée
        /// </summary>
        /// <returns> liste de tâche</returns>
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




        /// <summary>
        /// Obtient toute les tâche se lon l'id de la liste
        /// </summary>
        /// <param name="listID"> id de la liste </param>
        /// <param name="priorities">priorité de la tâche (ne sert que pour l'affichage)</param>
        /// <returns>liste de tâches</returns>
        public ObservableCollection<Task> getAllTaskByListID(int listID,List<Priority> priorities)
        {

            /*typeSorts.Add(new TypeSort("Aucun", 0));
            typeSorts.Add(new TypeSort("Priorité ASC", 1));
            typeSorts.Add(new TypeSort("Priorité DESC", 2));
            typeSorts.Add(new TypeSort("Réaliser ASC", 3));
            typeSorts.Add(new TypeSort("Réaliser ASC", 4));*/
            string query = string.Format("SELECT * FROM {0} WHERE {1} = {2} ", DatabaseBuild.TASK_TABLE_NAME, DatabaseBuild.TASK_ID_LIST, listID);

            // List<Task> listTask = new List<Task>();
            ObservableCollection<Task> listTask = new ObservableCollection<Task>();
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
                    int idPriority = int.Parse(result[DatabaseBuild.TASK_ID_PRIORITY].ToString());
                    int idTask = int.Parse(result[DatabaseBuild.TASK_KEY].ToString());
                    List<Priority> priorities1 = new List<Priority>();
                    foreach ( Priority priority in priorities)
                    {
                       
                        priorities1.Add(priority);
                        

                    }


                    listTask.Add(new Task(name, comment, state,idTask,idList,priorities1,idPriority));
                }
            }
            database.CloseConnection();

            return listTask;

        }





        /// <summary>
        /// Obtient l'id de la tâche dernièrement ajouté
        /// </summary>
        /// <returns> id tâche</returns>
        public int getLastAddedTaskID()
        {
            string query = string.Format("SELECT * FROM {0} ORDER BY {1} DESC LIMIT 1", DatabaseBuild.TASK_TABLE_NAME, DatabaseBuild.TASK_KEY);
            int id = -1;
            SQLiteCommand myCommand = new SQLiteCommand(query, database.myConnection);
            database.OpenConnection();
            SQLiteDataReader result = myCommand.ExecuteReader();
            if (result.HasRows)
            {
                while (result.Read())
                {
                    id = int.Parse(result[DatabaseBuild.TASK_KEY].ToString());
                }
            }
            database.CloseConnection();
            return id;
        }


        public int CountTaskToDoByList(int idList)
        {
            string query = string.Format("SELECT COUNT(*) TotalTasks FROM {0} WHERE {1} = {2} AND {3} = 0 ",DatabaseBuild.TASK_TABLE_NAME,DatabaseBuild.TASK_ID_LIST,idList,DatabaseBuild.TASK_STATE);
            int numberTask = -1;
            SQLiteCommand myCommand = new SQLiteCommand(query, database.myConnection);
            database.OpenConnection();
            SQLiteDataReader result = myCommand.ExecuteReader();

           
            if (result.HasRows)
            {
                while (result.Read())
                {
                    numberTask = int.Parse(result["TotalTasks"].ToString());
                }
            }

            database.CloseConnection();

            return numberTask;


        }






    }
}
