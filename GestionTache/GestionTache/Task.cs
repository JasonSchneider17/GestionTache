using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionTache
{
   public class Task
    {
        private string name;
        private string comment;
        private Priority priority;


        public Task()
        {

        }

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


    }
}
