using UnityEngine;


public class Level : MonoBehaviour {
    public string Name { get { return name; } }

    public float Time;
    public AudioClip BGM;
    public Color BackgroundColor;
    public Transform PlayerStart;
    public AudioSource AudioSource;
}
