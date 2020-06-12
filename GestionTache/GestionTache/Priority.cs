using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionTache
{
    public class Priority
    {
        private string name;
        private int degreePriority;
        private int idPriority;


        public Priority()
        {


        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }


        public int DegreePriority
        {
            get { return this.degreePriority; }
            set { this.degreePriority = value; }
        }
    }
}
