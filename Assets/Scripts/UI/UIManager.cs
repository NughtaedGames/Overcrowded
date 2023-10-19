using System.Collections;
using UnityEngine;
using UnityEngine.UI;


namespace WS20.P3.Overcrowded
{

    public class UIManager : MonoBehaviour
    {

        public static UIManager Instance;

        [Header("Menu Settings")]
        public GameObject gameSettings;

        [Header("Ingame Settings")]
        [SerializeField]
        private Image taskBar;

        [SerializeField]
        private Text taskBarText;

        public GameObject windowQuestPointer;

        public GameObject gateIcon;

        //[SerializeField]
        public GameObject hiderUI;
        //[SerializeField]
        public GameObject seekerUI;

        public Image progressBar;
        public Image seekerPenaltyProgressBar;

        public GameObject progressBarParticles;

        private void OnEnable()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SetTaskBarText(string s)
        {
            taskBarText.text = s;
        }

        public void ToggleGameSettingsMenu()
        {
            gameSettings.SetActive(!gameSettings.activeSelf);
        }

        public void ActivateUI(string name)
        {
            switch (name)
            {
                case "taskbar":
                    taskBar.gameObject.SetActive(true);
                    break;

                case "hider":
                    hiderUI.gameObject.SetActive(true);
                    seekerUI.gameObject.SetActive(false);
                    gateIcon.gameObject.SetActive(false);
                    break;

                case "seeker":
                    seekerUI.gameObject.SetActive(true);
                    hiderUI.gameObject.SetActive(false);
                    gateIcon.gameObject.SetActive(true);
                    SetTaskBarText("Catch all the hiders before they finish their Tasks");
                    windowQuestPointer.SetActive(false);
                    break;

                default:
                    Debug.LogError(name + " UI does not exist!");
                    break;
            }
        }

        public void DeactivateUI(string name)
        {
            switch (name)
            {
                case "taskbar":
                    taskBar.gameObject.SetActive(false);
                    break;

                case "hider":
                    hiderUI.gameObject.SetActive(false);
                    break;

                case "seeker":
                    seekerUI.gameObject.SetActive(false);
                    break;

                default:
                    Debug.LogError(name + " UI does not exist!");
                    break;
            }
        }

        private IEnumerator StopCooldownCoroutine(float cooldown)
        {

            float timeToWait = cooldown;
            float timePassed = 0.0f;

            Image image = seekerUI.GetComponent<Image>();
            
            image.fillAmount = 0;
            image.color = new Color(0.5f, 0.5f, 0.5f);
            while (timePassed <= timeToWait)
            {
                timePassed += Time.deltaTime;
                image.fillAmount = timePassed / timeToWait;
                yield return null;
            }
            image.color = new Color(1, 1, 1);
        }

        private IEnumerator WhistleCooldownCoroutine(float cooldown)
        {

            float timeToWait = cooldown;
            float timePassed = 0.0f;

            Image image = hiderUI.GetComponent<Image>();
            
            image.fillAmount = 0;
            image.color = new Color(0.5f, 0.5f, 0.5f);
            while (timePassed < timeToWait)
            {
                timePassed += Time.deltaTime;
                image.fillAmount = timePassed / timeToWait;

                yield return null;
            }
            image.color = new Color(1, 1, 1);
        }

        private IEnumerator GateCooldownCoroutine(float cooldown)
        {
            float timeToWait = cooldown;
            float timePassed = 0;

            Image gateIconImage = gateIcon.GetComponent<Image>();
            gateIconImage.fillAmount = 0;
            gateIconImage.color = new Color(0.5f, 0.5f, 0.5f);
            

            while (timePassed <= timeToWait)
            {
                timePassed += Time.deltaTime;
                gateIconImage.fillAmount = timePassed / timeToWait;
                yield return null;
            }
            gateIconImage.color = new Color(1, 1, 1);
        }
    }
}