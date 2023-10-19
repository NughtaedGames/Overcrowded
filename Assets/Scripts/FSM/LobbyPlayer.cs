using System.Collections;

namespace WS20.P3.Overcrowded
{
    public class LobbyPlayer : State
    {
        public LobbyPlayer(PlayerManager playerManager) : base(playerManager) { }

        #region MonoBehaviour CallBacks

        public override IEnumerator SetTask(int task)
        {
            return base.Start();
        }

        #endregion
    }
}