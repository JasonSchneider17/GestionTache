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

        public SQLiteConnection myConnection;
        private bool fileCreated = false;
        private static readonly string fileDBname="managementTaskDataBase.db";
        private string pathDB;

        public Database()
        {
            pathDB = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), fileDBname);

            myConnection = new SQLiteConnection("Data Source="+pathDB);

            if (!File.Exists(pathDB))
            {
                SQLiteConnection.CreateFile(pathDB);
                System.Console.WriteLine("Database file created");
                fileCreated = true;
            }




        }

        public void OpenConnection()
        {
            if (myConnection.State != System.Data.ConnectionState.Open)
            {
                myConnection.Open();
            }
        }

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
