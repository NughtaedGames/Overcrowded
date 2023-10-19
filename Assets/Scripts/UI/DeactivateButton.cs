using Photon.Pun;
using UnityEngine;

namespace WS20.P3.Overcrowded
{
    public class DeactivateButton : MonoBehaviour
    {
        private void Awake()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
