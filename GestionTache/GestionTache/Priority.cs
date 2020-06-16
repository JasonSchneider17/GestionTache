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


        public Priority(string name,int degreePriority)
        {
            this.name = name;
            this.degreePriority = degreePriority;

        }

        public Priority(string name, int degreePriority,int idPriority)
        {
            this.name = name;
            this.degreePriority = degreePriority;
            this.idPriority = idPriority;

        }

        public override string ToString()
        {
            return String.Format("{0}", Name);
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

        public int IDPriority
        {
            get { return this.idPriority; }
            set { this.idPriority = value; }
        }


    }
}
