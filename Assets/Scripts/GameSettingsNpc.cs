using System;
using Photon.Pun;
using UnityEngine;
using WS20.P3.Overcrowded;

[RequireComponent(typeof(SphereCollider))]
public class GameSettingsNpc : MonoBehaviourPunCallbacks
{
    private bool masterClientIsColliding;
    [SerializeField] private GameObject eInteract;
    [SerializeField] private GameObject xInteract;
    bool isSteamDeck = false;

    private void Start()
    {
        if (SystemInfo.operatingSystem.ToLower().Contains("steamos"))
        {
            isSteamDeck = true;
        }
    }

    void Update()
    {
        if (!masterClientIsColliding)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("X"))
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
            if (isSteamDeck)
            {
                eInteract.SetActive(false);
                xInteract.SetActive(true);
            }
            else
            {
                eInteract.SetActive(true);
                xInteract.SetActive(false);
            }
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
            xInteract.SetActive(false);
        }
    }
}
