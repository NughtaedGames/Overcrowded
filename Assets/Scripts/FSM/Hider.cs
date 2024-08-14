using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace WS20.P3.Overcrowded
{
    public class Hider : State
    {
        public Hider(PlayerManager playerManager) : base(playerManager) { }
        
        #region Public Fields

        public bool canWhistle = true;
        public bool interruptWhistle = false;

        #endregion

        #region Private Fields

        private bool allTasksDone;
        private bool firstTask = true;
        private int whistleCooldown = 3;
        private bool isSteamDeck;

        #endregion

        #region MonoBehaviour CallBacks

        public override IEnumerator Start()
        {
            PlayerManagerScript.photonView.RPC("MovePlayerToStartPosition", RpcTarget.All);

            yield return new WaitForSeconds(1);

            UIManager.Instance.gameObject.SetActive(true);
            UIManager.Instance.windowQuestPointer.SetActive(true);
            UIManager.Instance.ActivateUI("hider");

            if (SystemInfo.operatingSystem.ToLower().Contains("steamos"))
            {
                isSteamDeck = true;
                Debug.LogError("isSteamDeck");
            }
            
            yield return base.Start();
        }

        public override IEnumerator Update()
        {

            if (Input.GetKeyUp(KeyCode.Space) || (isSteamDeck && Input.GetButtonUp("A")))
            {

                if (interruptWhistle)
                {
                    yield break;
                }
                
                if (canWhistle)
                {
                    canWhistle = false;
                    AudioManager.instance.PlayLocalSound("Whistle", PlayerManagerScript.transform.position);
                    PlayerManagerScript.StartCoroutine("Whistle");
                    
                    UIManager.Instance.StartCoroutine("WhistleCooldownCoroutine", whistleCooldown);

                    float timePassed = 0;
                    
                    while (timePassed < whistleCooldown)
                    {
                        timePassed += Time.deltaTime;
                        yield return null;
                    }

                    canWhistle = true;
                }
            }
            yield return base.Update();
        }

        #endregion

        #region Public Methods


        

        public override IEnumerator SetTask(int task)
        {
            if (firstTask) firstTask = false;
            else TaskManager.Instance.TaskDone();
            
            if (task != -1)
            {
                currentTask = task;
                Task currentTaskPhotonView = PhotonView.Find(currentTask).gameObject.GetComponent<Task>();
                MapTasksList.Instance.DisplayTask(currentTaskPhotonView.mapPointer);
                UIManager.Instance.SetTaskBarText(currentTaskPhotonView.description);
                UIManager.Instance.windowQuestPointer.GetComponent<Window_QuestPointer>().targetPosition = new Vector3(PhotonView.Find(currentTask).gameObject.transform.position.x, 0f, PhotonView.Find(currentTask).gameObject.transform.position.z);
            }
            else
            {
                if (allTasksDone == false)
                {
                    TaskManager.Instance.AllTasksDone();
                    allTasksDone = true;
                }

                currentTask = -1;
                UIManager.Instance.SetTaskBarText("You are done with all your Tasks");
                UIManager.Instance.windowQuestPointer.SetActive(false);
            }
            return base.Start();
        }

        #endregion
    }
}