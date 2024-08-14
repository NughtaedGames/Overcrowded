using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

namespace WS20.P3.Overcrowded
{
    [RequireComponent(typeof(BoxCollider))]
    [System.Serializable]
    public class ButtonMashing : Task
    {
        [Header("Custom Settings")]

        #region Public Fields

        public Image barImage;

        public float reduceAmount;
        
        #endregion

        #region Private Fields

        [SerializeField] private GameObject statue;
        [SerializeField] private GameObject spaceBarImage;
        [SerializeField] private Sprite buttonPressed;
        [SerializeField] private Sprite buttonNotPressed;
        [SerializeField] private GameObject xBarImage;
        [SerializeField] private Sprite steamdeckButtonPressed;
        [SerializeField] private Sprite steamdeckButtonNotPressed;

        private bool isSteamDeck;
        
        private Hider hider;
        private Coroutine co;
        
        #endregion

        #region MonoBehaviour CallBacks

        public void Start()
        {
            if (SystemInfo.operatingSystem.ToLower().Contains("steamos"))
            {
                isSteamDeck = true;
                Debug.LogError("isSteamDeck");
            }

            if (isSteamDeck)
            {
                xBarImage.SetActive(true);
                spaceBarImage.SetActive(false);
            }
            else
            {
                xBarImage.SetActive(false);
                spaceBarImage.SetActive(true);
            }
        }

        public override void Update()
        {
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("X")) && isActive)
            {
                barImage.fillAmount += 0.1f;
                AudioManager.instance.PlayRandomFromList("ButtonMashing");
                if (isSteamDeck)
                {
                    xBarImage.GetComponent<Image>().sprite = steamdeckButtonPressed;
                }
                else
                {
                    spaceBarImage.GetComponent<Image>().sprite = buttonPressed;
                }
            }
            else if ((Input.GetKeyUp(KeyCode.Space) || Input.GetButtonUp("X")) && isActive)
            {
                if (isSteamDeck)
                {
                    xBarImage.GetComponent<Image>().sprite = steamdeckButtonNotPressed;
                }
                else
                {
                    spaceBarImage.GetComponent<Image>().sprite = buttonNotPressed;
                }
                
            }

            base.Update();
        }

        public override void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerManager>())
            {
                int collidingPlayer = other.GetComponent<PlayerManager>().photonView.ViewID;

                if (collidingPlayer == myPlayer &&
                    collidingPlayer == PlayerManager.LocalPlayerInstance.GetPhotonView().ViewID)
                {
                    if (this.photonView.ViewID == other.gameObject.GetComponent<PlayerManager>().GetState().currentTask)
                    {
                        if (other.GetComponent<PlayerManager>().GetState() is Hider h)
                        {
                            hider = h;
                        }
                    }
                }
            }

            base.OnTriggerEnter(other);
        }

        
        #endregion

        #region Public Methods

        public override void Interact()
        {
            barImage.fillAmount = 0;

            if (hider != null)
            {
                hider.interruptWhistle = true;
                if (co != null)
                {
                    StopCoroutine(co);
                    co = null;
                }
            }

            base.Interact();
        }

        public override void StopTask()
        {
            barImage.fillAmount = 0;
            if (hider != null)
            {
                
                if (co != null)
                {
                    StopCoroutine(co);
                    co = null;
                }
                co = StartCoroutine(nameof(WhistleCooldown));
                
            }

            base.StopTask();
        }
        
        public IEnumerator WhistleCooldown()
        {
            float timePassed = 0;
            float timeToWait = 1;
            
            while (timePassed < timeToWait)
            {
                timePassed += Time.deltaTime;

                yield return null;
            }
            
            hider.interruptWhistle = false;
        }

        public override IEnumerator Timer()
        {
            while (isActive)
            {
                if (barImage.fillAmount >= 1)
                {
                    TaskFinished();
                }
                else
                {
                    barImage.fillAmount -= reduceAmount;
                    yield return new WaitForSeconds(0.01f);
                }
            }
        }

        
        public override void TaskFinished()
        {
            if (hider != null)
            {
                if (co != null)
                {
                    StopCoroutine(co);
                    co = null;
                }
                co = StartCoroutine(nameof(WhistleCooldown));
            }
            base.TaskFinished();
        }

        #endregion

        #region Photon RPCs
        
        [PunRPC]
        protected override void UpdateTask()
        {
            AudioManager.instance.photonView.RPC("PlayRandomFromList", RpcTarget.All, "StatueDrop");

            StartCoroutine(nameof(ShowWarningCo));
            statue.transform.eulerAngles = new Vector3(0, 0, 90);
        }

        private IEnumerator ShowWarningCo()
        {

            Image img = mapPointer.GetComponent<TaskPointerAlertReference>().alertReference;
            
            img.gameObject.SetActive(true);
            mapPointer.SetActive(true);
            mapPointer.GetComponent<Image>().enabled = false;
            yield return new WaitForSeconds(3.25f);
            mapPointer.SetActive(false);
            
        }

        #endregion
    }
}