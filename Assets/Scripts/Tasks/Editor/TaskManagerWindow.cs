using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace WS20.P3.Overcrowded
{
    public class TaskManagerWindow : EditorWindow
    {

        [MenuItem("Tools/TaskManager Spawnpoint Editor")]
        public static void Open()
        {
            GetWindow<TaskManagerWindow>();
        }

        public Transform taskManager;

        public GameObject wall;

        public List<GameObject> prefabs;



        TasktypesEnum.OPTIONS display = TasktypesEnum.OPTIONS.Wall;

        private void OnGUI()
        {
            SerializedObject obj = new SerializedObject(this);

            EditorGUILayout.PropertyField(obj.FindProperty("taskManager"));
            EditorGUILayout.PropertyField(obj.FindProperty("prefabs"));

            if (taskManager == null)
            {
                EditorGUILayout.HelpBox("Root transform must be selected. Please assign a root transform.", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.BeginVertical("box");
                DrawButtons();
                EditorGUILayout.EndVertical();
            }

            obj.ApplyModifiedProperties();
        }

        void DrawButtons()
        {
            display = (TasktypesEnum.OPTIONS)EditorGUILayout.EnumPopup(display);
            if (GUILayout.Button("Create Task Spawnpoint"))
            {

                CreateSpawnpoint();
            }
            if (Selection.activeObject != null && Selection.activeGameObject.GetComponent<TaskSpawnpoint>())
            {
                EditorGUILayout.Space(20);


                if (GUILayout.Button("Refresh Spawnpoint"))
                {
                    Selection.activeGameObject.GetComponent<TaskSpawnpoint>().gizmoMeshesCached = false;
                    //CreateBranch();
                }

            }
        }

        void CreateSpawnpoint()
        {
            GameObject taskSpawnpointObject = new GameObject("TaskSpawnpoint " + taskManager.childCount, typeof(TaskSpawnpoint));
            taskSpawnpointObject.transform.SetParent(taskManager, false);

            TaskSpawnpoint spawnpoint = taskSpawnpointObject.GetComponent<TaskSpawnpoint>();

            spawnpoint.option = display;
            spawnpoint.prefabs = prefabs;

            Selection.activeGameObject = spawnpoint.gameObject;
        }

    }

}
