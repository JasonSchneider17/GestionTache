using GuiLabs.Undo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionTache
{
    class AddTaskAction : AbstractAction
    {
        Task taskAdded;
        ObservableCollection<Task> tasks;

        public AddTaskAction(Task taskAdded, ObservableCollection<Task> tasks) {

            this.taskAdded = taskAdded;
            this.tasks = tasks;
        
        }

        protected override void ExecuteCore()
        {
            if(taskAdded.ListID != tasks[0].ListID)
            {
                System.Console.WriteLine("Wtf");

            }

            MyGlobals.databaseHandler.TaskDAO.Add(taskAdded);
            taskAdded.IDTask = MyGlobals.databaseHandler.TaskDAO.getLastAddedTaskID();
            tasks.Add(taskAdded);
        }

        protected override void UnExecuteCore()
        {
            
        }
    }
}
