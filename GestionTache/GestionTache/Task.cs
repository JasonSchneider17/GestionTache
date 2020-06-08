using System;
using System.Collections.Generic;
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

    }
}
