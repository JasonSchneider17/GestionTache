using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionTache
{
   public class Task
    {
        private string name;        //name of the task
        private string comment;     //comment of the task
        private Priority priority;  //priority task
        private bool state;         //state task
        private int listID;         //id de la liste affilié à la tache
        private int idTask;
        private List<Priority> prioritiesDisplay;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="comment"></param>
        /// <param name="state"></param>
        public Task(string name,string comment, bool state)
        {
            this.name = name;
            this.comment = comment;
            this.state = state;
        }
        public Task(string name, string comment, bool state,int listID)
        {
            this.name = name;
            this.comment = comment;
            this.state = state;
            this.listID = listID;
        }
        public Task(string name, string comment, bool state, int listID,List<Priority> priorities)
        {
            this.name = name;
            this.comment = comment;
            this.state = state;
            this.listID = listID;
            this.prioritiesDisplay = priorities;
        }
        public Task(string name, string comment, bool state,int idTask, int listID, List<Priority> priorities,int idPriority)
        {
            this.name = name;
            this.comment = comment;
            this.state = state;
            this.idTask = idTask;
            this.listID = listID;
            this.prioritiesDisplay = priorities;
            
            foreach(Priority priority in prioritiesDisplay)
            {
                if (priority.IDPriority == idPriority)
                {
                    this.priority = priority;
                }
            }

        }




        //getter setter

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public string Comment
        {
            get { return this.comment; }
            set { this.comment = value; }
        }

        public Priority Priority
        {
            get { return this.priority; }
            set { this.priority = value; }
        }

        public bool State
        {
            get { return this.state; }
            set { this.state = value; }
        }

        public int ListID
        {
            get { return this.listID; }
            set { this.listID = value; }
        }

        public List<Priority> PrioritiesDisplay
        {
            get { return this.prioritiesDisplay; }
            set { this.prioritiesDisplay = value; }
        }

        public int IDTask
        {
            get { return this.idTask; }
            set { this.idTask = value; }
        }
    }
}
