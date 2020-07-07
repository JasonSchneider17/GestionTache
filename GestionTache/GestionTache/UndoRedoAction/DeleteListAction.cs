using GuiLabs.Undo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionTache
{
    class DeleteListAction : AbstractAction
    {

        ListOfTasks deletedList;
        ObservableCollection<ListOfTasks> lists;
        DatabaseHandler databaseHandler;
        int indexList;
        public DeleteListAction(ObservableCollection<ListOfTasks> lists,int indexList,DatabaseHandler databaseHandler)
        {
            this.lists = lists;
            this.indexList = indexList;
            deletedList = lists[indexList];
            
            this.databaseHandler = databaseHandler;
        }

        protected override void ExecuteCore()
        {
            databaseHandler.ListDAO.DeleteList(deletedList);
            lists.RemoveAt(indexList);
        }

        protected override void UnExecuteCore()
        {
           //faire un add qui met la liste met pas avec un nouveau id il faut conserver le même
            databaseHandler.ListDAO.Add(deletedList);
            lists.Insert(indexList,deletedList);
        }
    }
}
