using System.Collections.Generic;
using UnityEngine;

namespace WS20.P3.Overcrowded
{
    public class Waypoint : MonoBehaviour
    {
        #region Public Fields
        
        public List<Waypoint> branches;
        public Waypoint previousWaypoint;
        public Waypoint nextWaypoint;

        public bool showGizmoAlways;
        public bool spawnAble = true;
        public bool canMultiSpawn = true;
        [HideInInspector]
        public bool hasBeenSpawnedOn = false;
        
        public int stopCount = 0;
        public int stopMax = 3;

        [Range(0f, 1f)] public float branchRatio = 0.5f;
        [Range(0f, 1f)] public float stopRatio;
        public float stopDurationmin = 2f;
        public float stopDurationmax = 4f;
        
        [Range(0, 5)] public float width = 1f;
        public float radius = 5f;
        
        #endregion

        #region Private Fields

        private bool ratioReset;
        private float ratioBackup;

        #endregion
        
        void Update()
        {
            if (stopCount >= stopMax && ratioReset == false)
            {
                ratioReset = true;
                ratioBackup = stopRatio;
                stopRatio = 0f;
            }
            else if (stopCount < stopMax && ratioReset == true)
            {
                ratioReset = false;
                stopRatio = ratioBackup;
            }
        }

        public Vector3 GetPosition()
        {
            Vector2 sphere = (Vector3)(radius * Random.insideUnitCircle);
            Vector3 returnVector = transform.position;
            
            returnVector.x += sphere.x;
            returnVector.z += sphere.y;
            
            return returnVector;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("NPC")) stopCount++;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "NPC")
            {
                stopCount--;
            }
        }
    }
}