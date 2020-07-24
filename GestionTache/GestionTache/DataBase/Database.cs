using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionTache
{
    public class Database
    {

        public SQLiteConnection myConnection;                                       //Connexion base de données
        private bool fileCreated = false;                                           //indique si le fichier à été créé
        private static readonly string fileDBname="managementTaskDataBase.db";      //nom du fichier de la base de données
        private string pathDB;                                                      //Chemin qui permet d'accéder à la base de données
        
        public Database()
        {
            pathDB = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), fileDBname);
            MyGlobals.pathDatabase = pathDB;

            myConnection = new SQLiteConnection("Data Source="+pathDB);

            if (!File.Exists(pathDB))
            {
                SQLiteConnection.CreateFile(pathDB);
                System.Console.WriteLine("Database file created");
                fileCreated = true;
            }

        }

        public void CreateFile()
        {
            SQLiteConnection.CreateFile(pathDB);
            fileCreated = true;
        }

        /// <summary>
        /// Ouvre la connection à la base de donnée pour effectué des traitements
        /// </summary>
        public void OpenConnection()
        {
            if (myConnection.State != System.Data.ConnectionState.Open)
            {
                myConnection.Open();
            }
        }

        /// <summary>
        /// Ferme la base de données
        /// </summary>
        public void CloseConnection()
        {
            if (myConnection.State != System.Data.ConnectionState.Closed)
            {
                myConnection.Close();
            }
        }





        public bool FileCreated
        {
            get { return this.fileCreated; }
            // set { _myProperty = value; }
        }

    }
}
