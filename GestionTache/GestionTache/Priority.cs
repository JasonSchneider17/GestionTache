using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionTache
{
    public class Priority
    {
        private string name;            //nom de la priorité
        private int degreePriority;     //degrée indiquant le niveau de priorité
        private int idPriority;         //id de la priorité sur la base de donné

        /// <summary>
        /// constructeur
        /// </summary>
        /// <param name="name">nom de la priorité</param>
        /// <param name="degreePriority">degré de la priorité</param>
        public Priority(string name,int degreePriority)
        {
            this.name = name;
            this.degreePriority = degreePriority;

        }
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="name">nom de la priorité<</param>
        /// <param name="degreePriority">degré de la priorité</param>
        /// <param name="idPriority">id priorité</param>
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



        //getter setter
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
