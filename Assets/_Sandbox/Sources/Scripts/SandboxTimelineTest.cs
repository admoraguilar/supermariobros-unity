using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Linq;


public class SandboxTimelineTest : MonoBehaviour {
    [SerializeField] private PlayableDirector playableDirector;
    [SerializeField] private float playbackSpeed;
    [SerializeField] private GameObject gameObjectToScale;


    private void Start() {
        TimelineAsset timeline = playableDirector.playableAsset as TimelineAsset;
        Debug.Log(string.Format("Tracks count: {0}", timeline.GetOutputTracks().Count()));

        playableDirector.SetGenericBinding(timeline.GetOutputTrack(0), gameObjectToScale);
        playableDirector.timeUpdateMode = DirectorUpdateMode.Manual;
        //playableDirector.time = 0f;

        foreach(var outputTrack in timeline.GetOutputTracks()) {
            Debug.Log(outputTrack.name);
        }

        ActionTemplates.RunActionAfterSeconds("Test", 1f, () => {
            playableDirector.Play();
            Debug.Log("Playing");
            Debug.Log(playableDirector.state);
        });        
    }

    private void Update() {
        if(playableDirector.state == PlayState.Playing) {
            playableDirector.time += playbackSpeed * Time.deltaTime;
            playableDirector.Evaluate();
        }
    }
}
