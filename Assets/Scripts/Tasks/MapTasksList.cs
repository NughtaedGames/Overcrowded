using System.Collections.Generic;
using UnityEngine;

namespace WS20.P3.Overcrowded
{
    public class MapTasksList : MonoBehaviour
    {
        #region Public Fields

        public static MapTasksList Instance;

        public List<GameObject> taskMapPointers;

        #endregion

        #region MonoBehaviour CallBacks

        private void OnEnable()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion

        #region Public Methods

        public void DisplayTask(GameObject taskToDisplay)
        {
            for (int i = 0; i < taskMapPointers.Count; i++)
            {
                if (taskMapPointers[i] == taskToDisplay)
                {
                    taskMapPointers[i].SetActive(true);
                }
                else
                {
                    taskMapPointers[i].SetActive(false);
                }
            }
        }

        #endregion
    }
}