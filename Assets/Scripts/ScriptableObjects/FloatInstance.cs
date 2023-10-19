using UnityEngine;
using UnityEngine.SceneManagement;

namespace WS20.P3.Overcrowded
{
    [CreateAssetMenu]
    public class FloatInstance : ScriptableObject
    {
        #region Public Fields

        public float Float;
        public float BaseValue;
        public bool resetValue;

        #endregion

        #region Monobehaviour Callbacks

        private void OnEnable()
        {
            if (resetValue)
            {
                Float = BaseValue;
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Launcher")
            {
                Float = BaseValue;
            }
        }        

        #endregion
    }
}