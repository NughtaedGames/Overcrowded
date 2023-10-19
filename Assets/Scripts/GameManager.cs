using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

namespace WS20.P3.Overcrowded
{
    public class GameManager : GameStateMachine
    {
        #region Public Fields

        public static GameManager Instance;

        [SerializeField] private GameObject playerPrefab;
        
        public IntInstance seekerAmount;
        public IntInstance npcAmount;
        public IntInstance diedPlayers;
        
        public GameObject seekerSpawnpoint;
        public GameObject hiderSpawnpoint;

        public Image winLoseMessage;
        
        public GameObject loadingScreen;

        #endregion

        #region Private Fields

        [SerializeField] private GameObject AudioManagerPrefab;
        
        private bool isLoading;
        private Text winLoseText;

        [SerializeField] private Text countdownText;
        [SerializeField] private GameObject countdownObject;
        
        #endregion
        
        #region MonoBehaviour Callbacks

        private void Awake()
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
            SceneManager.sceneLoaded += OnSceneLoaded;
            winLoseText = winLoseMessage.GetComponentInChildren<Text>();
        }

        public override void OnEnable()
        {
            if (playerPrefab != null)
            {
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    var test2 = PhotonNetwork.Instantiate(this.AudioManagerPrefab.name, new Vector3(0f, 0, 0f),
                        Quaternion.identity, 0);
                }
            }
        }

        private void Start()
        {
            if (playerPrefab != null)
            {
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    var test = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 0.4f, 0f),
                        Quaternion.identity, 0);
                }
            }
            else
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
        }

        #endregion
        
        #region Photon Callbacks

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting

            AudioManager.instance.PlayRandomFromList("MessageTones");

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}",
                    PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

                if (PhotonNetwork.CurrentRoom.PlayerCount > 0)
                {
                    if (SceneManager.GetActiveScene().buildIndex != 1)
                    {
                        LoadScene(1);
                    }
                }
            }
        }
        
        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects            

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}",
                    PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
                if (PhotonNetwork.CurrentRoom.PlayerCount < 1)
                {
                    if (SceneManager.GetActiveScene().buildIndex != 0)
                    {
                        LoadScene(0);
                    }
                }
            }
        }

        #endregion
        
        #region Public Methods

        public void StartGame()
        {
            photonView.RPC(nameof(StartGameRPC), RpcTarget.AllBuffered);
        }

        [PunRPC]
        public void StartGameRPC()
        {
            AudioManager.instance.PlayRandomFromList("TaskDone");
            StartCoroutine(StartGameCo());
        }
        
        [PunRPC]
        public void LoadScene(int i)
        {
            if (!isLoading)
            {
                isLoading = true;
                PhotonNetwork.LoadLevel(i);
            }
        }

        public IEnumerator WaitForWinScreen(int sceneID)
        {
            yield return new WaitForSeconds(AudioManager.instance.GetRandomFromList("Win").clip.length-0.75f);
            LoadScene(sceneID);
        }

        [PunRPC]
        public void PlayWinLoseAudio(int winner) //0 for Seeker win, 1 for Hider win
        {
            State state = PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>().GetState();
            if ((state is Seeker && winner == 0) || (state is Hider && winner == 1))
            {
                winLoseMessage.gameObject.SetActive(true);
                winLoseText.text = "You won!";
                AudioManager.instance.PlayRandomFromList("Win");
            }
            else
            {
                winLoseMessage.gameObject.SetActive(true);
                winLoseText.text = "You lost!";
                AudioManager.instance.PlayRandomFromList("Lose");
            }
            if (PhotonNetwork.IsMasterClient) StartCoroutine(nameof(WaitForWinScreen), winner == 0 ? 4:3);
        }
        
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            isLoading = false;
        }


        public void UpdateAmountOfSeekers(int amt)
        {
            photonView.RPC(nameof(UpdateAmountOfSeekersPun), RpcTarget.AllBuffered, amt);
        }

        public void UpdateAmountOfNPCs(int amt)
        {
            photonView.RPC(nameof(UpdateAmountOfNPCsPun), RpcTarget.AllBuffered, amt);
        }

        #endregion

        #region Private Methods

        IEnumerator StartGameCo()
        {
            countdownObject.gameObject.SetActive(true);
            yield return new WaitForSeconds(1.0f);
            countdownText.text = "2";
            yield return new WaitForSeconds(1.0f);
            countdownText.text = "1";
            yield return new WaitForSeconds(1.0f);
            countdownObject.gameObject.SetActive(false);

            loadingScreen.SetActive(true);
            if (PhotonNetwork.IsMasterClient)
            {
                LoadScene(2);
                SetState(new HideAndSeekState(this));
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
            yield return new WaitForSeconds(3f);
            loadingScreen.SetActive(false);
        }

        #endregion

        #region Photon RPCs

        [PunRPC]
        public void UpdateAmountOfSeekersPun(int amt)
        {
            //seekerAmount.Integer = amt;
            PlayerPrefs.SetInt("seekerAmount", amt);
        }

        [PunRPC]
        public void UpdateAmountOfNPCsPun(int amt)
        {
            //npcAmount.Integer = amt;
            PlayerPrefs.SetInt("npcAmount", amt);
        }

        [PunRPC]
        public void StartIntenseMusic()
        {
            StartCoroutine(nameof(StartIntenseMusicCo));
        }

        public IEnumerator StartIntenseMusicCo()
        {
            AudioManager.instance.StopPlayingList("Music");
            AudioSource introSource = AudioManager.instance.GetSoundFromList("Music", "IntenseMusicIntro");
            introSource.Play();

            while (introSource.isPlaying)
            {
                yield return null;
            }
            
            AudioManager.instance.PlaySoundFromList("Music", "IntenseMusic");
        }
        
        #endregion
    }
}