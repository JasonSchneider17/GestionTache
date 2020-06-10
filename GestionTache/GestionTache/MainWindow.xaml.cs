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

        Database databaseObject;            //base de données
        DatabaseHandler databaseHandler;    //interagir avec base de données
        List<ListOfTasks> lists;            //liste
        bool listIsEdited = false;          //indique si on est en mode d'edition de tâche
        ListOfTasks selectedList;
        public MainWindow()
        {
            InitializeComponent();
            SetLanguageDictionary();

            List<Task> tasks = new List<Task>();

            /*tasks.Add(new Task("test1", "No comment", true));
            tasks.Add(new Task("test2", "No comment", false));

            DisplayListTask.ItemsSource = tasks;*/
            //database
            databaseObject = new Database();
            databaseHandler = new DatabaseHandler(databaseObject);
            //vérifie si un fichier de base de donnée en crée un sinon
            if (databaseObject.FileCreated)
            {
                databaseHandler.CreateTables();
            }

            //regarde la version de la base donné et réinitilise pour la mettre à jour
            Properties.Settings.Default.DatabaseVersionNew = 3;

            if (Properties.Settings.Default.DatabaseVersionNew > Properties.Settings.Default.DatabaseVersionOld)
            {
                databaseHandler.RemoveTables();
                databaseHandler.CreateTables();
                Properties.Settings.Default.DatabaseVersionOld = Properties.Settings.Default.DatabaseVersionNew;
                Properties.Settings.Default.Save();
            }


            //charge toute les liste présent sur la base de donnée
            lists = databaseHandler.ListDAO.getAllList();
            //attribue la liste à la listbox
            listBox_listOfTasks.ItemsSource = lists;




        }

        /// <summary>
        /// détermine le language de l'appli
        /// </summary>
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


        /// <summary>
        /// Ajout d'une liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_AddList_Click(object sender, RoutedEventArgs e)
        {
            listIsEdited = true;
            lists.Add(new ListOfTasks("add"));
            listBox_listOfTasks.Items.Refresh();

            listBox_listOfTasks.SelectedItem = listBox_listOfTasks.Items[listBox_listOfTasks.Items.Count - 1];
            listBox_listOfTasks.UpdateLayout();

            ListBoxItem listBoxItem = (ListBoxItem)listBox_listOfTasks.ItemContainerGenerator.ContainerFromItem(listBox_listOfTasks.SelectedItem);

            TextBox target = getTextBoxFromLisboxItem("txtBoxNameList", listBoxItem);
            TextBlock textblockList = getTextBlockFromLisboxItem("txtBlockNameList", listBoxItem);
            textblockList.Visibility = Visibility.Hidden;
            target.Visibility = Visibility.Visible;
            target.Focus();
            target.SelectAll();
            




        }

        /// <summary>
        /// ajout d'une tâche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_AddTask_Click(object sender, RoutedEventArgs e)
        {
            selectedList.Tasks.Add(new Task("test", "comment", true));
            ListBox_Tasks.Items.Refresh();
            
        }

  

        /// <summary>
        /// si le focus de la textbox pour le nom de la liste est quitter ajoute cette liste dans la base de données
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBoxNameList_LostFocus(object sender, RoutedEventArgs e)
        {
            ListBoxItem listBoxItem = (ListBoxItem)listBox_listOfTasks.ItemContainerGenerator.ContainerFromItem(listBox_listOfTasks.SelectedItem);

            TextBox target = getTextBoxFromLisboxItem("txtBoxNameList", listBoxItem);
            TextBlock textblockList = getTextBlockFromLisboxItem("txtBlockNameList", listBoxItem);
            textblockList.Visibility = Visibility.Visible;
            target.Visibility = Visibility.Hidden;

            databaseHandler.ListDAO.Add(lists[lists.Count-1]);
            listIsEdited = false;
         

        }


        /// <summary>
        /// permet d'obtenir un contrôle textbox contenue dans un listboxitem
        /// </summary>
        /// <param name="nameTextBox"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private TextBox getTextBoxFromLisboxItem(string nameTextBox,ListBoxItem item)
        {
            ContentPresenter contentPresenter = FindVisualChild<ContentPresenter>(item);
            DataTemplate dataTemplate = contentPresenter.ContentTemplate;
            TextBox target = (TextBox)dataTemplate.FindName(nameTextBox, contentPresenter);

            return target;
        }

        /// <summary>
        /// permet d'obtenir un contrôle textblock contenue dans un lisboxitem
        /// </summary>
        /// <param name="nameTextBlock"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private TextBlock getTextBlockFromLisboxItem(string nameTextBlock, ListBoxItem item)
        {
            ContentPresenter contentPresenter = FindVisualChild<ContentPresenter>(item);
            DataTemplate dataTemplate = contentPresenter.ContentTemplate;
            TextBlock target = (TextBlock)dataTemplate.FindName(nameTextBlock, contentPresenter);

            return target;
        }


        private childItem FindVisualChild<childItem>(DependencyObject obj)
    where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }


        /// <summary>
        /// Affiche les tâches de la liste séléctionner
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox_listOfTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!listIsEdited)
            {

                ListBoxItem itemSelected = (ListBoxItem)listBox_listOfTasks.ItemContainerGenerator.ContainerFromItem(listBox_listOfTasks.SelectedItem);

                selectedList = (ListOfTasks)itemSelected.DataContext;

                txtBox_TitleList.Text = selectedList.Name;

                selectedList.Tasks = new List<Task>();

                ListBox_Tasks.ItemsSource = selectedList.Tasks;


            }
  

        }


    }
}
