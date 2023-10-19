using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace WS20.P3.Overcrowded
{

    public class SettingsMenuManager : MonoBehaviour
    {
        [Header("Menu Settings")] [SerializeField]
        private GameObject settingsWindow;

        [SerializeField] private GameObject gameSettings;
        [SerializeField] private GameObject tutorialWindow;
        [SerializeField] private GameObject soundSettings;

        public Text passwordText;

        private AudioManager audioManager;

        private void Awake()
        {
            
        }

        private void Start()
        {
            audioManager = AudioManager.instance;
            settingsWindow.SetActive(false);
            gameSettings.SetActive(false);
            tutorialWindow.SetActive(false);
            soundSettings.SetActive(false);

            if (PhotonNetwork.IsConnected && passwordText && PhotonNetwork.CurrentRoom != null)
            {
                passwordText.text = PhotonNetwork.CurrentRoom.Name;
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SwitchSettingsMenuState();
            }
        }

        public void SwitchSettingsMenuState()
        {
            settingsWindow.SetActive(!settingsWindow.activeSelf);
            if (settingsWindow.activeSelf == false)
            {
                gameSettings.SetActive(false);
                tutorialWindow.SetActive(false);
                soundSettings.SetActive(false);
            }
        }

        public void PressButtonSound()
        {
            audioManager.PlayRandomFromList("UIButtonPress");
        }

        public void HoverButtonSound()
        {
            if (!audioManager)
            {
                return;
            }
            audioManager.PlayRandomFromList("UIButtonHover");
        }

    }

}
