using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace WS20.P3.Overcrowded
{
    public class WaypointNavigator : MonoBehaviour
    {
        #region Public Fields

        public Waypoint currentWaypoint;

        #endregion

        #region Private Fields

        [SerializeField] private NpcBehaviour controller;

        int direction;
        bool shouldStop = false;
        bool CR_running;

        public bool isStunned;

        private Coroutine stuckCoroutine;
        
        #endregion

        #region MonoBehaviour CallBacks

        private void Start()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            direction = Mathf.RoundToInt(Random.Range(0f, 1f));
            controller.SetDestination(currentWaypoint.GetPosition());
        }

        private void Update()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            if (!CR_running) StartCoroutine(SetWaypoint());
        }

        #endregion

        #region Private Methods

        IEnumerator SetWaypoint()
        {
            CR_running = true;
            
            if (controller.reachedDestination)
            {
                //Debug.LogError("NPC: " + this.transform.name + " Set Waypoint with reached destination");
                
                bool shouldBranch = false;
                
                if (currentWaypoint == null)
                {
                    yield break;
                }
                
                if (currentWaypoint.branches != null && currentWaypoint.branches.Count > 0)
                {
                    shouldBranch = Random.Range(0f, 1f) <= currentWaypoint.branchRatio ? true : false;
                }

                if (!shouldBranch)
                {
                    
                    float rnd = Random.Range(0f, 1f);
                    shouldStop = rnd <= currentWaypoint.stopRatio ? true : false;
                    
                    if (shouldStop)
                    {
                        if (PhotonNetwork.IsConnected)
                        {
                            float duration = Random.Range(currentWaypoint.stopDurationmin, currentWaypoint.stopDurationmax);
                            controller.Stun(duration);
                            shouldStop = false;
                            
                            yield return new WaitForSeconds(duration);
                        }
                    }
                }
                else
                {
                    shouldStop = false;
                }
                
                if (shouldBranch)
                {
                    currentWaypoint = currentWaypoint.branches[Random.Range(0, currentWaypoint.branches.Count - 1)];
                }
                else
                {
                    if (direction == 0)
                    {
                        if (currentWaypoint.nextWaypoint != null)
                        {
                            currentWaypoint = currentWaypoint.nextWaypoint;
                        }
                        else
                        {
                            currentWaypoint = currentWaypoint.previousWaypoint;
                            direction = 1;
                        }
                    }
                    else if (direction == 1)
                    {
                        if (currentWaypoint.previousWaypoint != null)
                        {
                            currentWaypoint = currentWaypoint.previousWaypoint;
                        }
                        else
                        {
                            currentWaypoint = currentWaypoint.nextWaypoint;
                            direction = 0;
                        }
                    }
                    
                    
                }



                if (currentWaypoint != null)
                {
                    controller.SetDestination(currentWaypoint.GetPosition());
                    if (stuckCoroutine != null)
                    {
                        controller.StopCoroutine("StuckReset");
                    }
                    stuckCoroutine = controller.StartCoroutine("StuckReset");
                }

            }
            else if (controller.isStuck)
            {
                controller.reachedDestination = true;
                controller.isStuck = false;
            }

            CR_running = false;
            yield break;
        }

        #endregion
    }
}