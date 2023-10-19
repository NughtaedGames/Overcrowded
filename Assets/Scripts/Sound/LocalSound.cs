using UnityEngine;

using Photon.Pun;
using WS20.P3.Overcrowded;

[RequireComponent(typeof(AudioSource))]
public class LocalSound : MonoBehaviourPunCallbacks
{

    bool isPlaying;

    public void PlaySound(string name)
    {
        photonView.RPC("PlaySoundRPC", RpcTarget.All, name);
    }

    [PunRPC]
    public void PlaySoundRPC(string name)
    {
        AudioSource source = this.GetComponent<AudioSource>();

        AudioSource selected = AudioManager.instance.GetRandomFromList(name);

        source.clip = selected.clip;
        source.volume = selected.volume;
        source.pitch = selected.pitch;
        source.maxDistance = selected.maxDistance;
        source.minDistance = selected.minDistance;

        source.Play();
        isPlaying = true;
    }

    private void Update()
    {
        if (isPlaying)
        {
            if (!this.GetComponent<AudioSource>().isPlaying)
            {
                //DestroyImmediate(this);
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.Destroy(this.photonView);
                }
            }
        }
    }
}
