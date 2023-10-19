using UnityEngine;

namespace WS20.P3.Overcrowded
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "ScriptableObjecs/AnimatedSprite",fileName = "NewAnimatedSprite")]
    public class SOAnimatedSprite : ScriptableObject
    {
        public Sprite sprite;
        public RuntimeAnimatorController animatorController;

        public string name;

        
    }
}