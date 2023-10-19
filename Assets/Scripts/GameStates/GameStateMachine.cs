using Photon.Pun;

namespace WS20.P3.Overcrowded
{
    public class GameStateMachine : MonoBehaviourPunCallbacks
    {
        #region Private Fields

        private GameState GameState;

        #endregion

        #region Public Methods

        public void SetState(GameState state)
        {
            if (state != null)
            {
                //if we set a new State we call OnDisable on the old GameState
                if (GameState != null)
                {
                    StartCoroutine(routine: GameState.OnDisable());
                }

                GameState = state;
                StartCoroutine(routine: GameState.Start());
            }
        }

        public GameState GetState() { return GameState; }

        #endregion
    }
}