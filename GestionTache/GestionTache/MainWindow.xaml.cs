using System;
using System.Collections.Generic;
using System.Data.Linq;
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
        ListOfTasks selectedList;           //liste sélectionner par l'utilisateur

        public MainWindow()
        {
            InitializeComponent();
            SetLanguageDictionary();

            //List<Task> tasks = new List<Task>();


            //database
            databaseObject = new Database();
            databaseHandler = new DatabaseHandler(databaseObject);
            //vérifie si un fichier de base de donnée en crée un sinon
            if (databaseObject.FileCreated)
            {
                databaseHandler.CreateTables();
                databaseHandler.PriorityDAO.AddDefaultPriority();
            }

            //regarde la version de la base donné et réinitilise pour la mettre à jour
            Properties.Settings.Default.DatabaseVersionNew = 4;

            if (Properties.Settings.Default.DatabaseVersionNew > Properties.Settings.Default.DatabaseVersionOld)
            {
                databaseHandler.RemoveTables();
                databaseHandler.CreateTables();
                Properties.Settings.Default.DatabaseVersionOld = Properties.Settings.Default.DatabaseVersionNew;
                Properties.Settings.Default.Save();
                databaseHandler.PriorityDAO.AddDefaultPriority();
            }


            //end database conf

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

            ListOfTasks list = new ListOfTasks("add");
            //ajoute la liste dans la base de donnée et récupère l'id attribué par la base de donnée à la liste ajouté
            databaseHandler.ListDAO.Add(list);
            list.ID = databaseHandler.ListDAO.getLastAddedListID();

            lists.Add(list);
            listBox_listOfTasks.Items.Refresh();

            //selectionne le listboxitem ajouté et focus la vue dessus
            listBox_listOfTasks.SelectedItem = listBox_listOfTasks.Items[listBox_listOfTasks.Items.Count - 1];
            listBox_listOfTasks.ScrollIntoView(listBox_listOfTasks.SelectedItem);
            listBox_listOfTasks.UpdateLayout();

            ListBoxItem listBoxItem = (ListBoxItem)listBox_listOfTasks.ItemContainerGenerator.ContainerFromIndex(listBox_listOfTasks.Items.Count - 1);
            //affiche la textbox pour éditer le titre de la liste
            TextBox target = getTextBoxFromLisboxItem("txtBoxNameList", listBoxItem);
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
            List<Priority> listpriority = databaseHandler.PriorityDAO.getAllPriority();
            //ajoute la tâche dans la base de donnée et récupère l'id attribué par la base de donnée à la tâche ajouté
            Task newTask = new Task("test", "comment", false, 0, selectedList.ID, listpriority, listpriority[0].IDPriority);
            databaseHandler.TaskDAO.Add(newTask);
            newTask.IDTask = databaseHandler.TaskDAO.getLastAddedTaskID();
            //ajoute la tâche à la liste de tâche actuellement affiché
            selectedList.Tasks.Add(newTask);
            ListBox_Tasks.Items.Refresh();

            //selectionne le listboxitem ajouté et focus la vue dessus
            ListBox_Tasks.SelectedItem = ListBox_Tasks.Items[ListBox_Tasks.Items.Count - 1];
            ListBox_Tasks.ScrollIntoView(ListBox_Tasks.SelectedItem);
            ListBox_Tasks.UpdateLayout();
          
            ListBoxItem listBoxItem = (ListBoxItem)ListBox_Tasks.ItemContainerGenerator.ContainerFromItem(ListBox_Tasks.SelectedItem);
           
            ContentPresenter contentPresenter = FindVisualChild<ContentPresenter>(listBoxItem);
            DataTemplate dataTemplate = contentPresenter.ContentTemplate;
            Border grid = (Border)dataTemplate.FindName("BorderGridItemTask", contentPresenter);
            grid.Background = GradientBackground();

            //affiche la textbox pour éditer le titre de la tâche
            TextBox target = getTextBoxFromLisboxItem("txtboxNameTask", listBoxItem);
            target.Visibility = Visibility.Visible;
            target.Focus();
            target.SelectAll();


        }


        private LinearGradientBrush GradientBackground()
        {
            LinearGradientBrush horizontalGradient = new LinearGradientBrush();
            horizontalGradient.StartPoint = new Point(0, 0.5);
            horizontalGradient.EndPoint = new Point(1, 0.5);
            Color color1 = (Color)ColorConverter.ConvertFromString("#36d1dc");
            Color color2 = (Color)ColorConverter.ConvertFromString("#5b86e5");
            horizontalGradient.GradientStops.Add(new GradientStop(color1, 0.0));
            horizontalGradient.GradientStops.Add(new GradientStop(color2, 1.0));
            return horizontalGradient;
        }





        /// <summary>
        /// permet d'obtenir un contrôle textbox contenue dans un listboxitem
        /// </summary>
        /// <param name="nameTextBox">nom de la textbox à chercher</param>
        /// <param name="item">ListboxItem parent de la textbox</param>
        /// <returns></returns>
        private TextBox getTextBoxFromLisboxItem(string nameTextBox, ListBoxItem item)
        {
            ContentPresenter contentPresenter = FindVisualChild<ContentPresenter>(item);
            DataTemplate dataTemplate = contentPresenter.ContentTemplate;
            TextBox target = (TextBox)dataTemplate.FindName(nameTextBox, contentPresenter);

            return target;
        }

        /// <summary>
        /// permet d'obtenir un contrôle textblock contenue dans un lisboxitem
        /// </summary>
        /// <param name="nameTextBlock"> nom du textblock à chercher</param>
        /// <param name="item">ListboxItem parent du textblock</param>
        /// <returns></returns>
        private TextBlock getTextBlockFromLisboxItem(string nameTextBlock, ListBoxItem item)
        {
            ContentPresenter contentPresenter = FindVisualChild<ContentPresenter>(item);
            DataTemplate dataTemplate = contentPresenter.ContentTemplate;
            TextBlock target = (TextBlock)dataTemplate.FindName(nameTextBlock, contentPresenter);

            return target;
        }

        /// <summary>
        /// permet d'obtenir un contrôle combobox contenue dans un lisboxitem
        /// </summary>
        /// <param name="nameComboBox">nom de la combobox à chercher</param>
        /// <param name="item">ListboxItem parent de la combobox </param>
        /// <returns></returns>
        private ComboBox GetComboBoxFromTaskBoxItem(string nameComboBox, ListBoxItem item)
        {
            ContentPresenter contentPresenter = FindVisualChild<ContentPresenter>(item);
            DataTemplate dataTemplate = contentPresenter.ContentTemplate;
            return (ComboBox)dataTemplate.FindName(nameComboBox, contentPresenter);
        }



        /// <summary>
        /// Recherche le contrôle enfant
        /// </summary>
        /// <typeparam name="childItem"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
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
                //récupère les infos de la liste séléctionner
                ListBoxItem itemSelected = (ListBoxItem)listBox_listOfTasks.ItemContainerGenerator.ContainerFromItem(listBox_listOfTasks.SelectedItem);
                selectedList = (ListOfTasks)itemSelected.DataContext;
                //change le titre de l'affichage des tâches
                txtBox_TitleList.Text = selectedList.Name;
                //ajoute les tâches affilié à la liste  depuis la base de données dans la liste
                selectedList.Tasks = databaseHandler.TaskDAO.getAllTaskByListID(selectedList.ID, databaseHandler.PriorityDAO.getAllPriority());
                //source de donnée de la listbox de tâche
                ListBox_Tasks.ItemsSource = selectedList.Tasks;


               

            }
        }

        /// <summary>
        /// Sauvegarde le nouveau nom de la liste dans la base de donnée et masque la textbox d'édition
        /// </summary>
        /// <param name="textBox"> textbox édité</param>
        private void ListNameEndEdition(TextBox textBox)
        {
            if (listIsEdited)
            {
                //masque la textbox pour n'afficher que le textblock
                textBox.Visibility = Visibility.Hidden;

                //recherche la liste correspondant à la textbox et la met à jour sur la base de données
                foreach (ListOfTasks list in lists)
                {
                    if (list.ID == int.Parse(textBox.Tag.ToString()))
                    {
                        databaseHandler.ListDAO.UpdateListName(list);
                        break;
                    }
                }

                listIsEdited = false;
            }
        }

        /// <summary>
        /// Sauvegarde le nouveau nom de la tâche dans la base de donnée et masque la textbox d'édition
        /// </summary>
        /// <param name="textBox"></param>
        private void TaskNameEndEdition(TextBox textBox)
        {
            //masque la textbox pour n'afficher que le textblock
            textBox.Visibility = Visibility.Hidden;

            //recherche la tâche correspondant à la textbox et la met à jour sur la base de données
            foreach (Task task in selectedList.Tasks)
            {
                if (task.IDTask == int.Parse(textBox.Tag.ToString()))
                {
                    databaseHandler.TaskDAO.UpdateTaskName(task);
                    break;
                }
            }

        }



        /// <summary>
        /// action effecté lors de la pression d'une touche sur la textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBoxNameList_KeyDown(object sender, KeyEventArgs e)
        {
            //met fin à l'édition de la textbox
            if (e.Key == Key.Enter)
            {
                TextBox textbox = e.Source as TextBox;

                ListNameEndEdition(textbox);
            }
        }

        /// <summary>
        /// si le focus de la textbox pour le nom de la liste est quitter ajoute cette liste dans la base de données
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBoxNameList_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textbox = e.Source as TextBox;
            ListNameEndEdition(textbox);
        }

        /// <summary>
        /// action effecté lors de la pression d'une touche sur la textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtboxNameTask_KeyDown(object sender, KeyEventArgs e)
        {
            //met fin à l'édition de la textbox
            if (e.Key == Key.Enter)
            {
                TextBox textBox = e.Source as TextBox;

                TaskNameEndEdition(textBox);
            }
        }

        /// <summary>
        /// action effecté lors de la perte de focus de la textboc
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtboxNameTask_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textbox = e.Source as TextBox;

            TaskNameEndEdition(textbox);
        }

        /// <summary>
        /// change la priorité de la tâche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbBoxTaskPriority_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ComboBox comboBox = e.Source as ComboBox;

            try
            {
                Priority idPriority = (Priority)e.AddedItems[0];

                //recherche la tâche correspondante
                foreach (Task task in selectedList.Tasks)
                {
                    if (task.IDTask == int.Parse(comboBox.Tag.ToString()))
                    {
                        //change la priorité de la tâche et la met à jour sur la base de données
                        foreach (Priority priority in task.PrioritiesDisplay)
                        {
                            if (priority.IDPriority == idPriority.IDPriority)
                            {
                                task.IdPriority = priority.IDPriority;
                                databaseHandler.TaskDAO.UpdateTaskPriorityId(task);

                            }
                        }
                    }
                }

            }
            catch (IndexOutOfRangeException)
            {

            }

        }



        /// <summary>
        /// Barre le titre de la tâche lorsqu'elle est effectué
        /// </summary>
        private void SetCrossOutTitle()
        {
            ListBoxItem item;

            //parcourt les listboxitem de Lisbox_Task pour retrouver le textblock des tâches effectué
            foreach(object objet in ListBox_Tasks.Items)
            {
                item = (ListBoxItem)ListBox_Tasks.ItemContainerGenerator.ContainerFromItem(objet);

                if (item != null)
                {
                    foreach (Task task in selectedList.Tasks)
                    {
                        if (task.IDTask == int.Parse(item.Tag.ToString()))
                        {
                            if (task.State)
                            {

                                TextBlock target = getTextBlockFromLisboxItem("txtblockNameTask", item);
                                target.TextDecorations = TextDecorations.Strikethrough;
                            }
                        }
                    }
                }
            }
            
        }        

      


   

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
           // SetCrossOutTitle();
        }

        /// <summary>
        /// action effectué quand la listbox est chargé
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_Loaded(object sender, RoutedEventArgs e)
        {
            SetCrossOutTitle();
        }

        /// <summary>
        /// action effectué quand la checkbox est checké
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_TaskState_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = e.Source as CheckBox;

            CheckboxAction(checkBox);
           
        }


        /// <summary>
        /// action effectué quand la checkbox n'est plus checké
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_TaskState_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = e.Source as CheckBox;
            CheckboxAction(checkBox);
        }


        /// <summary>
        /// Change l'état de la tâche selon l'état de la checkbox
        /// </summary>
        /// <param name="checkBox">checkbox affilié à la tâche</param>
        private void CheckboxAction( CheckBox checkBox)
        {
            //recherche la tâche affilié à la checkbox
            foreach (Task task in selectedList.Tasks)
            {
                if (task.IDTask == int.Parse(checkBox.Tag.ToString()))
                {
                    //change l'état de la tâche sur la base de donné d'après l'état de la checkbox
                    databaseHandler.TaskDAO.UpdateTaskState(task);


                    //barre ou enlève le barrage du textblock de la tâche
                    foreach (object objet in ListBox_Tasks.Items)
                    {
                        ListBoxItem item = (ListBoxItem)ListBox_Tasks.ItemContainerGenerator.ContainerFromItem(objet);

                        if (item != null)
                        {
                            if (item.Tag.ToString() == checkBox.Tag.ToString())
                            {
                                TextBlock target = getTextBlockFromLisboxItem("txtblockNameTask", item);


                                if (checkBox.IsChecked.Value)
                                {
                                    //ajoute un barrage 
                                    target.TextDecorations = TextDecorations.Strikethrough;
                                }
                                else
                                {
                                    //enlève le barrage
                                    target.TextDecorations = null;
                                }

                                break;
                            }
                        }
                    }

                    break;
                }
            }
        }


        /// <summary>
        /// Suppression de la tâche 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_Delete(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = e.Source as MenuItem;
            for(int i=0; i < ListBox_Tasks.Items.Count - 1; i++)
            {
                //supprime la tâche de la base de donnée
                if (int.Parse( menuItem.Tag.ToString()) == selectedList.Tasks[i].IDTask)
                {
                    databaseHandler.TaskDAO.DeletedTask(selectedList.Tasks[i]);
                    selectedList.Tasks.RemoveAt(i);
                    ListBox_Tasks.Items.Refresh();

                    break;
                }
            }
        }


    }
}
