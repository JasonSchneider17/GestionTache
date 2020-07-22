using GuiLabs.Undo;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        Database databaseObject;                            //base de données
        ObservableCollection<ListOfTasks> lists;            //listes
        ListOfTasks selectedList;                           //liste sélectionner par l'utilisateur
        string commentText;                                 //Commentaire de la tâche
        bool isTaskHiding = false;                          //indique si il faut cacher les tâches déja réalisé
        List<TypeSort> typeSorts;                           //type de trie
        ObservableCollection<Task> tasks;                   //Liste de tâches
        int selectedIndexSortTask=0;                        //index indiquant le trie de sélectionner
        //ActionManager actionManager = new ActionManager();
        //bool isListAdded = false;


        /// <summary>
        /// constructeur
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            SetLanguageDictionary();

            //database Conf
            databaseObject = new Database();
            MyGlobals.databaseHandler = new DatabaseHandler(databaseObject);
            //vérifie si un fichier de base de donnée en crée un sinon
            if (databaseObject.FileCreated)
            {
                MyGlobals.databaseHandler.CreateTables();
                MyGlobals.databaseHandler.PriorityDAO.AddDefaultPriority();
            }
         
            //charge toute les liste présent sur la base de donnée
            lists = MyGlobals.databaseHandler.ListDAO.GetAllList();

            foreach (ListOfTasks list in lists)
            {
                list.NumberTaskToDo = MyGlobals.databaseHandler.TaskDAO.CountTaskToDoByList(list.ID);
            }
            DataContext = this;
            PopulateTypeSorts();
            cmbBoxSortTask.SelectedIndex = 0;
        }

        /// <summary>
        /// Rempli la liste représentant les sortes de triage
        /// </summary>
        private void PopulateTypeSorts()
        {
            typeSorts = new List<TypeSort>();
            typeSorts.Add(new TypeSort((string)this.FindResource("NothingItemSort"), 0));
            typeSorts.Add(new TypeSort((string)this.FindResource("LessUrgentItemSort"), 1));
            typeSorts.Add(new TypeSort((string)this.FindResource("MoreUrgentItemSort"), 2));
            typeSorts.Add(new TypeSort((string)this.FindResource("CheckedItemSort"), 3));
            typeSorts.Add(new TypeSort((string)this.FindResource("UncheckedItemSort"), 4));
        }

        /// <summary>
        /// détermine le language de l'appli
        /// </summary>
        private void SetLanguageDictionary()
        {
            ResourceDictionary dict = new ResourceDictionary();
            switch (Thread.CurrentThread.CurrentCulture.ToString())
            {
               
                case "de-CH":    //allemand suisse                
                    dict.Source = new Uri("..\\..\\Language\\StringResourcesDE.xaml", UriKind.Relative);
                    break;
                case "fr-CH":    //français suisse
                    dict.Source = new Uri("..\\..\\Language\\StringResourcesFR.xaml", UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("..\\..\\Language\\StringResourcesFR.xaml", UriKind.Relative);
                    break;
            }
            this.Resources.MergedDictionaries.Add(dict);
        }


        /// <summary>
        /// Ajout d'une liste depuis le bouton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_AddList_Click(object sender, RoutedEventArgs e)
        {
            AddList();
        }

        /// <summary>
        /// Ajout d'une liste depuis le menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItem_addListOfTask_Click(object sender, RoutedEventArgs e)
        {
            AddList();
        }

        /// <summary>
        /// Ajoute une nouvelle liste et rentre en mode edition de son titre
        /// </summary>
        private void AddList()
        {
            ListOfTasks list = new ListOfTasks("NewList",lists.Count);

            /*AddListAction addListAction = new AddListAction(list,MyGlobals.databaseHandler,lists);
            actionManager.RecordAction(addListAction);*/
            //isListAdded = true;

            //ajoute la liste dans la base de donnée et récupère l'id attribué par la base de donnée à la liste ajouté
            MyGlobals.databaseHandler.ListDAO.Add(list);
            list.ID = MyGlobals.databaseHandler.ListDAO.getLastAddedListID();
            lists.Add(list);

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
        /// ajout d'une tâche depuis le bouton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_AddTask_Click(object sender, RoutedEventArgs e)
        {
            AddTask();
        }

        /// <summary>
        /// ajoute une tâche depuis le menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItem_addTask_Click(object sender, RoutedEventArgs e)
        {
            AddTask();
        }

        /// <summary>
        /// Ajout d'une nouvelle tâche et rentre en mode édition du titre
        /// </summary>
        private void AddTask()
        {
            List<Priority> listpriority = MyGlobals.databaseHandler.PriorityDAO.GetAllPriority();
            //ajoute la tâche dans la base de donnée et récupère l'id attribué par la base de donnée à la tâche ajouté
            Task newTask = new Task("newTask", "", false, 0, selectedList.ID, listpriority, listpriority[0].IDPriority,Tasks.Count);
            MyGlobals.databaseHandler.TaskDAO.Add(newTask);
            //AddTaskAction action = new AddTaskAction(newTask,tasks);
            //actionManager.RecordAction(action);
            
            ManipulateNumberTaskToDo(true, selectedList.ID);
            newTask.IDTask = MyGlobals.databaseHandler.TaskDAO.GetLastAddedTaskID();
            Tasks.Add(newTask);

            SortingTask();
            TotalTasksUpdate();
            

            //Recherche la tâche venant d'être ajouté dans la listbox
            int indexLastAddedTask = 0;
            for (int i = 0; i < Tasks.Count; i++)
            {
                if (newTask.IDTask == Tasks[i].IDTask)
                {
                    indexLastAddedTask = i;
                    break;
                }
            }
            //selectionne le listboxitem ajouté et focus la vue dessus
            ListBox_Tasks.SelectedItem = ListBox_Tasks.Items[indexLastAddedTask];
            ListBox_Tasks.SelectedItem = ListBox_Tasks.Items;
            ListBox_Tasks.ScrollIntoView(ListBox_Tasks.SelectedItem);
            ListBox_Tasks.UpdateLayout();


            ListBoxItem listBoxItem = (ListBoxItem)ListBox_Tasks.ItemContainerGenerator.ContainerFromIndex(indexLastAddedTask);

            //affiche la textbox pour éditer le titre de la tâche
            TextBox target = getTextBoxFromLisboxItem("txtboxNameTask", listBoxItem);
            target.Visibility = Visibility.Visible;
            target.Focus();
            target.SelectAll();
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
        /// Action lors de la sélection d'une liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_Selected_1(object sender, RoutedEventArgs e)
        {
            //récupère les infos de la liste séléctionner
            ListBoxItem itemSelected = e.Source as ListBoxItem;
            selectedList = (ListOfTasks)itemSelected.DataContext;
            //change le titre de l'affichage des tâches
            txtBox_TitleList.Text = selectedList.Name;

            if (Tasks != null && cmbBoxSortTask.SelectedIndex==0)
            {
                UpdateTasksIndex();
            }

            //ajoute les tâches affilié à la liste  depuis la base de données dans la liste
            Tasks = MyGlobals.databaseHandler.TaskDAO.GetAllTaskByListID(selectedList.ID, MyGlobals.databaseHandler.PriorityDAO.GetAllPriority());
            SortingTask();
            TotalTasksUpdate();
            DataContext = this;

            //active et désactive des contrôles
            Button_AddTask.IsEnabled = true;
            menuItem_addTask.IsEnabled = true;
            menuItem_DeleteListOfTask.IsEnabled = true;
            menuItem_UpdateListOfTask.IsEnabled = true;
            menuItem_EdiTask.IsEnabled = false;
            menuItem_DeleteTask.IsEnabled = false;
            BorderComment.Visibility = Visibility.Hidden;
        }

       

        /// <summary>
        /// Sauvegarde le nouveau nom de la liste dans la base de donnée et masque la textbox d'édition
        /// </summary>
        /// <param name="textBox"> textbox édité</param>
        private void ListNameEndEdition(TextBox textBox)
        {
            //masque la textbox pour n'afficher que le textblock
            textBox.Visibility = Visibility.Hidden;

            //recherche la liste correspondant à la textbox et la met à jour sur la base de données
            foreach (ListOfTasks list in lists)
            {
                if (list.ID == int.Parse(textBox.Tag.ToString()))
                {
                    /*if (isListAdded)
                    {
                       list.Name = textBox.Text;
                        txtBox_TitleList.Text = list.Name;
                        MyGlobals.databaseHandler.ListDAO.UpdateListName(list);
                        isListAdded = false;   
                    }
                    else
                    {
                        RenameListAction action = new RenameListAction(MyGlobals.databaseHandler, list, textBox.Text);
                        actionManager.RecordAction(action);
                        txtBox_TitleList.Text = list.Name;
                    }*/
                    list.Name = textBox.Text;
                    txtBox_TitleList.Text = list.Name;
                    MyGlobals.databaseHandler.ListDAO.UpdateListName(list);

                    break;
                }
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
            foreach (Task task in Tasks)
            {
                if (task.IDTask == int.Parse(textBox.Tag.ToString()))
                {
                    MyGlobals.databaseHandler.TaskDAO.UpdateTaskName(task);
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
                Keyboard.ClearFocus();
            }
        }

        /// <summary>
        /// Actions effectué lors de l'appuie d'une touche du clavier sur listItem de liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_KeyDown(object sender, KeyEventArgs e)
        {
            ListBoxItem listBoxItem = e.Source as ListBoxItem;
            //Suppression liste
            if (e.Key == Key.Delete)
            {
                DeleteList(int.Parse(listBoxItem.Tag.ToString()));
            }

        }

        /// <summary>
        /// si le focus de la textbox pour le nom de la liste est quitter ajoute cette liste dans la base de données
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtBoxNameList_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
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
        /// Suppression tâche avec touche delete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItemTaskDelete(object sender, KeyEventArgs e)
        {
            ListBoxItem listBoxItem = e.Source as ListBoxItem;

            if (e.Key == Key.Delete)
            {
                DeleteTask(int.Parse(listBoxItem.Tag.ToString()));
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
                foreach (Task task in Tasks)
                {
                    if (task.IDTask == int.Parse(comboBox.Tag.ToString()))
                    {
                        //change la priorité de la tâche et la met à jour sur la base de données
                        foreach (Priority priority in task.PrioritiesDisplay)
                        {
                            if (priority.IDPriority == idPriority.IDPriority)
                            {
                                task.IdPriority = priority.IDPriority;

                                MyGlobals.databaseHandler.TaskDAO.UpdateTaskPriorityId(task);
                                ListBox_Tasks.Items.Refresh();
                            }
                        }
                    }
                }
                //Trie à nouveau les tâche si l'une des options de trie par priorité est sélectionner
                TypeSort sort = (TypeSort)cmbBoxSortTask.SelectedItem;
                if (sort.ID == 1 || sort.ID == 2)
                {
                    SortingTask();
                }
            }
            catch (IndexOutOfRangeException)
            { }

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
            //trie les tâches à nouveau si les options de trie par l'état de la tâche est sélectionner
            TypeSort sort = (TypeSort)cmbBoxSortTask.SelectedItem;
            if (sort.ID == 4 || sort.ID == 3)
            {
                SortingTask();
            }
            TotalTasksUpdate();
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
            //trie les tâches à nouveau si les options de trie par l'état de la tâche est sélectionner
            TypeSort sort = (TypeSort)cmbBoxSortTask.SelectedItem;
            if (sort.ID == 4 || sort.ID == 3)
            {
                SortingTask();
            }
            TotalTasksUpdate();
        }

        /// <summary>
        /// Change l'état de la tâche selon l'état de la checkbox
        /// </summary>
        /// <param name="checkBox">checkbox affilié à la tâche</param>
        private void CheckboxAction(CheckBox checkBox)
        {
            //recherche la tâche affilié à la checkbox
            foreach (Task task in Tasks)
            {
                if (task.IDTask == int.Parse(checkBox.Tag.ToString()))
                {
                    //change l'état de la tâche sur la base de donné d'après l'état de la checkbox
                    MyGlobals.databaseHandler.TaskDAO.UpdateTaskState(task);

                    if (task.State)
                    {
                        ManipulateNumberTaskToDo(false, task.ListID);
                    }
                    else
                    {
                        ManipulateNumberTaskToDo(true, task.ListID);
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
        private void MenuItem_Task_Click_Delete(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = e.Source as MenuItem;
            DeleteTask(int.Parse(menuItem.Tag.ToString()));
        }

        /// <summary>
        /// Suppression liste sélectionner depuis le menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItem_DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem listBoxItem = (ListBoxItem)ListBox_Tasks.ItemContainerGenerator.ContainerFromItem(ListBox_Tasks.SelectedItem);
            DeleteTask(int.Parse(listBoxItem.Tag.ToString()));
        }
        /// <summary>
        /// Supression d'une tâche
        /// </summary>
        /// <param name="idTask">id de la tâche à supprimer </param>
        private void DeleteTask(int idTask)
        {
            for (int i = 0; i < ListBox_Tasks.Items.Count; i++)
            {
                //supprime la tâche de la base de donnée
                if (idTask == Tasks[i].IDTask)
                {
                    //regarde si la tâche est réaliser
                    if (!Tasks[i].State)
                    {
                        //si la tâche n'est pas réalisé envoie un message d'alerte
                        MessageBoxResult dialog = MessageBox.Show((string)this.FindResource("DeleteTaskMessageContent"), (string)this.FindResource("DeleteTaskMessageTitle"), MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

                        if (dialog == MessageBoxResult.Yes)
                        {
                            //supprime la tâche
                            ManipulateNumberTaskToDo(false, selectedList.ID);
                            MyGlobals.databaseHandler.TaskDAO.DeleteTask(Tasks[i]);
                            Tasks.RemoveAt(i);
                            TotalTasksUpdate();
                            menuItem_EdiTask.IsEnabled = false;
                            menuItem_DeleteTask.IsEnabled = false;
                            BorderComment.Visibility = Visibility.Hidden;
                        }
                    }
                    else
                    {
                        //supprime la tâche
                        MyGlobals.databaseHandler.TaskDAO.DeleteTask(Tasks[i]);
                        Tasks.RemoveAt(i);
                        TotalTasksUpdate();
                        menuItem_EdiTask.IsEnabled = false;
                        menuItem_DeleteTask.IsEnabled = false;
                        BorderComment.Visibility = Visibility.Hidden;
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Entrer dans la textbox pour modifier le nom de la tâche
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Task_Click_Update(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = e.Source as MenuItem;
            EditTask(int.Parse(menuItem.Tag.ToString()));
           
        }
        /// <summary>
        /// Entrer dans la textbox pour modifier le nom de la tâche depuis le menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItem_EdiTask_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem listBoxItem = (ListBoxItem)ListBox_Tasks.ItemContainerGenerator.ContainerFromItem(ListBox_Tasks.SelectedItem);
            EditTask(int.Parse(listBoxItem.Tag.ToString()));
        }

        /// <summary>
        /// Entre en mode édition du nom de la tâche
        /// </summary>
        /// <param name="idTask"></param>
        private void EditTask(int idTask)
        {
            foreach (Task task in Tasks)
            {
                if (task.IDTask == idTask)
                {
                    //modifie seulement les tâche non réalisé
                    if (!task.State)
                    {
                        ListBoxItem listBoxItem = (ListBoxItem)ListBox_Tasks.ItemContainerGenerator.ContainerFromItem(ListBox_Tasks.SelectedItem);
                        TextBox target = getTextBoxFromLisboxItem("txtboxNameTask", listBoxItem);
                        target.Visibility = Visibility.Visible;
                        target.Focus();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Suppresion d'une liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void List_Click_Delete(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = e.Source as MenuItem;
            DeleteList(int.Parse(menuItem.Tag.ToString()));
        }
        /// <summary>
        /// Suppresision d'une liste depuis le menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItem_DeleteListOfTask_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem listBoxItem = (ListBoxItem)listBox_listOfTasks.ItemContainerGenerator.ContainerFromItem(listBox_listOfTasks.SelectedItem);
            DeleteList(int.Parse(listBoxItem.Tag.ToString()));
        }
        /// <summary>
        /// Supprimer une liste et ses tâches liés
        /// </summary>
        /// <param name="tag"></param>
        private void DeleteList(int tag)
        {            
            for (int i = 0; i < lists.Count; i++)
            {
                if (tag == lists[i].ID)
                {
                    MessageBoxResult dialog = MessageBox.Show((string)this.FindResource("DeleteListMessageContent"), (string)this.FindResource("DeleteListMessageTitle"), MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                    
                    if (dialog == MessageBoxResult.Yes)
                    {
                        //supprime la liste et ses tâches
                        MyGlobals.databaseHandler.ListDAO.DeleteList(lists[i]);
                        MyGlobals.databaseHandler.TaskDAO.DeleteTasksByListID(lists[i].ID);
                        //DeleteListAction action = new DeleteListAction(lists, i, MyGlobals.databaseHandler);
                        //actionManager.RecordAction(action);
                        txtBox_TitleList.Text = "";
                        Tasks = null;
                        lists.RemoveAt(i);
                        Button_AddTask.IsEnabled = false;
                        menuItem_DeleteListOfTask.IsEnabled = false;
                        menuItem_UpdateListOfTask.IsEnabled = false;
                        BorderComment.Visibility = Visibility.Hidden;
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Met le nom de la liste en mode édition
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void List_Click_Update(object sender, RoutedEventArgs e)
        {
            ListEdit();
        }
        /// <summary>
        /// Met le nom de la liste en mode édition depuis le menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItem_UpdateListOfTask_Click(object sender, RoutedEventArgs e)
        {
            ListEdit();
        }

        /// <summary>
        /// Fais entrer la liste sélectionner en mode édition
        /// </summary>
        private void ListEdit()
        {
            ListBoxItem listBoxItem = (ListBoxItem)listBox_listOfTasks.ItemContainerGenerator.ContainerFromItem(listBox_listOfTasks.SelectedItem);
            TextBox target = getTextBoxFromLisboxItem("txtBoxNameList", listBoxItem);
            target.Visibility = Visibility.Visible;
            target.Focus();
        }

        /// <summary>
        /// Permet de manipuler le nombre indiquant le nombre de tâche non réaliser de la liste 
        /// </summary>
        /// <param name="isAdding"> indique si il faut incrémenter</param>
        /// <param name="idList">id de la liste ou faire l'ajout</param>
        private void ManipulateNumberTaskToDo(bool isAdding, int idList)
        {
            Utilities.manipulateNumberTaskToDo(isAdding, idList, lists);       
        }

        /// <summary>
        /// La liste sélectionner affiche son commentaire dans la section commentaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            ListBoxItem item = e.Source as ListBoxItem;
            BorderComment.Visibility = Visibility.Visible;

            //recherche la tâche sélectionner et attribue son id comme tag aux contrôle du commentaire
            foreach (Task task in Tasks)
            {
                if (task.IDTask == int.Parse(item.Tag.ToString()))
                {
                    TextBox_Comment.Tag = task.IDTask.ToString();
                    Textblock_Comment.Tag = task.IDTask.ToString();
                    CommentText = task.Comment;
                    break;
                }
            }

            menuItem_EdiTask.IsEnabled = true;
            menuItem_DeleteTask.IsEnabled = true;
        }

        /// <summary>
        /// Arrêter l'édition du commentaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_Comment_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                SaveComment();
            }
        }
        /// <summary>
        /// Sauvegarde le commentaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCommentSave_Click(object sender, RoutedEventArgs e)
        {
            SaveComment();
        }

        /// <summary>
        /// Sauvegarde du commentaire
        /// </summary>
        private void SaveComment()
        {
            Keyboard.ClearFocus();
            foreach (Task task in Tasks)
            {              
                if (task.IDTask == int.Parse(TextBox_Comment.Tag.ToString()))
                {
                    if (task.Comment != CommentText)
                    {
                        //modifie le commentaire
                        task.Comment = CommentText;
                        MyGlobals.databaseHandler.TaskDAO.UpdateTaskComment(task);
                    }
                    break;
                }
            }
        }


        /// <summary>
        /// Quan la textbox du commentaire perd le focus du clavier elle devient caché
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_Comment_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox_Comment.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Entrer en mode édition du commentaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Textblock_Comment_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock textBlock = e.Source as TextBlock;
            Task taskSelect = null;
            //recherche la tâche affilié au commentaire
            foreach (Task task in Tasks)
            {
                if (task.IDTask == int.Parse(textBlock.Tag.ToString()))
                {
                    taskSelect = task;

                    break;
                }
            }

            //Rentre en mode édition du commentaire
            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2 && !taskSelect.State)
            {
                TextBox_Comment.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Permet d'alterner entre l'affichage et le masquage des tâches déja réaliser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Visibility_Task_Click(object sender, RoutedEventArgs e)
        {
            IsTaskHiding = !IsTaskHiding;
            if (Button_Visibility_Task.Content == FindResource("visible"))
            {
                Button_Visibility_Task.Content = FindResource("hidden");
            }
            else
            {
                Button_Visibility_Task.Content = FindResource("visible");
            }
            ListBox_Tasks.Items.Refresh();

        }

        /// <summary>
        /// Sélectionne un type trie et l'applique
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbBoxSortTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (Tasks != null && selectedIndexSortTask == 0)
            {
                UpdateTasksIndex();
            }
            selectedIndexSortTask = cmbBoxSortTask.SelectedIndex;
            if (selectedList != null)
            {
                SortingTask();
            }
        }

        /// <summary>
        /// trie des tâches
        /// </summary>
        private void SortingTask()
        {
            TypeSort sort = (TypeSort)cmbBoxSortTask.SelectedItem;
            Utilities.SortTasks(sort, Tasks);
        }

        /// <summary>
        /// Mets a jour la progress bar des tâches et son texte
        /// </summary>
        public void TotalTasksUpdate()
        {
            int totalNumberTasks = Tasks.Count;
            int numberTaskDo = 0;
            //Compte le nombre de tâche réalisé
            foreach (Task task in Tasks)
            {
                if (task.State)
                {
                    numberTaskDo++;
                }
            }
            //paramètre les valeurs de la progressbar
            progressBarTasks.Maximum = totalNumberTasks;
            progressBarTasks.Value = numberTaskDo;
            //change le texte du textblock de la progressBar
            string text = (string)this.FindResource("TxtBlockProgressBar");
            textBlockTotalTasks.Text = string.Format("{0}/{1} {2}", numberTaskDo, totalNumberTasks, text);
        }

        /// <summary>
        /// Mets à jour la position des tâches dans la liste sur la base de données
        /// </summary>
        private void UpdateTasksIndex()
        {
            //changing
            for (int i = 0; i < Tasks.Count; i++)
            {
                Tasks[i].Index = i;
            }
            MyGlobals.databaseHandler.TaskDAO.UpdateTasksIndex(Tasks);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        //getter setter pour Binding

        /// <summary>
        /// getter setter commentaire
        /// </summary>
        public string CommentText
        {
            get { return this.commentText; }
            set
            {
                this.commentText = value;
                OnPropertyChanged();
            }
        }

        public bool IsTaskHiding
        {
            get { return this.isTaskHiding; }
            set { this.isTaskHiding = value; }
        }

        public List<TypeSort> TypeSorts
        {
            get { return this.typeSorts; }
            set { this.typeSorts = value; }
        }

        public ObservableCollection<Task> Tasks
        {
            get { return this.tasks; }
            set
            {
                this.tasks = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ListOfTasks> Lists
        {
            get { return this.lists; }
            set
            {
                this.lists = value;
                OnPropertyChanged();
            }
        }

        private void Undo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //actionManager.Undo();
        }
        private void Redo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //actionManager.Redo();
        }

        private void MenuHelp_Click(object sender, RoutedEventArgs e)
        {
            Helps.HelpProvider.ShowHelpTableOfContents();
        }

        private void Help_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Helps.HelpProvider.ShowHelpTableOfContents();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            for(int i=0; i < lists.Count; i++)
            {
                lists[i].IndexArray = i;


            }
            MyGlobals.databaseHandler.ListDAO.UpdateListsIndex(lists);
        }

        /// <summary>
        /// Permet d'exporter la base de donnée actuelle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_export_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = (string)this.FindResource("DataBaseFilter") + " (*.db)|*.db";
            saveFileDialog.Title = (string)this.FindResource("ExportDataTitle");
            if (saveFileDialog.ShowDialog() == true)
            {
                File.Copy(MyGlobals.pathDatabase, saveFileDialog.FileName, true);
            }

        }

        /// <summary>
        /// Permet de remplacer la base de donnée actuelle par une autre à partir d'un fichier .db choisi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_import_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = (string)this.FindResource("DataBaseFilter") + " (*.db)|*.db";
            openFileDialog.Title = (string)this.FindResource("ImportDataTitle");
            if (openFileDialog.ShowDialog() == true)
            {
                MessageBoxResult dialog = MessageBox.Show((string)this.FindResource("ImportMessageContent"), (string)this.FindResource("ImportMessageTitle"), MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

                if (dialog == MessageBoxResult.Yes)
                {
                    File.Copy(openFileDialog.FileName, MyGlobals.pathDatabase, true);
                    Lists = MyGlobals.databaseHandler.ListDAO.GetAllList();
                    

                    foreach (ListOfTasks list in lists)
                    {
                        list.NumberTaskToDo = MyGlobals.databaseHandler.TaskDAO.CountTaskToDoByList(list.ID);
                    }
                    cmbBoxSortTask.SelectedIndex = 0;
                    DesactivateDisplayTask();
                }


            }

        }

        /// <summary>
        /// Remplace le fichier de base de données actuelle par un cichier vide
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_clearData_Click(object sender, RoutedEventArgs e)
        {

            MessageBoxResult dialog = MessageBox.Show((string)this.FindResource("ClearMessageContent"), (string)this.FindResource("ClearMessageTitle"), MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            if (dialog == MessageBoxResult.Yes)
            {
                databaseObject = new Database();
                MyGlobals.databaseHandler = new DatabaseHandler(databaseObject);
                databaseObject.CreateFile();
                if (databaseObject.FileCreated)
                {
                    MyGlobals.databaseHandler.CreateTables();
                    MyGlobals.databaseHandler.PriorityDAO.AddDefaultPriority();
                }
                cmbBoxSortTask.SelectedIndex = 0;
                DesactivateDisplayTask();
                Lists.Clear();
            }
        }



        private void DesactivateDisplayTask()
        {
            txtBox_TitleList.Text = "";
            Button_AddTask.IsEnabled = false;
            progressBarTasks.Value = 0;
            cmbBoxSortTask.SelectedIndex = 0;
            textBlockTotalTasks.Text = "";
            if (Tasks != null)
            Tasks.Clear();
            BorderComment.Visibility = Visibility.Hidden;
        }
    }



}

