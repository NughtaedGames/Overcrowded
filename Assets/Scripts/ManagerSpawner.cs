using UnityEngine;
using Photon.Pun;

namespace WS20.P3.Overcrowded
{
    public class ManagerSpawner : MonoBehaviour
    {
        #region Public Fields

        public GameObject spawnedObject;

        #endregion

        #region Private Fields
        
        [SerializeField] private GameObject objectToSpawn;

        #endregion

        #region MonoBehaviour CallBacks

        private void Awake()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                spawnedObject = PhotonNetwork.Instantiate(objectToSpawn.name, new Vector3(0,0,0), Quaternion.identity); //Instantiate(objectToSpawn);
            }
        }

        #endregion

        #region Public Methods

        public void PlaySpawnedObject()
        {
            spawnedObject.GetComponent<GameManager>().StartGame();
        }

        #endregion
    }
}