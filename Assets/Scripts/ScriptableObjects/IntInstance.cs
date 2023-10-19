using UnityEngine;
using UnityEngine.SceneManagement;

namespace WS20.P3.Overcrowded
{
    [CreateAssetMenu]
    public class IntInstance : ScriptableObject
    {
        public int Integer;
        public int BaseValue;
        public bool resetValue;
        
        private void OnEnable()
        {
            if (resetValue)
            {
                Integer = BaseValue;
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Launcher")
            {
                Integer = BaseValue;
            }
        }

    }
}