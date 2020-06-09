using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GestionTache
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Database databaseObject;
        DatabaseHandler databaseHandler;
        public MainWindow()
        {
            InitializeComponent();

            List<Task> tasks = new List<Task>();

            /*tasks.Add(new Task("test1", "No comment", true));
            tasks.Add(new Task("test2", "No comment", false));

            DisplayListTask.ItemsSource = tasks;*/

            databaseObject = new Database();
            databaseHandler = new DatabaseHandler(databaseObject);

            if (databaseObject.FileCreated)
            {
                databaseHandler.CreateTables();
            }


            /*databaseHandler.TaskDAO.Add(new Task("truc", "adwad", true));
            databaseHandler.TaskDAO.Add(new Task("truc2", "adwad", true));*/

            tasks = databaseHandler.TaskDAO.getAllTask();
            DisplayListTask.ItemsSource = tasks;


        }
    }
}
