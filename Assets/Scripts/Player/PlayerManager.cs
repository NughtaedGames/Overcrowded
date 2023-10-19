using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace WS20.P3.Overcrowded
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(CameraWork))]
    public class PlayerManager : StateMachine, IOnEventCallback, IPunObservable
    {

        #region Public Fields

        public static GameObject LocalPlayerInstance;

        public CharacterController myController;
        //public RuntimeAnimatorController seekerAnimatedSprite;
        public GameObject whistle;

        public Transform holdUpPosition;

        public List<int> myTasks;
        public Material[] outlineShaders;

        public Vector3 oldPosition;

        public bool isSeeker;
        public bool controlsActive;
        public bool isStunned;
        public bool isBeingCaught;

        //public bool isCollidingWithGate;
        public GateMechanic collidingGate;

        [Tooltip("The maximum distance in which the player can select someone")]
        public float maxTargetDistance;

        public static int SizeID = Shader.PropertyToID("_Magic");
        public static int PlayerPosID = Shader.PropertyToID("_PlayerPosition");
        public static int DistanceThresholdID = Shader.PropertyToID("_DistanceThreshold");
        public static int DistanceThresholdZID = Shader.PropertyToID("_DistanceThresholdZ");

        //public const byte setSeekerEvent = 1;
        public const byte setHiderEvent = 2;
        public const byte getTasksEvent = 3;
        public const byte setRolesEvent = 5;

        public enum Shaders
        {
            Standard,
            Selected,
            Hider,
            Seeker
        }

        #endregion

        #region Private Fields

        [SerializeField] private GameObject shackles;
        [SerializeField] public GameObject area;
        [SerializeField] private GameObject exclamationBubble;
        [SerializeField] private List<Material> materials;
        //[SerializeField] private List<Sprite> sprites;
        [SerializeField] private ParticleSystem ShacklesParticle1;
        [SerializeField] private ParticleSystem ShacklesParticle2;
        [SerializeField] private CameraWork cameraWork;

        //private Rigidbody myRigidBody;
        private List<GameObject> myTaskList;

        private Vector3 movement;
        private Vector3 networkPosition;
        private Vector3 velocity;

        private bool isDead;
        public bool isExclamating;

        [SerializeField] private float materialFade = 0.1f;

        #endregion

        #region MonoBehaviour Callbacks

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
            isSeeker = false;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void Awake()
        {
            if (photonView.IsMine)
            {
                PlayerManager.LocalPlayerInstance = this.gameObject;
            }

            controlsActive = true;
            isStunned = false;

            DontDestroyOnLoad(this.gameObject);
        }

        void Start()
        {
            SetState(new LobbyPlayer(this));
            
            if (cameraWork.cameraTransform == null)
            {
                if (photonView.IsMine)
                {
                    cameraWork.OnStartFollowing();
                }
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (photonView.IsMine)
            {
                cameraWork.OnStartFollowing();
            }
            else
            {
                this.GetComponent<AudioListener>().enabled = false;
            }

        }
        
        void Update()
        {
            velocity = transform.position - oldPosition;
            oldPosition = transform.position;

            // Handling position updates related to the given input
            movement = transform.position - oldPosition;

            if (!photonView.IsMine)
            {
                transform.position = Vector3.MoveTowards(transform.position, networkPosition,
                    Time.deltaTime * GetState().speed);
                return;
            }

            photonView.RPC("UpdateAnimator", RpcTarget.All);

            if (cameraWork.isFollowing)
            {
                cameraWork.Follow();
            }
            
            
            RaycastHit hit;
            
            Camera camera = Camera.main;
            Vector3 cameraPosition = camera.transform.position;
            Vector3 myPosition = transform.position;
            
            if (Physics.Raycast(camera.transform.position,
                this.transform.position + new Vector3(0, 0.7f, 0) - cameraPosition, out hit))
            {
                Debug.DrawLine(cameraPosition, hit.point, Color.red);

                GameObject objectHit = hit.transform.gameObject;

                if (objectHit != gameObject &&
                    objectHit.gameObject.GetComponent<NpcBehaviour>() == null)
                {
                    foreach (Material material in materials)
                    {
                        Vector2 coordinates =
                            camera.WorldToViewportPoint(myPosition +
                                                             new Vector3(0, 0.7f, 0));
                        material.SetFloat(DistanceThresholdID,
                            Vector3.Distance(myPosition, cameraPosition));
                        material.SetFloat(DistanceThresholdZID,
                            Vector3.Distance(new Vector3(0, 0, myPosition.z),
                                new Vector3(0, 0, cameraPosition.z)));
                        material.SetVector(PlayerPosID, coordinates);

                        if (material.GetFloat(SizeID) < 6)
                        {
                            material.SetFloat(SizeID, material.GetFloat(SizeID) + materialFade);
                        }
                    }
                }
                else
                {
                    foreach (Material material in materials)
                    {
                        if (material.GetFloat(SizeID) > 0)
                        {
                            material.SetFloat(SizeID, material.GetFloat(SizeID) - materialFade);
                        }
                    }
                }
            }
            if (State != null) StartCoroutine(State.Update());
        
        }

        private void FixedUpdate()
        {
            if (State != null) StartCoroutine(State.FixedUpdate());
        }

        #endregion

        #region Photon OnEventCallBacks

        public void OnEvent(EventData photonEvent)
        {
            byte eventCode = photonEvent.Code;

            if (eventCode == setRolesEvent)
            {
                if (photonView.IsMine)
                {
                    object[] rolesList = (object[]) photonEvent.CustomData;
                    object[] seekerList = (object[]) rolesList[0];
                    object[] hiderList = (object[]) rolesList[1];

                    foreach (var seeker in seekerList)
                    {
                        if ((string) seeker == PhotonNetwork.LocalPlayer.UserId)
                        {
                            photonView.RPC(nameof(SetSeeker), RpcTarget.All);
                        }
                    }

                    foreach (var hider in hiderList)
                    {
                        if ((string) hider == PhotonNetwork.LocalPlayer.UserId)
                        {
                            int spriteID = SpriteManager.SpriteManagerInstance.GetRandomHiderSpriteIndex();
                            photonView.RPC(nameof(SetHider), RpcTarget.All, spriteID);
                        }
                    }
                }
            }
            else if (eventCode == getTasksEvent)
            {
                object[] data = (object[]) photonEvent.CustomData;

                if ((int) data[0] == this.photonView.ViewID)
                {
                    List<int> newTaskList = new List<int>();

                    for (int i = 1; i < data.Length; i++)
                    {
                        newTaskList.Add(
                            PhotonView.Find((int) data[i]).gameObject.GetComponent<Task>().photonView.ViewID);
                        PhotonView.Find((int) data[i]).gameObject.GetComponent<Task>().myPlayer = (int) data[0];
                    }

                    if (photonView.IsMine)
                    {
                        myTasks = newTaskList;
                        GetState().SetTask(myTasks[0]);
                        UIManager.Instance.ActivateUI("taskbar");
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        public IEnumerator Whistle()
        {
            photonView.RPC("WhistleActivateRPC", RpcTarget.AllBuffered);
            yield return new WaitForSeconds(1);
            photonView.RPC("WhistleDeactivateRPC", RpcTarget.AllBuffered);
        }

        public IEnumerator StopTaskForPlayer()
        {
            isBeingCaught = true;
            yield return new WaitForSeconds(0.5f);
            isBeingCaught = false;
        }
        
        #endregion

        #region Photon RPCs

        [PunRPC]
        public void MovePlayerToStartPosition()
        {
            if (GetState() is Hider) transform.position = GameManager.Instance.hiderSpawnpoint.transform.position;
            if (GetState() is Seeker) transform.position = GameManager.Instance.seekerSpawnpoint.transform.position;
        }

        [PunRPC]
        public void UpdateAnimator()
        {
            GetComponentInChildren<Animator>().SetFloat("Speed", velocity.magnitude);
            if (velocity.x < -0.005) GetComponentInChildren<SpriteRenderer>().flipX = false;
            else if (velocity.x > 0.005) GetComponentInChildren<SpriteRenderer>().flipX = true;
        }

        [PunRPC]
        public void WhistleActivateRPC()
        {
            whistle.SetActive(true);
        }

        [PunRPC]
        public void WhistleDeactivateRPC()
        {
            whistle.SetActive(false);
        }

        [PunRPC]
        public void LiftRPC(bool state)
        {
            GetComponentInChildren<Animator>().SetBool("Lift", state);
        }

        [PunRPC]
        public void SetSeeker()
        {
            SetState(new Seeker(this));
            GetComponentInChildren<SpriteRenderer>().material = outlineShaders[(int) Shaders.Seeker];
            GetComponentInChildren<Animator>().runtimeAnimatorController =
                SpriteManager.SpriteManagerInstance.GetSeekerSprite();

            if (photonView.IsMine)
            {
                isSeeker = true;
            }
        }

        [PunRPC]
        public void SetHider(int spriteID)
        {
            SetState(new Hider(this));

            if (!(PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>().GetState() is Seeker))
            {
                GetComponentInChildren<SpriteRenderer>().material = outlineShaders[(int) Shaders.Hider];
            }

            GetComponentInChildren<Animator>().runtimeAnimatorController =
                SpriteManager.SpriteManagerInstance.GetCharacterSprite(spriteID);

            if (photonView.IsMine)
            {
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions {Receivers = ReceiverGroup.MasterClient};

                PhotonNetwork.RaiseEvent(setHiderEvent, this.photonView.ViewID, raiseEventOptions,
                    SendOptions.SendReliable);
            }
        }

        [PunRPC]
        private void GotCaughtRPC()
        {
            StartCoroutine(nameof(StopTaskForPlayer));
            shackles.SetActive(true);
            isDead = true;
            isStunned = true;
            controlsActive = false;
            ShacklesParticle1.Play();
            ShacklesParticle2.Play();
            if (photonView.IsMine)
            {
                AudioManager.instance.PlayRandomFromList("Caught");
            }
        }

        [PunRPC]
        public void StunPlayerRPC()
        {

            if (!(GetState() is Seeker))
            {
                isExclamating = true;
            }

            if (isExclamating)
            {
                exclamationBubble.SetActive(true);
            }

            StartCoroutine(StunCoroutine(1));
        }

        [PunRPC]
        public void SlowPlayer()
        {
            if (isExclamating)
            {
                exclamationBubble.SetActive(true);
            }

            StartCoroutine(ReactivateBool(controlsActive, 3));
        }

        [PunRPC]
        public void StopPlayerTaskRPC()
        {
            StartCoroutine(nameof(StopTaskForPlayer));
        }
        
        [PunRPC]
        public void DeactivateControls()
        {
            controlsActive = false;
        }

        #endregion

        #region Public Methods

        public void Lift(bool state)
        {
            photonView.RPC(nameof(LiftRPC), RpcTarget.AllBuffered, state);
        }

        public void NextTask(int taskID)
        {
            int pos = myTasks.Count + 1;

            for (int i = 0; i < myTasks.Count; i++)
            {
                if (myTasks[i] == taskID)
                {
                    pos = i;
                    myTasks[i] = -1;
                }
            }

            if (myTasks.Count - 1 > pos)
            {
                ;
                GetState().SetTask(myTasks[pos + 1]);
            }
            else
            {
                GetState().SetTask(-1);
            }
        }

        public void CatchPlayer()
        {
            if (isDead) return;
            photonView.RPC(nameof(DeactivateControls), RpcTarget.AllBuffered);

            this.photonView.RPC(nameof(GotCaughtRPC), RpcTarget.AllBuffered);

            TaskManager.Instance.PlayerDied();
        }

        public void StunPlayer()
        {
            photonView.RPC(nameof(StunPlayerRPC), RpcTarget.AllBuffered);
        }

        public void StopPlayerTask()
        {
            photonView.RPC(nameof(StopPlayerTaskRPC), RpcTarget.AllBuffered);
        }
        
        //Theoretically lag compensation -> doesnt work with character controller, need to calculate velocity or use rigidbody instead
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(transform.position);
                stream.SendNext(movement);
            }
            else
            {
                networkPosition = (Vector3) stream.ReceiveNext();
                movement = (Vector3) stream.ReceiveNext();

                float lag = Mathf.Abs((float) (PhotonNetwork.Time - info.timestamp));
                networkPosition += (movement * lag);
            }
        }

        #endregion

        #region Private Methods

        private IEnumerator ReactivateBool(bool bo, float waitTime)
        {
            controlsActive = false;
            yield return new WaitForSeconds(waitTime);
            controlsActive = true;
        }

        private IEnumerator StunCoroutine(float waitTime)
        {
            isStunned = true;
            yield return new WaitForSeconds(waitTime);
            isStunned = false;
            if (isExclamating)
            {
                exclamationBubble.SetActive(false);
                isExclamating = false;
            }
        }

        #endregion
    }
}