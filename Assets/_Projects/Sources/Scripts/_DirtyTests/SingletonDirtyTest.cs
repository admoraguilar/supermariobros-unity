using UnityEngine;


public class SingletonDirtyTest : MonoBehaviour {
    [SerializeField] private AudioClip soundClip;

    private IAudioController audioController;


    private void Awake() {
        audioController = Singleton.Get<IAudioController>();
    }

    private void Start() {
        audioController.PlayOneShot(soundClip);
    }
}
