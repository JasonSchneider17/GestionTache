using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionTache
{

    /// <summary>
    /// Contient toute les informations de tables de la base de donné
    /// </summary>
    public static class DatabaseBuild
    {


        //TABLE task 
        public const String TASK_KEY = "id_task";
        public const String TASK_NAME = "name_task";
        public const String TASK_COMMENT = "comment_task";
        public const String TASK_STATE = "state_task";
        public const String TASK_ID_LIST = "id_list";
        public const String TASK_ID_PRIORITY = "id_priority";

        public const String TASK_TABLE_NAME = "Task";
        public const String TASK_TABLE_CREATE =
                "CREATE TABLE " + TASK_TABLE_NAME + " (" +
                        TASK_KEY + " INTEGER PRIMARY KEY AUTOINCREMENT, " +
                        TASK_NAME + " TEXT, " +
                        TASK_COMMENT + " TEXT, " +
                        TASK_STATE + " INTEGER," +
                         TASK_ID_LIST + " INTEGER, "+
                         TASK_ID_PRIORITY+" INTEGER ) ";
        public const String TASK_TABLE_DROP = "DROP TABLE IF EXISTS " + TASK_TABLE_NAME + ";";


        //table list
        public const String LIST_KEY = "id_list";
        public const String LIST_NAME = "name_list";
        public const String LIST_TABLE_NAME = "List";
        public const String LIST_TABLE_CREATE =
                "CREATE TABLE " + LIST_TABLE_NAME + " (" +
                        LIST_KEY + " INTEGER PRIMARY KEY AUTOINCREMENT, " +
                        LIST_NAME + " TEXT )";
        public const String LIST_TABLE_DROP = "DROP TABLE IF EXISTS " + LIST_TABLE_NAME + ";";

        //table priority
        public const String PRIORITY_KEY = "id_priority";
        public const String PRIORITY_NAME = "name_priority";
        public const String PRIORITY_DEGREE = "degree_priority";
        public const String PRIORITY_TABLE_NAME = "Priority";
        public const String PRIORITY_TABLE_CREATE =
                "CREATE TABLE " + PRIORITY_TABLE_NAME + " (" +
                        PRIORITY_KEY + " INTEGER PRIMARY KEY AUTOINCREMENT, " +
                        PRIORITY_NAME + " TEXT, " +
                        PRIORITY_DEGREE + " INTEGER )";
        public const String PRIORITY_TABLE_DROP = "DROP TABLE IF EXISTS " + PRIORITY_TABLE_NAME + ";";


    }
}
