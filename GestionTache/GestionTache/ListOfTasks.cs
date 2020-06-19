using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionTache
{
    public class ListOfTasks
    {
        private string name;            //nom de la liste
        private List<Task> tasks;       //liste de tâches
        private int id;                 //ide de la liste sur la base de donnée
        private int numberTaskToDo;     //nombre de tache devant encore être réaliser

        /// <summary>
        /// constructeur
        /// </summary>
        /// <param name="name"></param>
        public ListOfTasks(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// constructeur
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public ListOfTasks(string name,int id)
        {
            this.name = name;
            this.id = id;
        }

        //getter setter


        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public List<Task> Tasks
        {
            get { return this.tasks; }
            set { this.tasks = value; }
        }

        public int ID
        {
            get { return this.id; }
            set { this.id = value; }
        }

        public int NumberTaskToDo
        {
            get { return this.numberTaskToDo; }
            set { this.numberTaskToDo = value; }
        }


    }
}
