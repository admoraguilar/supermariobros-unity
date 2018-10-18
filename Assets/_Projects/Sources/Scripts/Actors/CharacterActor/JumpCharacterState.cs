using UnityEngine;
using WishfulDroplet;
using WishfulDroplet.Extensions;


[CreateAssetMenu(menuName = "Actors/CharacterActor/States/Jump")]
public class JumpCharacterState : CharacterActor.CharacterState {
    public AudioClip JumpSound;


    public override void OnEnter(CharacterActor characterActor) {
        characterActor.lastJumpPos = characterActor.thisTransform.localPosition;
        characterActor.thisAnimator.PlayNoRepeat("Jump");
        Singleton.Get<IAudioController>().PlayOneShot(JumpSound);
    }

    public override void OnFixedUpdate(CharacterActor characterActor) {
        float speed = characterActor.isSprinting ? characterActor.airSprintSpeed : characterActor.airMoveSpeed;
        characterActor.thisCharacter2D.Move(new Vector2(characterActor.inputAxis.x * speed, 1f * characterActor.jumpSpeed) * Time.fixedDeltaTime);

    }
}
