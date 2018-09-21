using UnityEngine;


public class Command : MonoBehaviour {
    private AudioController thisAudioController;


    public void AudioController_PlayOneShot(AudioClip audioClip) {
        thisAudioController.PlayOneShot(audioClip);
    }

    private void Awake() {
        thisAudioController = SingletonController.Get<AudioController>();
    }
}
