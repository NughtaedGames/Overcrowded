using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Random = UnityEngine.Random;

namespace WS20.P3.Overcrowded
{
    public class NpcBehaviour : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Public Fields

        public bool isAngry;
        public bool isExclamating;
        public bool isStuck;
        public bool reachedDestination;

        #endregion

        #region Private Fields

        [SerializeField] private Animator animator;
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private GameObject speechBubble;
        [SerializeField] private GameObject exclamationBubble;

        private Transform target;
        private Vector3 destination;

        private bool isStunned;
        private const float stopDistanceMin = 0.1f;
        private const float stopDistanceMax = 0.3f;
        private float stopDistance;
        private float destinationDistance;

        #endregion

        #region MonoBehaviour CallBacks

        private new void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private new void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        void Update()
        {
            Vector3 currentSpeed = agent.velocity;
            animator.SetFloat("Speed", currentSpeed.magnitude);
            if (currentSpeed.x < -0.2) spriteRenderer.flipX = false;
            else if (currentSpeed.x > 0.2) spriteRenderer.flipX = true;
            
            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }
            
            if (!isStunned && transform.position != destination)
            {
                Vector3 destinationDirection = destination - transform.position;
                    destinationDirection.y = 0;
                    destinationDistance = destinationDirection.magnitude;
                    stopDistance = Random.Range(stopDistanceMin, stopDistanceMax);
                    reachedDestination = destinationDistance < stopDistance;
            }
        }

        #endregion

        #region Public Methods
        
        public IEnumerator CheckForStates()
        {
            if (isAngry) speechBubble.SetActive(true);
            if (isExclamating) exclamationBubble.SetActive(true);

            yield return new WaitForSeconds(3);

            if (isAngry) speechBubble.SetActive(false);
            isAngry = false;

            if (isExclamating) exclamationBubble.SetActive(false);
            isExclamating = false;
        }

        public void SetDestination(Vector3 destination)
        {
            this.destination = destination;
            agent.SetDestination(destination);
            reachedDestination = false;
        }

        
        public IEnumerator StuckReset()
        {
            yield return new WaitForSecondsRealtime(6);
            isStuck = true;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(transform.position);
                stream.SendNext(agent.velocity);
            }
            else
            {
                transform.position = (Vector3) stream.ReceiveNext();
                agent.velocity = (Vector3) stream.ReceiveNext();
            }
        }

        #endregion

        #region Private Regions

        private IEnumerator StunCoroutine(float? duration)
        {
            StartCoroutine(nameof(CheckForStates));

            if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
            {
                yield break;
            }

            isStunned = true;
            agent.SetDestination(this.transform.position);
            if (duration != null)
            {
                yield return new WaitForSeconds((float)duration);
            }
            else
            {
                yield return new WaitForSeconds(Random.Range(3f, 5f));
            }
            isStunned = false;
            agent.SetDestination(destination);
            StopCoroutine(nameof(StuckReset));
        }

        #endregion
        
        #region Photon RPCs

        [PunRPC]
        public void AssignSprite(int index)
        {
            animator.runtimeAnimatorController =
                SpriteManager.SpriteManagerInstance.GetCharacterSprite(index);
        }

        [PunRPC]
        public void Stun(float? duration)
        {
            StartCoroutine(StunCoroutine(duration));
        }

        #endregion
    }
}