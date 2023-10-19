using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Audio;

namespace WS20.P3.Overcrowded
{
    [RequireComponent(typeof(SphereCollider))]
    [RequireComponent(typeof(PhotonView))]
    public class GateMechanic : MonoBehaviourPunCallbacks
    {
        #region Public Fields
        
        public Waypoint[] waypoints;
        public float[] waypoints_branchRatio;
        public bool gateIsDown;

        #endregion

        #region Private Fields

        [SerializeField] private GameObject gateUp;
        [SerializeField] private GameObject gateDown;
        [SerializeField] private GameObject interactE;
        
        [SerializeField] private int gateCoolDown;

        private bool playerIsColliding;
        
        #endregion

        #region Monobehaviour Callback

        public void ActivateGate()
        {
            gateUp.SetActive(false);
            gateDown.SetActive(true);
            StartCoroutine("GateCooldown");

            photonView.RPC(nameof(ActivateGateRPC), RpcTarget.AllBuffered);
        }
        
        public virtual void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerManager>() is PlayerManager playerManager && playerManager.isSeeker)
            {
                playerManager.collidingGate = this;
                interactE.SetActive(true);
                playerIsColliding = true;
            }
        }

        public virtual void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<PlayerManager>() is PlayerManager playerManager && playerManager.isSeeker)
            {
                playerManager.collidingGate = null;
                interactE.SetActive(false);
                playerIsColliding = false;
            }
        }

        #endregion
        
        #region Public Methods

        IEnumerator GateCooldown()
        {
            gateIsDown = true;
            yield return new WaitForSeconds(gateCoolDown);
            AudioManager.instance.PlayLocalSound("GateLift", transform.position);
            gateIsDown = false;
            gateUp.SetActive(true);
            gateDown.SetActive(false);
            if (playerIsColliding)
            {
                interactE.SetActive(true);
            }

            if (waypoints.Length != 0)
            {
                for (int i = 0; i < waypoints.Length; i++)
                {
                    waypoints[i].branchRatio = waypoints_branchRatio[i];
                }
            }
        }
                
        #endregion
                
        #region Photon RPCs

        [PunRPC]
        private void ActivateGateRPC()
        {
            gateUp.SetActive(false);
            gateDown.SetActive(true);
            AudioManager.instance.PlayLocalSound("GateDrop", transform.position);
            interactE.SetActive(false);
            if (waypoints.Length != 0)
            {
                waypoints_branchRatio = new float[waypoints.Length];

                for (int i = 0; i < waypoints.Length; i++)
                {
                    Waypoint waypoint = waypoints[i];
                    
                    waypoints_branchRatio[i] = waypoint.branchRatio;
                    waypoint.branchRatio = 0;
                }
            }
            StartCoroutine("GateCooldown");
        }

        #endregion

        
    }
}