using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
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

        Database databaseObject;            //base de données
        DatabaseHandler databaseHandler;    //interagir avec base de données
        List<ListOfTasks> lists;            //liste
        //bool listIsEdited = false;        //indique si on est en mode d'edition de tâche
        ListOfTasks selectedList;           //liste sélectionner par l'utilisateur
        string commentText;                 //Commentaire de la tâche
        bool isTaskHiding = false;           //indique si il faut cacher les tâches 
        List<TypeSort> typeSorts;


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

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






        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.



        public MainWindow()
        {
            InitializeComponent();
           // SetLanguageDictionary();

            //List<Task> tasks = new List<Task>();


            //database Conf
            databaseObject = new Database();
            databaseHandler = new DatabaseHandler(databaseObject);
            //vérifie si un fichier de base de donnée en crée un sinon
            if (databaseObject.FileCreated)
            {
                databaseHandler.CreateTables();
                databaseHandler.PriorityDAO.AddDefaultPriority();
            }

            //regarde la version de la base donné et réinitilise pour la mettre à jour
            Properties.Settings.Default.DatabaseVersionNew = 5;

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

            foreach (ListOfTasks list in lists)
            {
                list.NumberTaskToDo = databaseHandler.TaskDAO.CountTaskToDoByList(list.ID);
            }

            //attribue la liste à la listbox
            listBox_listOfTasks.ItemsSource = lists;
            DataContext = this;
            PopulateTypeSorts();
            cmbBoxSortTask.SelectedIndex=0;
        }


        private void PopulateTypeSorts()
        {
            typeSorts = new List<TypeSort>();
            typeSorts.Add(new TypeSort("Aucun", 0));
            typeSorts.Add(new TypeSort("Les moins urgents", 1));
            typeSorts.Add(new TypeSort("Les plus urgents", 2));
            typeSorts.Add(new TypeSort("Déja réaliser ", 3));
            typeSorts.Add(new TypeSort("Non réaliser", 4));


        }

        /// <summary>
        /// détermine le language de l'appli
        /// </summary>
        //private void SetLanguageDictionary()
        //{
        //    ResourceDictionary dict = new ResourceDictionary();
        //    switch (Thread.CurrentThread.CurrentCulture.ToString())
        //    {
        //        /*case "en-US":
        //            dict.Source = new Uri("..\\..\\Language\\StringResourcesEN.xaml", UriKind.Relative);
        //            break;*/
        //        case "fr-CH":
        //            dict.Source = new Uri("..\\..\\Language\\StringResourcesFR.xaml", UriKind.Relative);
        //            break;
        //        default:
        //            dict.Source = new Uri("..\\..\\Language\\StringResourcesFR.xaml", UriKind.Relative);
        //            break;
        //    }
        //    this.Resources.MergedDictionaries.Add(dict);
        //}


        /// <summary>
        /// Ajout d'une liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_AddList_Click(object sender, RoutedEventArgs e)
        {
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
            manipulateNumberTaskToDo(true, selectedList.ID);


            newTask.IDTask = databaseHandler.TaskDAO.getLastAddedTaskID();
            //ajoute la tâche à la liste de tâche actuellement affiché
            selectedList.Tasks.Add(newTask);
            ListBox_Tasks.Items.Refresh();

            int indexLastAddedTask=0;
            for(int i = 0; i < selectedList.Tasks.Count; i++)
            {
                if (newTask.IDTask == selectedList.Tasks[i].IDTask)
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



            ContentPresenter contentPresenter = FindVisualChild<ContentPresenter>(listBoxItem);
            //DataTemplate dataTemplate = contentPresenter.ContentTemplate;
            //Border grid = (Border)dataTemplate.FindName("BorderGridItemTask", contentPresenter);
            //grid.Background = GradientBackground();

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

        private void ListBoxItem_Selected_1(object sender, RoutedEventArgs e)
        {
            //récupère les infos de la liste séléctionner
            //ListBoxItem itemSelected = (ListBoxItem)listBox_listOfTasks.ItemContainerGenerator.ContainerFromItem(listBox_listOfTasks.SelectedItem);
            ListBoxItem itemSelected = e.Source as ListBoxItem;
            selectedList = (ListOfTasks)itemSelected.DataContext;
            //change le titre de l'affichage des tâches
            txtBox_TitleList.Text = selectedList.Name;
            //ajoute les tâches affilié à la liste  depuis la base de données dans la liste
            selectedList.Tasks = databaseHandler.TaskDAO.getAllTaskByListID(selectedList.ID, databaseHandler.PriorityDAO.getAllPriority());
            //source de donnée de la listbox de tâche
            ListBox_Tasks.ItemsSource = selectedList.Tasks;
            Button_AddTask.IsEnabled = true;
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
                    list.Name = textBox.Text;
                    databaseHandler.ListDAO.UpdateListName(list);
                    listBox_listOfTasks.Items.Refresh();
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
            { }

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            TextBox_Comment.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// action effectué quand la listbox est chargé
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBoxItem_Loaded(object sender, RoutedEventArgs e)
        {

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
        private void CheckboxAction(CheckBox checkBox)
        {
            //recherche la tâche affilié à la checkbox
            foreach (Task task in selectedList.Tasks)
            {
                if (task.IDTask == int.Parse(checkBox.Tag.ToString()))
                {
                    //change l'état de la tâche sur la base de donné d'après l'état de la checkbox
                    databaseHandler.TaskDAO.UpdateTaskState(task);

                    if (task.State)
                    {
                        manipulateNumberTaskToDo(false, task.ListID);
                    }
                    else
                    {
                        manipulateNumberTaskToDo(true, task.ListID);
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
            for (int i = 0; i < ListBox_Tasks.Items.Count; i++)
            {
                //supprime la tâche de la base de donnée
                if (int.Parse(menuItem.Tag.ToString()) == selectedList.Tasks[i].IDTask)
                {

                    //regarde si la tâche est réaliser
                    if (!selectedList.Tasks[i].State)
                    {
                        //si la tâche n'est pas réalisé envoie un message d'alerte
                        MessageBoxResult dialog = MessageBox.Show("Cette tâche n'est pas encore réalisé voulez-vous vraiment la supprimer?", "Alerte suppresion tâche", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

                        if (dialog == MessageBoxResult.Yes)
                        {
                            //supprime la tâche
                            manipulateNumberTaskToDo(false, selectedList.ID);
                            databaseHandler.TaskDAO.DeletedTask(selectedList.Tasks[i]);
                            selectedList.Tasks.RemoveAt(i);
                        }
                    }
                    else
                    {
                        //supprime la tâche
                        selectedList.Tasks.RemoveAt(i);
                        databaseHandler.TaskDAO.DeletedTask(selectedList.Tasks[i]);
                    }
                    ListBox_Tasks.Items.Refresh();
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

            foreach (Task task in selectedList.Tasks)
            {
                if (task.IDTask == int.Parse(menuItem.Tag.ToString()))
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

            for (int i = 0; i < lists.Count; i++)
            {
                if (int.Parse(menuItem.Tag.ToString()) == lists[i].ID)
                {
                    //regarde si la liste contient des tâches
                    if (lists[i].Tasks.Count > 0)
                    {
                        MessageBoxResult dialog = MessageBox.Show("En supprimant cette liste les tâches lié seron aussi supprimer voulez-vous continuer?", "Alerte suppresion liste", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

                        if (dialog == MessageBoxResult.Yes)
                        {
                            //supprime la liste et ses tâches
                            databaseHandler.ListDAO.DeleteList(lists[i]);
                            databaseHandler.TaskDAO.DeletedTaksByListID(lists[i].ID);
                            txtBox_TitleList.Text = "";
                            ListBox_Tasks.ItemsSource = null;
                            ListBox_Tasks.Items.Refresh();
                            lists.RemoveAt(i);
                            Button_AddTask.IsEnabled = false;
                        }
                    }
                    else
                    {
                        //supprime la liste
                        databaseHandler.ListDAO.DeleteList(lists[i]);
                        txtBox_TitleList.Text = "";
                        lists.RemoveAt(i);
                        Button_AddTask.IsEnabled = false;
                    }
                    listBox_listOfTasks.Items.Refresh();
                    break;
                }
            }
        }

        /// <summary>
        /// Met le nom de la liste en mode modification
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void List_Click_Update(object sender, RoutedEventArgs e)
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
        private void manipulateNumberTaskToDo(bool isAdding, int idList)
        {
            for (int i = 0; i < lists.Count; i++)
            {
                if (lists[i].ID == idList)
                {
                    if (isAdding)
                    {
                        lists[i].NumberTaskToDo++;
                    }
                    else
                    {
                        lists[i].NumberTaskToDo--;
                    }
                    listBox_listOfTasks.Items.Refresh();
                    break;
                }
            }
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

            foreach (Task task in selectedList.Tasks)
            {
                if (task.IDTask == int.Parse(item.Tag.ToString()))
                {
                    TextBox_Comment.Tag = task.IDTask.ToString();
                    Textblock_Comment.Tag = task.IDTask.ToString();
                    CommentText = task.Comment;
                    break;
                }
            }
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
                Keyboard.ClearFocus();
                foreach (Task task in selectedList.Tasks)
                {
                    if (task.IDTask == int.Parse(TextBox_Comment.Tag.ToString()))
                    {
                        if (task.Comment != CommentText)
                        {
                            task.Comment = CommentText;
                            databaseHandler.TaskDAO.UpdateTaskComment(task);
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveComment_Click(object sender, RoutedEventArgs e)
        {
            TextBox_Comment.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_Comment_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox_Comment.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Textblock_Comment_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock textBlock = e.Source as TextBlock;
            Task taskSelect = null;

            foreach (Task task in selectedList.Tasks)
            {
                if (task.IDTask == int.Parse(textBlock.Tag.ToString()))
                {
                    taskSelect = task;

                    break;
                }
            }
            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2 && !taskSelect.State)
            {
                TextBox_Comment.Visibility = Visibility.Visible;
            }
        }

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

        private void cmbBoxSortTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = e.Source as ComboBox;

            //DegreePriority = 1
            if (selectedList != null)
            {
                TypeSort sort = (TypeSort)combo.SelectedItem;
                //SortingTask(selectedList.Tasks,sort);
               SortingTask( sort);
                //ListBox_Tasks.ItemsSource = selectedList.Tasks;


                //ListBox_Tasks.Items.SortDescriptions.Add(new SortDescription("Priority.DegreePriority", ListSortDirection.Ascending));
                ListBox_Tasks.Items.Refresh();
            }
        }

        private void SortingTask(TypeSort sort)
        {
            if (sort.ID == 1 || sort.ID == 2)
            {
                for (int j = selectedList.Tasks.Count - 1; j > 0; j--)
                {
                    for (int i = 0; i < j; i++)
                    {
                        switch (sort.ID)
                        {
                            case 1:
                                if (selectedList.Tasks[i].Priority.DegreePriority > selectedList.Tasks[i + 1].Priority.DegreePriority)
                                {
                                    Task temporary;
                                    temporary = selectedList.Tasks[i];
                                    selectedList.Tasks[i] = selectedList.Tasks[i + 1];
                                    selectedList.Tasks[i + 1] = temporary;
                                }
                                break;
                            case 2:
                                if (selectedList.Tasks[i].Priority.DegreePriority < selectedList.Tasks[i + 1].Priority.DegreePriority)
                                {
                                    Task temporary;
                                    temporary = selectedList.Tasks[i];
                                    selectedList.Tasks[i] = selectedList.Tasks[i + 1];
                                    selectedList.Tasks[i + 1] = temporary;
                                }
                                break;
                        }

                    }
                }
            }


        }
    }
}
