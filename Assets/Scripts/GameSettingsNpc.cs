using Photon.Pun;
using UnityEngine;
using WS20.P3.Overcrowded;

[RequireComponent(typeof(SphereCollider))]
public class GameSettingsNpc : MonoBehaviourPunCallbacks
{
    private bool masterClientIsColliding;
    [SerializeField] private GameObject eInteract;
    
    
    void Update()
    {
        if (!masterClientIsColliding)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            UIManager.Instance.ToggleGameSettingsMenu();
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.W))
        {
            if (UIManager.Instance.gameSettings.activeSelf)
            {
                UIManager.Instance.gameSettings.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        if (other.GetComponent<PlayerManager>().photonView == PlayerManager.LocalPlayerInstance.GetPhotonView())
        {
            masterClientIsColliding = true;
            eInteract.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        if (other.GetComponent<PlayerManager>().photonView == PlayerManager.LocalPlayerInstance.GetPhotonView())
        {
            masterClientIsColliding = false;
            eInteract.SetActive(false);
        }
    }
}
