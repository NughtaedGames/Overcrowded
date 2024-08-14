using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace WS20.P3.Overcrowded
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class ChickenBehaviour : MonoBehaviour
    {

        #region Public Fields

        public float wanderRadius;
        public float wanderTimer;
        [Range(0,5)] public float stopRatio;
        public float minStopTime = 1f;
        public float maxStopTime = 3f;

        #endregion

        #region Private Fields

        private Transform target;
        private NavMeshAgent agent;
        private float timer;
        private bool isStopping;
        private Animator m_Animator;
        private Vector3 directionScale;

        private Vector3 newPos;

        #endregion

        #region MonoBehaviour Callbacks

        void OnEnable () {
            m_Animator = gameObject.GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent> ();
            timer = wanderTimer;
        }

        void Update () {
            timer += Time.deltaTime;

            if (timer >= wanderTimer && !isStopping)
            {
                newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                agent.SetDestination(newPos);
                timer = 0;
                
                if ((Random.Range(0f,5f) <= stopRatio) && !isStopping)
                {
                    StartCoroutine(nameof(StopTimeCoroutine));
                }
            }

            if (agent.desiredVelocity.x > 0.1 && !isStopping)
            {
                
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (agent.desiredVelocity.x < -0.1 && !isStopping)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }        

        #endregion


        #region Private Methods

        private IEnumerator StopTimeCoroutine()
        {
            isStopping = true;
            m_Animator.SetBool("stop", true);
            agent.SetDestination(transform.position);
            yield return new WaitForSeconds(Random.Range(minStopTime, maxStopTime));
            agent.SetDestination(newPos);
            isStopping = false;
            m_Animator.SetBool("stop", false);
        }
        
        private static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask) {
            Vector3 randDirection = Random.insideUnitSphere * dist;
 
            randDirection += origin;
 
            NavMeshHit navHit;
 
            NavMesh.SamplePosition (randDirection, out navHit, dist, layermask);
 
            return navHit.position;
        }        

        #endregion
    }
}