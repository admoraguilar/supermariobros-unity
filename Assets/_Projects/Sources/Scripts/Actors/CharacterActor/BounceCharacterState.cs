using UnityEngine;
using WishfulDroplet;
using WishfulDroplet.Extensions;


[CreateAssetMenu(menuName = "Actors/CharacterActor/States/Bounce")]
public class BounceCharacterState : CharacterActor.CharacterState {
    public AudioClip StepSound;
    public float BounceTime;

    private float bounceTime;


    public override void OnEnter(CharacterActor characterActor) {
        bounceTime = 0f;

        characterActor.thisAnimator.PlayNoRepeat("Jump");
        characterActor.thisCharacter2D.SetVelocity(new Vector2(characterActor.thisCharacter2D.Velocity.x, 0f));
        Singleton.Get<IAudioController>().PlayOneShot(StepSound);
    }

    public override void OnFixedUpdate(CharacterActor characterActor) {
        if(bounceTime > BounceTime) {
            characterActor.formStateMachine.PopState();
        } else {
            bounceTime += Time.fixedDeltaTime;

            float speed = characterActor.isSprinting ? characterActor.airSprintSpeed : characterActor.airMoveSpeed;
            characterActor.thisCharacter2D.Move(new Vector2(characterActor.inputAxis.x * speed, 1f * characterActor.jumpSpeed) * Time.fixedDeltaTime);
        }
    }
}
