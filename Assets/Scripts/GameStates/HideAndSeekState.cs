using System.Collections;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace WS20.P3.Overcrowded
{
    public class HideAndSeekState : GameState, IOnEventCallback
    {
        public HideAndSeekState(GameManager gameManager) : base(gameManager) { }
        
        #region Public Fields

        public Player[] playerList;
        public object[] seekerList;
        public object[] hiderList;
        public object[] hiderPhotonViewIDList;

        public const byte setRolesEvent = 5;
        public const byte setHiderEvent = 2;

        public bool isPlayingIntenseMusic;
        
        #endregion

        #region Private Fields

        private int amountOfSeekers;

        #endregion

        #region MonoBehaviour CallBacks
        
        
        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        public override IEnumerator OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
            yield break;
        }

        public override IEnumerator Start()
        {
            OnEnable();

            amountOfSeekers = PlayerPrefs.GetInt("seekerAmount"); //GameManagerScript.seekerAmount.Integer;

            if (PhotonNetwork.IsMasterClient && GameManagerScript.photonView.IsMine)
            {
                
                playerList = new Player[PhotonNetwork.CurrentRoom.PlayerCount];
                seekerList = new object[amountOfSeekers];
                hiderList = new object[PhotonNetwork.CurrentRoom.PlayerCount - amountOfSeekers];
                
                if (playerList.Length < amountOfSeekers)
                {
                    Debug.Log("There are less players than the amount of Seekers");
                }

                for (int k = 0; k < PhotonNetwork.CurrentRoom.PlayerCount; k++)
                {
                    playerList[k] = PhotonNetwork.PlayerList[k];
                }

                int j = 0;
                while(j < amountOfSeekers)
                {
                    int randomPlayer = Random.Range(0, PhotonNetwork.CurrentRoom.PlayerCount);
                    if (!seekerList.Contains(playerList[randomPlayer].UserId))
                    {
                        seekerList[j++] = playerList[randomPlayer].UserId;
                    }
                }
                Debug.LogError("Klaudia ist schuld!");
                
                //Rest of players are hider
                for (int i = 0; i < playerList.Length; i++)
                {
                    if (!seekerList.Contains(playerList[i].UserId))
                    {
                        for (int l = 0; l < hiderList.Length; l++)
                        {
                            if (hiderList[l] == null)
                            {
                                hiderList[l] = playerList[i].UserId;
                                break;
                            }
                        }
                    }
                }

                object[] sendRolesEventData = new object[2];
                sendRolesEventData[0] = seekerList;
                sendRolesEventData[1] = hiderList;
                
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};
                PhotonNetwork.RaiseEvent(setRolesEvent, sendRolesEventData, raiseEventOptions, SendOptions.SendReliable);

                yield return new WaitForSeconds(1f);
            }
            
        }
        
        #endregion

        #region Public Methods

        public void UpdateWinCondition()
        {

            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            if (GameManagerScript.diedPlayers.Integer == PhotonNetwork.PlayerList.Length - PlayerPrefs.GetInt("seekerAmount")) //GameManager.Instance.seekerAmount.Integer)
            {
                GameManagerScript.photonView.RPC(nameof(GameManager.PlayWinLoseAudio), RpcTarget.All, 0);
            }
        
            //Task + Penalty Win Condition
            
            if (UIManager.Instance.seekerPenaltyProgressBar.fillAmount >= 0.75)
            {
                if (!isPlayingIntenseMusic)
                {
                    isPlayingIntenseMusic = true;
                    GameManagerScript.photonView.RPC(nameof(GameManagerScript.StartIntenseMusic), RpcTarget.AllBuffered);
                }
                
                
            }
            
            if (UIManager.Instance.seekerPenaltyProgressBar.fillAmount >= 1)
            {
                GameManagerScript.photonView.RPC(nameof(GameManager.PlayWinLoseAudio), RpcTarget.All, 1);
            }
        }

        #endregion

        #region Photon OnEventCallbacks
        
        public void OnEvent(EventData photonEvent)
        {
            byte eventCode = photonEvent.Code;
            
            if (!GameManagerScript) return;
            
            if (eventCode == setHiderEvent)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    if (GameManagerScript.photonView.IsMine)
                    {
                        int photonViewIDToAdd = (int) photonEvent.CustomData;

                        if (hiderPhotonViewIDList == null)
                        {
                            hiderPhotonViewIDList = new object[PhotonNetwork.CurrentRoom.PlayerCount - amountOfSeekers];
                        }

                        for (int i = 0; i < hiderPhotonViewIDList.Length; i++)
                        {
                            if (hiderPhotonViewIDList[i] == null)
                            {
                                hiderPhotonViewIDList[i] = photonViewIDToAdd;
                                break;
                            }
                        }

                        foreach (var hider in hiderPhotonViewIDList)
                        {
                            if (hider == null) return;
                        }
                        
                        TaskManager.Instance.CreateTaskList(hiderPhotonViewIDList);
                    }
                }
            }
        }
        
        #endregion
    }
}