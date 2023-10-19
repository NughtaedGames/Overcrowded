using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace WS20.P3.Overcrowded
{
    public class Seeker : State
    {
        public Seeker(PlayerManager playerManager) : base(playerManager) { }
        
        #region Private Fields
        
        private const float radius = 2;
        private GameObject hitTarget;
        
        private bool canUseSpace = true;
        private bool canUseE = true;
        
        private const float spaceCooldown = 10;
        private const float eCooldown = 20;

        #endregion

        #region MonoBehaviour CallBacks
        
        public override IEnumerator Start()
        {
            PlayerManagerScript.photonView.RPC("MovePlayerToStartPosition", RpcTarget.All);
            UIManager.Instance.ActivateUI("seeker");

            yield break;
        }

        public override IEnumerator Update()
        {
            if (PlayerManagerScript.isStunned) yield break;

            #region Raycast Check
            
            LayerMask mask = LayerMask.GetMask("Selectable");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            Material standardShader = 
                PlayerManagerScript.outlineShaders[(int) PlayerManager.Shaders.Standard];
            Material selectedShader = PlayerManagerScript.outlineShaders[(int) PlayerManager.Shaders.Selected];
            
            if (Physics.Raycast(ray, out RaycastHit hit, 50f, mask))
            {
                Debug.DrawLine(ray.origin, hit.point, Color.red);
                GameObject objectHit = hit.transform.gameObject;

                if (objectHit.gameObject != PlayerManagerScript.gameObject)
                {
                    if (Vector3.Distance(objectHit.gameObject.transform.position, PlayerManagerScript.gameObject.transform.position) <= PlayerManagerScript.maxTargetDistance)
                    {
                        if (objectHit.CompareTag("Player") || objectHit.CompareTag("NPC"))
                        {
                            if (hitTarget != objectHit && hitTarget != null)
                            {
                                hitTarget.GetComponentInChildren<SpriteRenderer>().material = standardShader;
                            }
                            hitTarget = objectHit;
                            objectHit.GetComponentInChildren<SpriteRenderer>().material = selectedShader;
                            
                            
                            
                            if (Input.GetMouseButtonUp(0))
                            {
                                
                                if (hitTarget.GetComponent<PlayerManager>() is PlayerManager player && !(player.GetState() is Seeker))  //hitTarget.tag == "Player" && !(hitTarget.GetComponent<PlayerManager>().GetState() is Seeker )) // hitTarget.GetComponent<CharacterControllerScript>().GetState()
                                {
                                    AudioManager.instance.PlayRandomFromList("Caught");
                                    player.CatchPlayer();     
                                }
                                else if (hitTarget.GetComponent<NpcBehaviour>() is NpcBehaviour npc)
                                {
                                    //npcBehaviour npc = hitTarget.GetComponent<npcBehaviour>();
                                    TaskManager.Instance.SeekerClicksNPC();
                                    AudioManager.instance.PlayRandomFromList("Angry");
                                    npc.isAngry = true;
                                    npc.StartCoroutine(nameof(NpcBehaviour.CheckForStates));
                                    npc.photonView.RPC("Stun", RpcTarget.MasterClient, (float?)null);
                                    PlayerManagerScript.StunPlayer();
                                }
                            }
                        }
                    }
                }
            }
            else if(hitTarget != null)
            {
                hitTarget.GetComponentInChildren<SpriteRenderer>().material = standardShader;
                hitTarget = null;
            }

            #endregion
            

            if (canUseSpace)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    PlayerManagerScript.area.SetActive(true);
                    PlayerManagerScript.area.transform.SetParent(null);
                    PlayerManagerScript.area.transform.localScale = new Vector3(radius*2, radius*2, 1f);
                    PlayerManagerScript.area.transform.SetParent(PlayerManagerScript.gameObject.transform);
                    PlayerManagerScript.area.transform.eulerAngles = new Vector3(90f, 0f, 0f);
                }
                //else PlayerManagerScript.area.SetActive(false);
                
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    canUseSpace = false;
                    StunSurrounding();
                    PlayerManagerScript.area.SetActive(false);
                                      
                    UIManager.Instance.StartCoroutine("StopCooldownCoroutine", spaceCooldown);

                    float timePassed = 0;
                    
                    while (timePassed < spaceCooldown)
                    {
                        timePassed += Time.deltaTime;
                        yield return null;
                    }
                    
                    canUseSpace = true;
                }
            }

            if (canUseE && PlayerManagerScript.collidingGate != null)
            {
                if (Input.GetKeyDown(KeyCode.E) && !PlayerManagerScript.collidingGate.gateIsDown)
                {
                    canUseE = false;
                    UIManager.Instance.StartCoroutine("GateCooldownCoroutine", eCooldown);
                    PlayerManagerScript.collidingGate.ActivateGate();
                    
                    float timePassed = 0;
                    
                    while (timePassed < eCooldown)
                    {
                        timePassed += Time.deltaTime;
                        yield return null;
                    }

                    canUseE = true;
                }
            }
            yield return base.Update();
        }

        #endregion
        
        #region Private Methods
        
        private void StunSurrounding()
        {
            Vector3 center = PlayerManager.LocalPlayerInstance.transform.position;
            Collider[] hitColliders = Physics.OverlapSphere(center, radius);
            
            foreach (var hitCollider in hitColliders)
            {

                if (hitCollider.GetComponent<NpcBehaviour>() is NpcBehaviour npc)
                {
                    npc.isExclamating = true;
                    if (AudioManager.instance.isListPlaying("HALT") == false)
                    {
                        AudioManager.instance.PlayRandomFromList("HALT");
                    }
                    npc.StartCoroutine(nameof(NpcBehaviour.CheckForStates));
                    npc.photonView.RPC("Stun", RpcTarget.MasterClient, (float?)null);                    
                }

                if (hitCollider.GetComponent<PlayerManager>() is PlayerManager playerManager)
                {
                    if (playerManager.GetState() is Hider)
                    {
                        playerManager.StopPlayerTask();
                    }
                }
            }
        }
        

        #endregion
    }
}