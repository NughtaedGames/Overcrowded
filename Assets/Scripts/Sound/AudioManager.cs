using UnityEngine.Audio;
using UnityEngine;
using System.Collections.Generic;

using Photon.Pun;
using System.Collections;
using Random = UnityEngine.Random;

namespace WS20.P3.Overcrowded
{

    public class AudioManager : MonoBehaviourPunCallbacks
    {
        #region Public Fields
        
        [SerializeField] AudioMixer audioMixer;

        public FloatInstance masterVolume;
        
        public static AudioManager instance;

        public bool hasOnDestroyOnLoad = true;

        [Space(20)]
        public List<SoundList> soundLists = new List<SoundList>();

        #endregion

        #region Private Fields

        [Header("Volumes")]
        [SerializeField] private FloatInstance musicVolume;
        [SerializeField] private FloatInstance ambienceVolume;
        [SerializeField] private FloatInstance sfxVolume;
        #endregion


        #region Monobehaviour Callbacks
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            if (hasOnDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }

            foreach (SoundList list in soundLists)
            {
                foreach (Sound s in list.sounds)
                {
                    s.source = gameObject.AddComponent<AudioSource>();
                    s.source.clip = s.clip;
                    s.source.outputAudioMixerGroup = s.audioMixerGroup;
                    
                    s.source.volume = s.volume;
                    s.source.pitch = s.pitch;
                    s.source.loop = s.loop;
                    s.source.minDistance = s.minDistance;
                    s.source.maxDistance = s.maxDistance;
                }
            }
        }


        private void Start()
        {
            if (hasOnDestroyOnLoad)
            {
                audioMixer.SetFloat("masterVolume", masterVolume.Float);
                audioMixer.SetFloat("musicVolume", musicVolume.Float);
                audioMixer.SetFloat("ambienceVolume", ambienceVolume.Float);
                audioMixer.SetFloat("sfxVolume", sfxVolume.Float);
                PlaySoundFromList("Music","Main");
                StartAmbience();
            }
            else
            {
                PlaySoundFromList("Music", "Menu");
            }
        }


        private void Update()
        {
            if (!PlayerManager.LocalPlayerInstance)
            {
                return;
            }
            if (PlayerManager.LocalPlayerInstance.transform.position != null)
            {
                this.transform.position = PlayerManager.LocalPlayerInstance.transform.position;
            }
        }
        #endregion


        private void StartAmbience()
        {
            PlaySoundFromList("Ambient", "Birds");
            PlaySoundFromList("Ambient", "CrowdSmall");

            float rnd = Random.Range(60, 90);
            StartCoroutine(nameof(RandomAmbienceCoroutine), rnd);

        }

        private IEnumerator RandomAmbienceCoroutine(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            PlaySoundFromList("Ambient", "Axe");
        }
        
        public void StopPlayingList(string name)
        {
            SoundList s = soundLists.Find(list => list.name == name);
            if (s == null)
            {
                Debug.LogWarning("List: " + name + " not found!");
                return;
            }
            if (s.sounds.Count < 1)
            {
                Debug.LogWarning("List: " + name + " does not have any Sounds!");
                return;
            }

            foreach (var sound in s.sounds)
            {
                if (sound.source.isPlaying)
                {
                    sound.source.Stop();
                }
            }
        }

        [PunRPC]
        public bool? isListPlaying(string name)
        {
            SoundList s = soundLists.Find(list => list.name == name);
            if (s == null)
            {
                Debug.LogWarning("List: " + name + " not found!");
                return null;
            }
            foreach (Sound sound in s.sounds)
            {
                if (sound.source.isPlaying)
                {
                    return true;
                }
            }
            return false;
        }

        [PunRPC]
        public void PlaySoundFromList(string listname, string soundname)
        {
            SoundList sl = soundLists.Find(list => list.name == listname);
            if (sl == null)
            {
                Debug.LogWarning("List: " + name + " not found!");
                return;
            }

            Sound s = sl.sounds.Find(sound => sound.name == soundname);
            if (s == null)
            {
                Debug.LogWarning("List: " + name + " not found!");
                return;
            }
            s.source.Play();
        }

        [PunRPC]
        public void PlayRandomFromList(string name)
        {
            SoundList s = soundLists.Find(list => list.name == name);
            if (s == null)
            {
                Debug.LogWarning("List: " + name + " not found!");
                return;
            }
            if (s.sounds.Count < 1)
            {
                Debug.LogWarning("List: " + name + " does not have any Sounds!");
                return;
            }
            s.sounds[UnityEngine.Random.Range(0, s.sounds.Count)].source.Play();
        }

        public AudioSource GetRandomFromList(string name)
        {
            SoundList s = soundLists.Find(list => list.name == name);
            if (s == null)
            {
                Debug.LogWarning("List: " + name + " not found!");
                return null;
            }
            if (s.sounds.Count < 1)
            {
                Debug.LogWarning("List: " + name + " does not have any Sounds!");
                return null;
            }
            return s.sounds[UnityEngine.Random.Range(0, s.sounds.Count)].source;
        }

        public AudioSource GetSoundFromList(string listname, string soundname)
        {
            SoundList sl = soundLists.Find(list => list.name == listname);
            if (sl == null)
            {
                Debug.LogWarning("List: " + name + " not found!");
                return null;
            }

            Sound s = sl.sounds.Find(sound => sound.name == soundname);
            if (s == null)
            {
                Debug.LogWarning("List: " + name + " not found!");
                return null;
            }
            return s.source;
        }
        
        public void PlayLocalSound(string name, Vector3 pos)
        {
            GameObject spawnedSound = PhotonNetwork.Instantiate("LocalSound", pos, Quaternion.identity);

            AudioSource spawnedSource = spawnedSound.GetComponent<AudioSource>();

            spawnedSound.GetComponent<LocalSound>().PlaySound(name);
        }
    }
}

