using Photon.Pun;

namespace WS20.P3.Overcrowded
{
    public abstract class StateMachine : MonoBehaviourPunCallbacks
    {
        #region Protected Fields

        protected State State;

        #endregion

        #region Public Methods

        public void SetState(State state)
        {
            State = state;
            if (photonView.IsMine) StartCoroutine(routine: State.Start());
        }

        public State GetState() { return State; }

        #endregion
    }
}