using UnityEngine;
using WishfulDroplet;
using WishfulDroplet.Extensions;


public class AudioController : MonoBehaviour, IAudioController {
    [SerializeField] private AudioSource worldAudioSource;


    public void Play(AudioClip audioClip, ulong delay = 0L) {
        worldAudioSource.clip = audioClip;
        worldAudioSource.Play(delay);
    }

    public void PlayOneShot(AudioClip clip, float volumeScale = 1f) {
        worldAudioSource.PlayOneShot(clip, volumeScale);
    }

    private void Reset() {
        Transform tr = GetComponent<Transform>();

        worldAudioSource = Utilities.CreateOrGetObject("WorldAudioSource", tr).AddOrGetComponent<AudioSource>();
        worldAudioSource.playOnAwake = false;
        worldAudioSource.volume = .5f;
    }
}


public interface IAudioController {
    void Play(AudioClip audioClip, ulong delay = 0L);
    void PlayOneShot(AudioClip clip, float volumeScale = 1f);
}