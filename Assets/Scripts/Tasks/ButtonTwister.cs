using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace WS20.P3.Overcrowded
{
    struct ButtonTwisterField
    {
        public Sprite unpressed;
        public Sprite pressed;
        public Text textComponent;
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

        #endregion

        #region Private Fields

        private const int AsciiOfA = 'a';
        private int currentProgressLevel;

        private ButtonTwisterField[,] textFields;

        private static char[] _viableKeys = {'b', 'c', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 't', 'u', 'v', 'x', 'y', 'z'};
        private List<char> viableKeys = _viableKeys.ToList();

        #endregion

        #region MonoBehaviour CallBacks

        public void Awake()
        {
            textFields = new ButtonTwisterField[levels,uiLetterBackgrounds.Length];
        }

        public override void Update()
        {
            if (isActive){
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
            base.Interact();
        }

        public override void StopTask()
        {
            viableKeys = _viableKeys.ToList();
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