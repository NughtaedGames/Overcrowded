using UnityEngine.Audio;
using UnityEngine;

namespace WS20.P3.Overcrowded
{
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        public AudioMixerGroup audioMixerGroup;
        
        [Range(0f, 1f)]
        public float volume;
        [Range(.1f, 3f)]
        public float pitch;

        public bool loop;

        public float minDistance = 1;
        public float maxDistance = 500;

        [HideInInspector]
        public AudioSource source;

    }
}

