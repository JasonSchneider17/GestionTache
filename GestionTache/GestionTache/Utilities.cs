using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace GestionTache
{
    public static class Utilities
    {

        /// <summary>
        /// Trie les tâches selon le type de trie défini
        /// </summary>
        /// <param name="sort">type de trie</param>
        /// <param name="Tasks">Liste de tâche à trier</param>
        public static void SortTasks(TypeSort sort,ObservableCollection<Task> Tasks)
        {

            for (int j = Tasks.Count - 1; j > 0; j--)
            {
                for (int i = 0; i < j; i++)
                {
                    switch (sort.ID)
                    {
                        //trie par aucun type spécifique
                        case 0:
                            if (Tasks[i].Index > Tasks[i + 1].Index)
                            {
                                Tasks.Move(i, i + 1);
                            }
                            break;
                        //trie des tâches par les moins urgents
                        case 1:
                            if (Tasks[i].Priority.DegreePriority > Tasks[i + 1].Priority.DegreePriority)
                            {
                                Tasks.Move(i, i + 1);
                            }
                            break;
                        //trie des tâches par les plus urgents
                        case 2:
                            if (Tasks[i].Priority.DegreePriority < Tasks[i + 1].Priority.DegreePriority)
                            {
                                Tasks.Move(i, i + 1);
                            }
                            break;
                        //trie des tâches par ceux non réaliser
                        case 3:
                            int truc = Tasks[i].State.CompareTo(Tasks[i + 1].State);
                            if (truc < 0)
                            {
                                Tasks.Move(i, i + 1);
                            }
                            break;
                        //trie des tâches par ceux déja réalisé
                        case 4:
                            int compare = Tasks[i].State.CompareTo(Tasks[i + 1].State);
                            if (compare > 0)
                            {
                                Tasks.Move(i, i + 1);
                            }
                            break;
                    }
                }
            }
        }


        /// <summary>
        /// Permet de manipuler le nombre indiquant le nombre de tâche non réaliser de la liste 
        /// </summary>
        /// <param name="isAdding"> indique si il faut incrémenter</param>
        /// <param name="idList">id de la liste ou faire l'ajout</param>
        public static void manipulateNumberTaskToDo(bool isAdding, int idList,ObservableCollection<ListOfTasks> lists)
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
                    break;
                }
            }
        }



    }
}
