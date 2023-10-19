using System.Collections;

namespace WS20.P3.Overcrowded
{
    [System.Serializable]
    public abstract class GameState
    {
        public GameState(GameManager gameManagerScript) { GameManagerScript = gameManagerScript; }
        
        #region Protected Fields

        protected GameManager GameManagerScript;

        #endregion

        #region Public Methods
        
        public virtual IEnumerator Start() { yield break; }

        public virtual IEnumerator Update() { yield break; }

        public virtual IEnumerator OnDisable() { yield break; }

        #endregion
    }
}