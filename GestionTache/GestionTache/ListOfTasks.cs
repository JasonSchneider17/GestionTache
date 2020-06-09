using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionTache
{
    public class ListOfTasks
    {
        private string name;
        private List<Task> tasks;


        public ListOfTasks(string name)
        {
            this.name = name;
        }


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
    }
}
