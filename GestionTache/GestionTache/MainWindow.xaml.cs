using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            SetLanguageDictionary();

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



            tasks = databaseHandler.TaskDAO.getAllTask();
            DisplayListTask.ItemsSource = tasks;




        }

        private void SetLanguageDictionary()
        {
            ResourceDictionary dict = new ResourceDictionary();
            switch (Thread.CurrentThread.CurrentCulture.ToString())
            {
                /*case "en-US":
                    dict.Source = new Uri("..\\..\\Language\\StringResourcesEN.xaml", UriKind.Relative);
                    break;*/
                case "fr-CH":
                    dict.Source = new Uri("..\\..\\Language\\StringResourcesFR.xaml", UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("..\\..\\Language\\StringResourcesFR.xaml", UriKind.Relative);
                    break;
            }
            this.Resources.MergedDictionaries.Add(dict);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

            databaseHandler.TaskDAO.Add(new Task("truc", "adwad", true));
            databaseHandler.TaskDAO.Add(new Task("truc2", "adwad", true));
        }
    }
}
