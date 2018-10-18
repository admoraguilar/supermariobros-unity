using UnityEngine;
using WishfulDroplet;
using WishfulDroplet.Extensions;


[CreateAssetMenu(menuName = "Actors/CharacterActor/States/Idle")]
public class IdleCharacterState : CharacterActor.CharacterState {
    public override void OnEnter(CharacterActor characterActor) {
        characterActor.thisAnimator.PlayNoRepeat("Idle");
    }

    public override void OnUpdate(CharacterActor characterActor) {
        
    }
}
