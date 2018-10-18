using UnityEngine;
using WishfulDroplet;
using WishfulDroplet.Extensions;


[CreateAssetMenu(menuName = "Actors/CharacterActor/States/Walk")]
public class WalkCharacterState : CharacterActor.CharacterState {
    public override void OnEnter(CharacterActor characterActor) {
        characterActor.thisAnimator.PlayNoRepeat("Walk");
    }

    public override void OnUpdate(CharacterActor characterActor) {
        characterActor.thisAnimator.SetFloat("WalkSpeedMultiplier",
                                    2.5f * Mathf.Max(0.1f, Mathf.InverseLerp(0f, characterActor.thisCharacter2D.MaxVelocity.x,
                                                                                 Mathf.Abs(characterActor.thisCharacter2D.Velocity.x))));
    }

    public override void OnFixedUpdate(CharacterActor characterActor) {
        float speed = characterActor.isSprinting ? characterActor.landSprintSpeed : characterActor.landMoveSpeed;
        characterActor.thisCharacter2D.Move(new Vector2(characterActor.inputAxis.x, 0f) * speed * Time.fixedDeltaTime);
    }
}
