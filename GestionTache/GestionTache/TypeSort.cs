using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionTache
{
   public class TypeSort
    {
        private string name;    //Nom du type de trie
        private int id;         //id du type de trie
        
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public TypeSort(string name,int id)
        {
            this.name = name;
            this.id = id;

        }

        //getter setter

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
