using GuiLabs.Undo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionTache
{
    public class AddListAction : AbstractAction
    {
        ListOfTasks listAdded;
        DatabaseHandler databaseHandler;
        ObservableCollection<ListOfTasks> lists;

        public AddListAction(ListOfTasks listAdded,DatabaseHandler databaseHandler,ObservableCollection<ListOfTasks> lists)
        {
            this.listAdded = listAdded;
            this.databaseHandler = databaseHandler;
            this.lists = lists;
        }

        protected override void ExecuteCore()
        {

            databaseHandler.ListDAO.Add(listAdded);
            listAdded.ID = databaseHandler.ListDAO.getLastAddedListID();
            lists.Add(listAdded); 

        }

        protected override void UnExecuteCore()
        {
            databaseHandler.ListDAO.DeleteList(listAdded);

            for(int i = 0; i < lists.Count; i++)
            {
                if (lists[i].ID == listAdded.ID)
                {
                    lists.RemoveAt(i);
                }
            }
        }
    }
}
