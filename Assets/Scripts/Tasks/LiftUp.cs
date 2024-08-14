using UnityEngine;
using Photon.Pun;
using UnityEngine.Serialization;

namespace WS20.P3.Overcrowded
{
    [RequireComponent(typeof(BoxCollider))]
    [System.Serializable]
    public class LiftUp : Task
    {
        [Header("Custom Settings")]

        #region Public Fields

        public bool secondPlayerIsColliding;

        public bool secondPlayerHoldingUp;

        public int amountOfMaxSwitches = 10;

        public RectTransform upperButton;
        public RectTransform lowerButton;
        public RectTransform hands;
        public RectTransform flagObject;
        
        #endregion

        #region Private Fields

        [FormerlySerializedAs("flag1")] [SerializeField] private GameObject flag1Object;
        [FormerlySerializedAs("flag2")] [SerializeField] private GameObject flag2Object;
        [FormerlySerializedAs("flag3")] [SerializeField] private GameObject flag3Object;

        [FormerlySerializedAs("rippedFlag")] [SerializeField] private GameObject rippedFlagObject;
        [FormerlySerializedAs("wholeFlag")] [SerializeField] private GameObject wholeFlagObject;
        
        private bool currentSide;
        private int? secondPlayerID;

        private int amountOfCurrentSwitches = 0;
        private PhotonView localPlayerPhotonView;
        
        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            localPlayerPhotonView = PlayerManager.LocalPlayerInstance.GetPhotonView();
        }

        public override void Update()
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            if (moveDirection != new Vector3(0, 0, 0) || (isActive && !secondPlayerHoldingUp))
            {
                StopTask();
            }

            if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("X"))
            {
                if (secondPlayerIsColliding && !playerIsColliding)
                {
                    PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>().Lift(true);
                    int playerID = PlayerManager.LocalPlayerInstance.GetPhotonView().ViewID;
                    this.photonView.RPC(nameof(HoldingUp), RpcTarget.AllBuffered, playerID);
                }
                else if (playerIsColliding && !taskIsFinished && !isActive && secondPlayerHoldingUp) Interact();
            }
            

            if (secondPlayerIsColliding && moveDirection != new Vector3(0, 0, 0))
            {
                PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>().Lift(false);
                this.photonView.RPC(nameof(HoldingUpFalse), RpcTarget.AllBuffered);
            }

            if (true)
            {
                var mousePos = Input.mousePosition;
                mousePos.x -= Screen.width/2;
                mousePos.y -= Screen.height/2;
                
                if (mousePos.y <= upperButton.anchoredPosition.y && mousePos.y >= lowerButton.anchoredPosition.y)
                {

                    float y_strtch = (mousePos.y - upperButton.anchoredPosition.y) / (lowerButton.anchoredPosition.y - upperButton.anchoredPosition.y) * 0.4f + 1f;    
                        
                    Vector3 newScale = new Vector3(1f,
                        y_strtch, 1f);

                    flagObject.localScale = newScale;
                                
                            
                    Vector3 newPos = new Vector3(0, mousePos.y*0.6f, 0);
                    hands.anchoredPosition = newPos;
                }
                
            }
            
            if (playerIsColliding && !taskIsFinished || secondPlayerIsColliding) marker.SetActive(true);
            else marker.SetActive(false);
        }
        
        public override void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerManager>() is PlayerManager playerManager)
            {
                int collidingPlayer = playerManager.photonView.ViewID;

                if (playerManager.GetState() is Hider)
                {
                    if (collidingPlayer == myPlayer && collidingPlayer == localPlayerPhotonView.ViewID && this.photonView.ViewID == playerManager.GetState().currentTask)
                    {
                        playerIsColliding = true;
                    }
                    else if (playerManager.photonView == localPlayerPhotonView)
                    {
                        secondPlayerIsColliding = true;
                    }
                }
            }
        }
        
        public override void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<PlayerManager>() is PlayerManager playerManager)
            {
                int collidingPlayer = playerManager.photonView.ViewID;

                if (playerManager.GetState() is Hider hider)
                {
                    if (collidingPlayer == myPlayer && collidingPlayer == localPlayerPhotonView.ViewID && this.photonView.ViewID == playerManager.GetState().currentTask)
                    {
                        playerIsColliding = false;
                    }
                    else if (playerManager.photonView == localPlayerPhotonView)
                    {
                        secondPlayerIsColliding = false;
                        secondPlayerID = null;
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        public override void Interact()
        {
            Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 5f);
            
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.GetComponent<PlayerManager>() is PlayerManager playerManager)
                {
                    if (secondPlayerID == playerManager.photonView.ViewID)
                    {
                        playerManager.GetState().isGravityActive = false;
                        playerManager.myController.detectCollisions = false;
                        Debug.LogError("Position to go up to: " + PlayerManager.LocalPlayerInstance.transform.position + "Holdup position: " + playerManager.holdUpPosition.position);
                        PlayerManager.LocalPlayerInstance.transform.position = playerManager.holdUpPosition.position;
                    }
                }
            }
            base.Interact();
        }

        [PunRPC]
        protected override void UpdateTask()
        {
            rippedFlagObject.SetActive(true);
            wholeFlagObject.SetActive(false);
        }

        
        public override void TaskFinished()
        {
            PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>().GetState().isGravityActive = true;
            base.TaskFinished();
        }

        public override void StopTask()
        {
            PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>().GetState().isGravityActive = true;
            base.StopTask();
        }

        public void SwitchSide(bool side)
        {
            if (side != currentSide)
            {
                currentSide = side;
                amountOfCurrentSwitches += 1;
                AudioManager.instance.PlayRandomFromList("TearFlag");

                if (amountOfCurrentSwitches > amountOfMaxSwitches / 2)
                {
                    flag1Object.SetActive(false);
                    flag2Object.SetActive(true);
                    if (amountOfCurrentSwitches >= amountOfMaxSwitches)
                    {
                        flag1Object.SetActive(false);
                        flag2Object.SetActive(false);
                        flag3Object.SetActive(true);
                        TaskFinished();
                    }
                }
            }
        }

        #endregion

        #region Photon RPCs

        [PunRPC]
        private void HoldingUp(int ID)
        {
            secondPlayerHoldingUp = true;

            secondPlayerID = ID;
        }

        [PunRPC]
        private void HoldingUpFalse()
        {
            secondPlayerHoldingUp = false;

            secondPlayerID = null;
        }

        #endregion
    }
}