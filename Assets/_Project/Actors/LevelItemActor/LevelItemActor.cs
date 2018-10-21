using UnityEngine;
using WishfulDroplet;


public class LevelItemActor : Actor<LevelItemActor, LevelItemActor.LevelItemBrain> {
    [InspectorNote("Level Item Actor")]
    [Header("Data")]
    public int test;


    private void OnDrawGizmos() {
        
    }

    public abstract class LevelItemBrain : ActorBrain<LevelItemActor> {
        
    }
}
