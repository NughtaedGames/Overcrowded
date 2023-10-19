using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

namespace WS20.P3.Overcrowded
{
    public class TaskManager : MonoBehaviourPunCallbacks
    {
        #region Public Fields

        public GameObject[] SpawnPoints;
        public List<GameObject> taskPrefabs;
        public object[] content;
        public static TaskManager Instance;
        public int tasksPerPlayer = 3;

        #endregion

        #region Private Fields

        [SerializeField] private float seekerPenaltyIncrease;
        [SerializeField] private FloatInstance seekerPenaltyProgress;

        private const byte getTasksEvent = 3;

        private GameObject myTask;

        //private bool areTasksCreated;

        private int diedPlayers = 0;
        private int tasksDone = 0;

        private bool isLoading;
        
        #endregion

        #region MonoBehaviour CallBacks

        private void OnEnable()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }

        #endregion

        #region Public Methods

        public void CreateTaskList(object[] hiderList) // is called on MasterClient by HideAndSeekState only
        {

            if (PhotonNetwork.IsMasterClient) // sicher ist sicher lol
            {
                List<GameObject> spawnpointsToSpawnAt = new List<GameObject>(SpawnPoints);
                List<GameObject> taskPrefabsToSpawn;
                
                reshuffle(spawnpointsToSpawnAt);

                for (int i = 0; i < hiderList.Length; i++)
                {
                    taskPrefabsToSpawn = taskPrefabs;

                    //Check if there is enough Spawnpoints for all Players to get Tasks
                    if (SpawnPoints.Length >= tasksPerPlayer * hiderList.Length)
                    {
                        content = new object[tasksPerPlayer + 1];
                        content[0] = hiderList[i];

                        if (taskPrefabs.Count >= tasksPerPlayer)
                        {
                            reshuffle(taskPrefabsToSpawn);

                            for (int j = 0; j < tasksPerPlayer; j++)
                            {
                                GameObject objToSpawn = GetRandomFromEnumType(taskPrefabsToSpawn[j].GetComponent<Task>().option, spawnpointsToSpawnAt);

                                object[] dataToSpawnWith = new object[1];
                                dataToSpawnWith[0] = MapTasksList.Instance.taskMapPointers.IndexOf(objToSpawn.GetComponent<TaskSpawnpoint>().mapPointer);

                                GameObject spawnedObject =
                                    PhotonNetwork.Instantiate(taskPrefabsToSpawn[j].name, objToSpawn.transform.position,
                                        objToSpawn.transform.rotation, 0, dataToSpawnWith);
                                content[j + 1] = spawnedObject.GetPhotonView().ViewID;
                            }
                        }

                        RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.All};

                        PhotonNetwork.RaiseEvent(getTasksEvent, content, raiseEventOptions, SendOptions.SendReliable);
                    }
                    else
                    {
                        Debug.Log("There is not enough spawnpoints for every Player to get tasks!");
                    }
                }
            }
        }

        public void SeekerClicksNPC()
        {
            this.photonView.RPC(nameof(SeekerClicksNPCPun), RpcTarget.MasterClient);
        }

        public void AllTasksDone()
        {
            this.photonView.RPC(nameof(AllTasksDonePun), RpcTarget.MasterClient);
        }

        public void PlayerDied()
        {
            this.photonView.RPC(nameof(PlayerDiedPun), RpcTarget.MasterClient);
        }

        void reshuffle(List<GameObject> list)
        {
            for (int t = 0; t < list.Count; t++)
            {
                GameObject tmp = list[t];
                int r = Random.Range(t, list.Count);
                list[t] = list[r];
                list[r] = tmp;
            }
        }

        GameObject GetRandomFromEnumType(TasktypesEnum.OPTIONS option, List<GameObject> list)
        {
            GameObject tmp;

            switch (option)
            {
                case TasktypesEnum.OPTIONS.Wall:

                    foreach (var item in list)
                    {
                        if (item.GetComponent<TaskSpawnpoint>().option == TasktypesEnum.OPTIONS.Wall)
                        {
                            tmp = item;
                            list.Remove(item);
                            return tmp;
                        }
                    }

                    return null;

                case TasktypesEnum.OPTIONS.FreeStanding:

                    foreach (var item in list)
                    {
                        if (item.GetComponent<TaskSpawnpoint>().option == TasktypesEnum.OPTIONS.FreeStanding)
                        {
                            tmp = item;
                            list.Remove(item);
                            return tmp;
                        }
                    }

                    return null;

                case TasktypesEnum.OPTIONS.HighWall:

                    foreach (var item in list)
                    {
                        if (item.GetComponent<TaskSpawnpoint>().option == TasktypesEnum.OPTIONS.HighWall)
                        {
                            tmp = item;
                            list.Remove(item);
                            return tmp;
                        }
                    }
                    return null;

                case TasktypesEnum.OPTIONS.Market:
                    foreach (var item in list)
                    {
                        if (item.GetComponent<TaskSpawnpoint>().option == TasktypesEnum.OPTIONS.Market)
                        {
                            tmp = item;
                            list.Remove(item);
                            return tmp;
                        }
                    }
                    return null;
                
                default:
                    return null;
            }
        }

        #endregion

        #region Photon RPCs

        [PunRPC]
        public void AllTasksDonePun()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (GameManager.Instance.GetState() is HideAndSeekState hideAndSeekState)
                {
                    hideAndSeekState.UpdateWinCondition();
                }
            }
        }

        [PunRPC]
        public void SeekerClicksNPCPun()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (GameManager.Instance.GetState() is HideAndSeekState hideAndSeekState)
                {
                    this.photonView.RPC(nameof(IncreaseSeekerPenaltyProgress), RpcTarget.AllBuffered,
                        seekerPenaltyProgress.Float, seekerPenaltyIncrease);
                    this.photonView.RPC(nameof(UpdateSeekerPenaltyProgressBar), RpcTarget.AllBuffered, (float) tasksDone,
                        (float) hideAndSeekState.hiderList.Length, (float) tasksPerPlayer);
                }
            }
        }

        [PunRPC]
        public void IncreaseSeekerPenaltyProgress(float progress, float increase)
        {
            seekerPenaltyProgress.Float = progress + increase;
        }

        public void TaskDone()
        {
            this.photonView.RPC(nameof(TaskDonePun), RpcTarget.MasterClient);
        }

        [PunRPC]
        public void TaskDonePun()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                tasksDone++;
                if (GameManager.Instance.GetState() is HideAndSeekState hState)
                {
                    this.photonView.RPC(nameof(UpdateProgressBar), RpcTarget.AllBuffered, (float) tasksDone,
                        (float) hState.hiderList.Length, (float) tasksPerPlayer);
                    this.photonView.RPC(nameof(UpdateSeekerPenaltyProgressBar), RpcTarget.AllBuffered, (float)tasksDone,
                        (float)hState.hiderList.Length, (float)tasksPerPlayer);
                }
            }
        }

        [PunRPC]
        public void UpdateProgressBar(float tDone, float hiders, float tPerPlayer)
        {
            UIManager.Instance.progressBar.fillAmount = tDone / tPerPlayer / hiders;

            if (GameManager.Instance.GetState() is HideAndSeekState hideAndSeekState)
            {
                hideAndSeekState.UpdateWinCondition();
            }
        }

        [PunRPC]
        public void UpdateSeekerPenaltyProgressBar(float tDone, float hiders, float tPerPlayer)
        {
            UIManager.Instance.seekerPenaltyProgressBar.fillAmount = tDone / tPerPlayer / hiders + seekerPenaltyProgress.Float;
            GameObject particles = UIManager.Instance.progressBarParticles;
            Vector3 particlePos = particles.transform.localPosition;
            particlePos.x = (UIManager.Instance.progressBar.rectTransform.rect.width * UIManager.Instance.seekerPenaltyProgressBar.fillAmount) - UIManager.Instance.progressBar.rectTransform.rect.width/2;
            particles.transform.localPosition = particlePos;
            particles.GetComponent<ParticleSystem>().Play();
            
            if (GameManager.Instance.GetState() is HideAndSeekState hideAndSeekState)
            {
                hideAndSeekState.UpdateWinCondition();
            }
        }

        [PunRPC]
        public void PlayerDiedPun()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            
            GameManager.Instance.diedPlayers.Integer++;
                
            if (GameManager.Instance.GetState() is HideAndSeekState hideAndSeekState)
            {
                hideAndSeekState.UpdateWinCondition();
            }
        }


        #endregion
    }
}