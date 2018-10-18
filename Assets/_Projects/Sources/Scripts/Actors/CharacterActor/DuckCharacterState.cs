using UnityEngine;
using WishfulDroplet;
using WishfulDroplet.Extensions;


[CreateAssetMenu(menuName = "Actors/CharacterActor/States/Duck")]
public class DuckCharacterState : CharacterActor.CharacterState {
    public CharacterActor.BoxColliderInfo BoxColliderInfo;


    public override void OnEnter(CharacterActor characterActor) {
        characterActor.thisAnimator.PlayNoRepeat("Duck");

        characterActor.thisCollisionCollider2D.offset = BoxColliderInfo.Offset;
        characterActor.thisCollisionCollider2D.size = BoxColliderInfo.Size;

        characterActor.thisInteractionCollider2D.offset = BoxColliderInfo.Offset;
        characterActor.thisInteractionCollider2D.size = BoxColliderInfo.Size;
    }
}
