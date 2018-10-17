using UnityEngine;


public class CharacterIdleState : CharacterActor.CharacterBrain {
    public override void UpdateInput(CharacterActor characterActor) {
        characterActor.inputAxis = Vector2.zero;
        
    }
}
