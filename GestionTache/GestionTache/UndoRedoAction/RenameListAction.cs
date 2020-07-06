using GuiLabs.Undo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionTache
{
    class RenameListAction : AbstractAction
    {
        DatabaseHandler databaseHandler;
        ListOfTasks listUpdated;
        string oldName;
        string newName;

        public RenameListAction(DatabaseHandler databaseHandler,ListOfTasks list,string newName)
        {
            this.databaseHandler = databaseHandler;
            this.listUpdated = list;
            this.oldName = this.listUpdated.Name;
            this.newName = newName;
        }

        protected override void ExecuteCore()
        {
            listUpdated.Name = newName;
            databaseHandler.ListDAO.UpdateListName(listUpdated);

        }

        protected override void UnExecuteCore()
        {
            listUpdated.Name = oldName;
            databaseHandler.ListDAO.UpdateListName(listUpdated);
        }
    }
}
