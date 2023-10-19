using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace WS20.P3.Overcrowded
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        #region Private Fields
     
        [Tooltip("The Ui Panel to let the user enter name, connect and play")] [SerializeField]
        private GameObject controlPanel;

        [Tooltip("The UI Label to inform the user that the connection is in progress")] [SerializeField]
        private GameObject progressLabel;

        [SerializeField] private Text roomNameText;

        private string roomName;

        bool isConnecting;

        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField] private byte maxPlayersPerRoom = 6;
        
        string gameVersion = "1";

        private static System.Random random = new System.Random();
        
        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            
            List<GameObject> dontDestroyOnLoadList = GetDontDestroyOnLoadObjects();

            foreach (GameObject go in dontDestroyOnLoadList)
            {
                if (go.name != "PhotonMono" && go.name != "[Debug Updater]")
                {
                    if (go.GetComponent<TaskManager>() || go.GetComponent<SpriteManager>())
                    {
                        Destroy(go.transform.gameObject);
                    }
                }
            }
        }

        void Start()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

        #endregion
        
        #region Photon CallBacks

        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
            if (isConnecting)
            {
                // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
                if (roomNameText.text != "")
                {
                    Debug.LogError("JOIN ROOM BY NAME: " + roomName);
                    PhotonNetwork.JoinRoom(roomName);
                }
                else
                {
                    PhotonNetwork.JoinRandomRoom();
                }
                isConnecting = false;
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            isConnecting = false;

            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            
            const string chars = "ABCDEFGHIKLMNOPQRSTUVWXYZ0123456789";
            roomName = new string(Enumerable.Repeat(chars, 5).Select(s => s[random.Next(s.Length)]).ToArray());
            
            PhotonNetwork.CreateRoom(roomName, new RoomOptions {MaxPlayers = maxPlayersPerRoom, PublishUserId = true});
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            PhotonNetwork.CreateRoom(roomName, new RoomOptions {MaxPlayers = maxPlayersPerRoom, PublishUserId = true});
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");

            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                PhotonNetwork.LoadLevel(1);
            }
            else
            {
                //PhotonNetwork.LoadLevel(1);
                Debug.LogError("More than one Player");
            }
        }

        #endregion

        #region Public Methods

        public void Connect()
        {
            roomName = roomNameText.text;
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);

            if (PhotonNetwork.IsConnected)
            {
                if (roomNameText.text != "")
                {
                    Debug.LogError("JOIN ROOM BY NAME: " + roomName);
                    PhotonNetwork.JoinRoom(roomName);
                }
                else
                {
                    PhotonNetwork.JoinRandomRoom();
                }
            }
            else
            {
                Debug.Log("CONNECT");
                isConnecting = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }

        #endregion
        
        #region Private Methods

        private static List<GameObject> GetDontDestroyOnLoadObjects()
        {
            List<GameObject> result = new List<GameObject>();
            List<GameObject> rootGameObjectsExceptDontDestroyOnLoad = new List<GameObject>();
            
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                rootGameObjectsExceptDontDestroyOnLoad.AddRange(SceneManager.GetSceneAt(i).GetRootGameObjects());
            }

            List<GameObject> rootGameObjects = new List<GameObject>();
            Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
            
            for (int i = 0; i < allTransforms.Length; i++)
            {
                Transform root = allTransforms[i].root;
                if (root.hideFlags == HideFlags.None && !rootGameObjects.Contains(root.gameObject))
                {
                    rootGameObjects.Add(root.gameObject);
                }
            }

            for (int i = 0; i < rootGameObjects.Count; i++)
            {
                if (!rootGameObjectsExceptDontDestroyOnLoad.Contains(rootGameObjects[i]))
                    result.Add(rootGameObjects[i]);
            }
            return result;
        }

        #endregion
    }
}