using System.Collections;
using UnityEngine;
using Photon.Pun;

namespace WS20.P3.Overcrowded
{
    [RequireComponent(typeof(BoxCollider))]
    [System.Serializable]
    [SelectionBase]
    public class Task : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
    {
        [Header("General Settings")]

        #region Public Fields

        public GameObject mapPointer;
        public GameObject progressBar;
        public GameObject marker;
        public GameObject xMarker;

        public bool playerIsColliding;
        public bool taskIsFinished;
        public int myPlayer;
        public string description;

        public TasktypesEnum.OPTIONS option;
        public bool isActive;
        
        #endregion

        #region Private Fields

        #endregion
        
        #region Protected Fields

        protected Vector3 moveDirection;

        #endregion

        #region Monobehaviour Callbacks

        public virtual void Update()
        {
            if (isActive)
            {
                moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
                if (moveDirection != new Vector3(0, 0, 0) || PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>().isBeingCaught)
                {
                    StopTask();
                    PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>().isBeingCaught = false;
                }
            }

            if ((Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("X")) && playerIsColliding && !taskIsFinished && !isActive)
            {
                Interact();
            }

            if (playerIsColliding && !taskIsFinished)
            {
                if (SystemInfo.operatingSystem.ToLower().Contains("steamos"))
                {
                    xMarker.SetActive(true);
                }
                else
                {
                    marker.SetActive(true);
                }
                
            }
            else
            {
                marker.SetActive(false);
                xMarker.SetActive(false);
            }
        }

        public virtual void OnTriggerEnter(Collider other)
        {
            if (!other.GetComponent<PlayerManager>())
            {
                return;
            }

            int collidingPlayer = other.GetComponent<PlayerManager>().photonView.ViewID;

            if (collidingPlayer == myPlayer && collidingPlayer == PlayerManager.LocalPlayerInstance.GetPhotonView().ViewID)
            {
                if (this.photonView.ViewID == other.gameObject.GetComponent<PlayerManager>().GetState().currentTask)
                    playerIsColliding = true;
            }
        }

        public virtual void OnTriggerExit(Collider other)
        {
            if (!other.GetComponent<PlayerManager>())
            {
                return;
            }

            int collidingPlayer = other.GetComponent<PlayerManager>().photonView.ViewID;

            if (collidingPlayer == myPlayer && collidingPlayer == PlayerManager.LocalPlayerInstance.GetPhotonView().ViewID)
                playerIsColliding = false;
        }

        #endregion

        #region Photon CallBacks

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            object[] instantiationData = info.photonView.InstantiationData;
            mapPointer = MapTasksList.Instance.taskMapPointers[(int) instantiationData[0]];
            mapPointer.SetActive(false);
        }

        #endregion

        #region Public Methods

        public virtual void Interact()
        {
            isActive = true;
            progressBar.SetActive(true);
            StartCoroutine(nameof(Timer));
        }

        public virtual void StopTask()
        {
            progressBar.SetActive(false);
            isActive = false;
        }

        [ContextMenu("Finish Task")]
        public virtual void TaskFinished()
        {
            
            taskIsFinished = true;
            PhotonView.Find(myPlayer).gameObject.GetComponent<PlayerManager>().NextTask(this.photonView.ViewID);
            this.photonView.RPC(nameof(UpdateTask), RpcTarget.AllBuffered);
            AudioManager.instance.PlayRandomFromList("TaskDone");
            isActive = false;
            progressBar.SetActive(false);
        }
        
        public virtual IEnumerator Timer() { yield break; }

        protected virtual void UpdateTask() { }
        
        #endregion
    }
}