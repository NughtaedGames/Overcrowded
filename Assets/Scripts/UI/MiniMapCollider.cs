using UnityEngine;
using Photon.Pun;

namespace WS20.P3.Overcrowded
{
    public class MiniMapCollider : MonoBehaviour
    {
        [SerializeField]
        GameObject myMapPoint;

        void OnTriggerEnter(Collider target)
        {
            if (target.GetComponent<PlayerManager>())
            {
                if (target.GetComponent<PlayerManager>().photonView == PlayerManager.LocalPlayerInstance.GetPhotonView())
                {
                    myMapPoint.SetActive(true);
                }
            }
        }
        void OnTriggerExit(Collider target)
        {
            if (target.GetComponent<PlayerManager>())
            {
                if (target.GetComponent<PlayerManager>().photonView == PlayerManager.LocalPlayerInstance.GetPhotonView())
                {
                    myMapPoint.SetActive(false);
                }
                

            }
        }
    }
}

