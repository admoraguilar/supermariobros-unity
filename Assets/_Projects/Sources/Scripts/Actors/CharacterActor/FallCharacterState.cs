using UnityEngine;
using WishfulDroplet;
using WishfulDroplet.Extensions;


[CreateAssetMenu(menuName = "Actors/CharacterActor/States/Fall")]
public class FallCharacterState : CharacterActor.CharacterState {
    public override void OnEnter(CharacterActor characterActor) {
        characterActor.thisAnimator.PlayNoRepeat("Fall");
    }

    public override void OnFixedUpdate(CharacterActor characterActor) {
        float speed = characterActor.isSprinting ? characterActor.airSprintSpeed : characterActor.airMoveSpeed;
        characterActor.thisCharacter2D.Move(new Vector2(characterActor.inputAxis.x * speed, 0f) * Time.fixedDeltaTime);
    }
}
