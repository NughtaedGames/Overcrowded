using System.Collections;
using UnityEngine;
using Photon.Pun;
using Random = UnityEngine.Random;

namespace WS20.P3.Overcrowded
{
    public class npcSpawner : MonoBehaviour
    {
        #region Private Fields

        [SerializeField] private GameObject npcPrefab;

        private int amountOfNPCSprites;

        #endregion
        
        #region Public Fields

        public IntInstance amountOfNPCs;

        #endregion
        
        #region MonoBehaviour CallBacks

        void Start()
        {
            StartCoroutine(Spawn());
        }

        private void Awake()
        {
            amountOfNPCSprites = SpriteManager.SpriteManagerInstance.characterSprites.Length - 1;
        }

        #endregion

        #region Private Methods

        private IEnumerator Spawn()
        {
            if (!PhotonNetwork.IsConnected)
            {
                yield break;
            }

            // second if statement needed, so NPC behaviour doesn't break
            if (PhotonNetwork.IsMasterClient)
            {
                float count = 0;
                while (count < PlayerPrefs.GetInt("npcAmount"))
                //while (count < amountOfNPCs.Integer) //npcAmountToSpawn
                {
                    GameObject obj;
                    Transform child = PickRandomSpawn();

                    //Calculates the index of the NPC sprite in Spritemanager.characterSprites-List, for an even distribution
                    float floaty = (count / PlayerPrefs.GetInt("npcAmount")) * amountOfNPCSprites + 1;
                    //float floaty = (count / amountOfNPCs.Integer) * amountOfNPCSprites + 1;
                    int floaty2 = (int) floaty;

                    if (PhotonNetwork.IsConnected)
                    {
                        obj = PhotonNetwork.Instantiate(npcPrefab.name, child.position, new Quaternion(0, 0, 0, 0));
                        obj.GetComponent<PhotonView>().RPC("AssignSprite", RpcTarget.All, floaty2);
                    }
                    else
                    {
                        obj = Instantiate(npcPrefab);
                        obj.GetComponent<NpcBehaviour>().AssignSprite(floaty2);
                    }

                    obj.GetComponent<WaypointNavigator>().currentWaypoint = child.GetComponent<Waypoint>();

                    yield return new WaitForEndOfFrame();

                    count++;
                }
            }
        }

        Transform PickRandomSpawn()
        {
            bool isSpawned = false;
            GameObject waypointObject;

            while (isSpawned == false)
            {
                int random = Random.Range(0, transform.childCount - 1);
                waypointObject = transform.GetChild(random).gameObject;
                Waypoint waypoint = waypointObject.GetComponent<Waypoint>();

                if (waypoint.spawnAble && !waypoint.hasBeenSpawnedOn)
                {
                    if (!waypoint.canMultiSpawn)
                    {
                        waypoint.hasBeenSpawnedOn = true;
                    }
                    return waypointObject.transform;
                }
            }

            return transform.GetChild(0);
        }

        #endregion
    }
}