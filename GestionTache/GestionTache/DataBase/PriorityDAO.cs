using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionTache
{
   public class PriorityDAO
    {
        private Database database; //base de données

       
        public PriorityDAO(Database database)
        {
            this.database = database;
        }

    }
}
