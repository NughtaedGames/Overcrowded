using System.Collections;
using UnityEngine;

namespace WS20.P3.Overcrowded
{
    [System.Serializable]
    public abstract class State
    {
        public State(PlayerManager playerManagerScript) { PlayerManagerScript = playerManagerScript; }
        
        #region Public Fields
        
        public int currentTask;
        public float speed = 3f;

        public bool isGravityActive = true;

        #endregion

        #region Protected Fields

        protected PlayerManager PlayerManagerScript;
        protected Vector3 moveDirection = Vector3.zero;

        #endregion

        #region Private Fields

        private float timer;

        #endregion

        #region MonoBehaviour CallBacks

        public virtual IEnumerator Start() { yield break; }

        public virtual IEnumerator Update()
        {
            PlayerManagerScript.StartCoroutine(Movement());
            yield break;
        }

        public IEnumerator FixedUpdate()
        {
            PlayerManagerScript.StartCoroutine(StepSound());

            yield break;
        }

        #endregion

        #region Public Methods

        public IEnumerator StepSound()
        {
            if (moveDirection != new Vector3(0, 0, 0))
            {
                timer += 0.1f;

                if (AudioManager.instance.isListPlaying("StoneSteps") != true &&
                    AudioManager.instance.isListPlaying("StoneSteps") != null && timer >= 2.5f)
                {
                    AudioManager.instance.PlayRandomFromList("StoneSteps");
                    timer = 0;
                }
            }

            yield break;
        }

        public virtual IEnumerator Movement()
        {
            if (PlayerManagerScript.photonView.IsMine)
            {
                if (PlayerManagerScript.controlsActive || !PlayerManagerScript.isStunned)
                {

                    #region Input Handling

                    //keyboard movement input
                    moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
                    moveDirection.Normalize();
                    moveDirection *= speed;

                    // Move the controller
                    PlayerManagerScript.myController.Move(moveDirection * Time.deltaTime);
                    

                    if (isGravityActive)
                    {
                        PlayerManagerScript.myController.detectCollisions = true;
                        Vector3 playerPosition = PlayerManagerScript.transform.position;
                        
                        Vector3 raycastStart = new Vector3(playerPosition.x,
                            playerPosition.y + 0.2f, playerPosition.z);
                        
                        if (Physics.Raycast(raycastStart, Vector3.down, out RaycastHit hit, 100f))
                        {
                            //Set the target location to the location of the hit.
                            Vector3 targetLocation = hit.point;

                            //Move the object to the target location.
                            PlayerManagerScript.transform.position = targetLocation;
                        }
                    }
                    
                    else
                    {
                        PlayerManagerScript.myController.detectCollisions = false;
                    }
                    
                    #endregion
                }
                else
                {
                    moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
                    moveDirection.Normalize();
                    moveDirection *= speed * 0.7f; // * 100;
                    PlayerManagerScript.myController.Move(moveDirection * Time.deltaTime);
                    float distance = 100f;
                
                    RaycastHit hit;
                
                    Vector3 raycastStart = new Vector3(PlayerManagerScript.transform.position.x,
                        PlayerManagerScript.transform.position.y + 0.2f, PlayerManagerScript.transform.position.z);
                
                    if (Physics.Raycast(raycastStart, Vector3.down, out hit, distance))
                    {
                        //Set the target location to the location of the hit.
                        Vector3 targetLocation = hit.point;
                
                        //Move the object to the target location.
                        PlayerManagerScript.transform.position = targetLocation;
                    }
                }
            }
            yield break;
        }

        public virtual IEnumerator SetTask(int taskID) { yield break; }

        public virtual IEnumerator TaskDone() { yield break; }

        #endregion
    }
}