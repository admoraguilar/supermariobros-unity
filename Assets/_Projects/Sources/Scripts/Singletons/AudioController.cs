using UnityEngine;


public class AudioController : MonoBehaviour {
    [SerializeField] private AudioSource worldAudioSourcePrefab;

    private AudioSource worldAudioSource;

    private Transform thisTransform;


    public void PlayOneShot(AudioClip clip, float volumeScale = 1f) {
        worldAudioSource.PlayOneShot(clip, volumeScale);
    }

    private void Awake() {
        thisTransform = GetComponent<Transform>();
    }

    private void Start() {
        // Spawn systemss
        worldAudioSource = this.Spawn(worldAudioSourcePrefab, thisTransform, true);
    }
}
