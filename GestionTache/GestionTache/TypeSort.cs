using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionTache
{
   public class TypeSort
    {
        private string name;
        private int id;
        
        public TypeSort(string name,int id)
        {
            this.name = name;
            this.id = id;

        }

        public string Name
        {
            get { return this.name;}
            set { this.name = value; }
        }

        public int ID
        {
            get { return this.id; }
            set { this.id = value; }

        }


    }
}
