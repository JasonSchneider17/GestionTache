using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionTache
{
    public class ListOfTasks:INotifyPropertyChanged
    {
        private string name;                        //nom de la liste
        private int id;                             //ide de la liste sur la base de donnée
        private int numberTaskToDo;                 //nombre de tache devant encore être réaliser
        private int indexArray;                     //indique le numéro d'index de sa liste

        /// <summary>
        /// constructeur
        /// </summary>
        /// <param name="name"></param>
        public ListOfTasks(string name,int indexArray)
        {
            this.name = name;
            this.indexArray = indexArray;
        }


        /// <summary>
        /// constructeur
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /*public ListOfTasks(string name,int id)
        {
            this.name = name;
            this.id = id;
        }*/

        public ListOfTasks(string name,int id,int indexArray)
        {
            this.name = name;
            this.id = id;
            this.indexArray = indexArray;

        }

        //getter setter


        public string Name
        {
            get { return this.name; }
            set { this.name = value;
                OnPropertyChanged("Name");
            }
        }




        public int ID
        {
            get { return this.id; }
            set { this.id = value;
                OnPropertyChanged("ID");
            }
        }

        public int NumberTaskToDo
        {
            get { return this.numberTaskToDo; }
            set { this.numberTaskToDo = value;
                OnPropertyChanged("NumberTaskToDo");
            }
        }

        public int IndexArray
        {
            get { return this.indexArray; }
            set
            {
                this.indexArray = value;
                
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
