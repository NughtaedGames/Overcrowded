using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

namespace WS20.P3.Overcrowded
{
    public class SpriteManager : MonoBehaviour
    {
        #region Public Fields
        
        public static SpriteManager SpriteManagerInstance;
        
        public SOAnimatedSprite[] characterSprites;

        public enum characterType
        {
            Seeker,
            Hider
        }

        #endregion

        #region MonoBehaviour Callbacks

        private void Awake()
        {
            if (SpriteManagerInstance == null)
            {
                SpriteManagerInstance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            for(int i =0;i<characterSprites.Length;i++)
            {
                if (characterSprites[i].name == "Seeker")
                {
                    if (i == 0) break;
                    SOAnimatedSprite temp = characterSprites[0];
                    characterSprites[0] = characterSprites[i];
                    characterSprites[i] = temp;
                }
            }
            //Luca die Schuld geben
            Debug.LogError("Luca ist schuld, ich schwöre.");
        }

        #endregion

        #region Public Methods

        public RuntimeAnimatorController GetSeekerSprite()
        {
            return characterSprites[0].animatorController;
        }

        public int GetRandomHiderSpriteIndex()
        {
            return Random.Range(1, characterSprites.Length - 1);
        }

        public RuntimeAnimatorController GetCharacterSprite(characterType type)
        {
            if (type == characterType.Seeker) return characterSprites[0].animatorController;
            int c = UnityEngine.Random.Range(1, characterSprites.Length);
            return characterSprites[c].animatorController;
        }

        public RuntimeAnimatorController GetCharacterSprite(int index)
        {
            return characterSprites[index].animatorController;
        }
        #endregion
    }
}


