using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using UnityEditor;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace WS20.P3.Overcrowded
{
    [Serializable]
    struct ButtonTwisterField
    {
        public Sprite unpressed;
        public Sprite pressed;
        public Text textComponent;
        public Image specialButton;
        public char key;
    }

    [RequireComponent(typeof(BoxCollider))]
    [System.Serializable]
    public class ButtonTwister : Task
    {
        [Header("Custom Settings")]

        #region Public Fields

        public GameObject finishedTaskSprite;
        public GameObject originalTaskSprite;

        public Sprite unpressedBackground;
        public Sprite pressedBackground;
        [FormerlySerializedAs("uiLetters_")] public Text[] uiLetters;
        
        public Sprite[] progressSprites;
        public Image[] uiLetterBackgrounds;
        public Image progress;
        
        [Range(1,10)] public int levels;

        

        private bool isSteamDeck =false;
        //public GameObject buttonsGameObject;
        public Image[] specialButtons;
        public List<Sprite> specialSprites;


        #endregion

        #region Private Fields

        private const int AsciiOfA = 'a';
        private int currentProgressLevel;

        [SerializeField]
        private ButtonTwisterField[,] textFields;

        private static char[] _viableKeys = {'b', 'c', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 't', 'u', 'v', 'x', 'y', 'z'};
        private static char[] _viableControllerKeys = {'a', 'b', 'x', 'y', 'q', 'w', 'e', 'r', 'z', 'u'};
        //private static string[] _viableControllerKeys = {"a", "y", "x", "y", "Axis6", "Axis7"};
        private List<char> viableKeys = _viableKeys.ToList();
        private List<char> viableControllerKeys = _viableControllerKeys.ToList();

        #endregion

        #region MonoBehaviour CallBacks

        public void Awake()
        {
            if (isSteamDeck)
            {
                textFields = new ButtonTwisterField[levels,uiLetterBackgrounds.Length];
            }
            else
            {
                textFields = new ButtonTwisterField[levels,uiLetterBackgrounds.Length];
            }
            
            if (SystemInfo.operatingSystem.ToLower().Contains("steamos"))
            {
                isSteamDeck = true;
                Debug.LogError("isSteamDeck");
            }
        }

        public override void Update()
        {
            if (isSteamDeck && isActive)
            {
                bool canProgress = true;
                //buttonsGameObject.SetActive(true);
                Debug.LogError("The application is running on SteamOS.");

                for (int i = 0; i < uiLetterBackgrounds.Length; i++)
                {
                    ButtonTwisterField field = textFields[currentProgressLevel, i];
                    if (field.key == 'q' || field.key == 'w' || field.key == 'e' || field.key == 'r')
                    {
                        if (Input.GetAxis("Axis7") >= 1 && field.key == 'e') //Right
                        {
                            uiLetterBackgrounds[i].sprite = pressedBackground;
                        } else if (Input.GetAxis("Axis7") <= -1f && field.key == 'q') //Left
                        {
                            uiLetterBackgrounds[i].sprite = pressedBackground;
                        } else if (Input.GetAxis("Axis8") >= 1f && field.key == 'r') //Down
                        {
                            uiLetterBackgrounds[i].sprite = pressedBackground;
                        } else if (Input.GetAxis("Axis8") <= -1f && field.key == 'w') //Up
                        {
                            uiLetterBackgrounds[i].sprite = pressedBackground;
                        }
                        else
                        {
                            uiLetterBackgrounds[i].sprite = unpressedBackground;
                            canProgress = false;
                        }
                    } else if (field.textComponent.text == "A" || field.textComponent.text == "B" || field.textComponent.text == "X" || field.textComponent.text == "Y" || field.textComponent.text == "R1" || field.textComponent.text == "L1")
                    {
                        if (Input.GetButton("" + field.textComponent.text))
                        {
                            uiLetterBackgrounds[i].sprite = pressedBackground;
                        }
                        else
                        {
                            uiLetterBackgrounds[i].sprite = unpressedBackground;
                            canProgress = false;
                        }
                        
                    }
                    else
                    {
                        uiLetterBackgrounds[i].sprite = unpressedBackground;
                        canProgress = false;
                    }
                }
                
                if (canProgress)
                {
                    //buttonsGameObject.SetActive(false);
                    progress.sprite = progressSprites[currentProgressLevel++];
                    AudioManager.instance.PlayRandomFromList("PaintWall");
                    if(currentProgressLevel==levels) TaskFinished();
                    else
                    {
                        UpdateSteamdeckDisplay();
                    }
                }

                //return;
            }
            

            
            if (isActive && !isSteamDeck){
                bool canProgress = true;
                for (int i = 0; i < uiLetterBackgrounds.Length; i++)
                {
                    ButtonTwisterField field = textFields[currentProgressLevel, i];
                    if (Input.GetKey("" + field.key))
                    {
                        uiLetterBackgrounds[i].sprite = pressedBackground;
                    }
                    else
                    {
                        uiLetterBackgrounds[i].sprite = unpressedBackground;
                        canProgress = false;
                    }
                }

                if (canProgress)
                {
                    progress.sprite = progressSprites[currentProgressLevel++];
                    AudioManager.instance.PlayRandomFromList("PaintWall");
                    if(currentProgressLevel==levels) TaskFinished();
                    else
                    {
                        for (int i = 0; i < textFields.GetLength(1); i++)
                        {
                            textFields[currentProgressLevel, i].textComponent.text = "" + Char.ToUpper(textFields[currentProgressLevel, i].key);
                        }
                    }
                }
            }

            base.Update();
        }

        #endregion

        #region Public Methods

        public override void Interact()
        {
            if (isSteamDeck)
            {
                //STEAM DECK
                for (int x = 0; x < textFields.GetLength(0); x++)
                {
                    for (int y = 0; y < textFields.GetLength(1); y++)
                    {
                        char temp = viableControllerKeys[Random.Range(0, viableControllerKeys.Count)];
                        //int posInAlphabet = temp - AsciiOfA;
    
                        textFields[x,y].key = temp;
                        textFields[x,y].textComponent = uiLetters[y];
                        textFields[x,y].specialButton = specialButtons[y];
                        Debug.LogError("Remove controller key: " + temp);
                        viableControllerKeys.Remove(temp);
                    }
                }

                UpdateSteamdeckDisplay();
                // for (int i = 0; i < textFields.GetLength(1); i++)
                // {
                //     textFields[currentProgressLevel, i].textComponent.text = "" + GetControllerMapping(textFields[currentProgressLevel, i].key);
                // }
            }
            else
            {
                //PC
                for (int x = 0; x < textFields.GetLength(0); x++)
                {
                    for (int y = 0; y < textFields.GetLength(1); y++)
                    {
                        char temp = viableKeys[Random.Range(0, viableKeys.Count)];
                        int posInAlphabet = temp - AsciiOfA;
        
                        textFields[x,y].key = temp;
                        textFields[x,y].textComponent = uiLetters[y];
                        viableKeys.Remove(temp);
                    }
                } 
                for (int i = 0; i < textFields.GetLength(1); i++)
                {
                    textFields[currentProgressLevel, i].textComponent.text = "" + Char.ToUpper(textFields[currentProgressLevel, i].key);
                }                
            }
            


            base.Interact();
        }

        public void UpdateSteamdeckDisplay()
        {
            for (int i = 0; i < textFields.GetLength(1); i++)
            {
                //textFields[currentProgressLevel, i].textComponent.text = "" +  GetControllerMapping(textFields[currentProgressLevel, i].key);
                var key = GetControllerMapping(textFields[currentProgressLevel, i].key);
                if (key == "Left" || key == "Right" || key == "Up" || key == "Down")
                {
                    textFields[currentProgressLevel, i].textComponent.text = "" + key;
                    textFields[currentProgressLevel, i].textComponent.gameObject.SetActive(false);
                    textFields[currentProgressLevel, i].specialButton.gameObject.SetActive(true);
                    switch (key)
                    {
                        case "Down":
                            textFields[currentProgressLevel, i].specialButton.sprite = specialSprites[0];
                            break;
                        case "Left":
                            textFields[currentProgressLevel, i].specialButton.sprite = specialSprites[1];
                            break;
                        case "Right":
                            textFields[currentProgressLevel, i].specialButton.sprite = specialSprites[2];
                            break;
                        case "Up":
                            textFields[currentProgressLevel, i].specialButton.sprite = specialSprites[3];
                            break;
                    }
                }
                else
                {
                    textFields[currentProgressLevel, i].textComponent.text = "" + key;
                    textFields[currentProgressLevel, i].textComponent.gameObject.SetActive(true);
                    textFields[currentProgressLevel, i].specialButton.gameObject.SetActive(false);
                }
            }
        }
        
        
        public string GetControllerMapping(char key)
        {
            switch (key)
            {
                case 'a':
                    return "A";
                case 'b':
                    return "B";
                case 'x':
                    return "X";
                case 'y':
                    return "Y";
                case 'q':
                    return "Left";
                case 'w':
                    return "Up";
                case 'e':
                    return "Right";
                case 'r':
                    return "Down";
                case 'z':
                    return "R1";
                case 'u':
                    return "L1";
                default:
                    return "Error";
            }
        }
        
        public override void StopTask()
        {
            viableKeys = _viableKeys.ToList();
            viableControllerKeys = _viableControllerKeys.ToList();
            base.StopTask();
        }

        #endregion

        #region Photon RPCs

        [PunRPC]
        protected override void UpdateTask()
        {
            originalTaskSprite.SetActive(false);
            finishedTaskSprite.SetActive(true);
        }

        #endregion
    }
}