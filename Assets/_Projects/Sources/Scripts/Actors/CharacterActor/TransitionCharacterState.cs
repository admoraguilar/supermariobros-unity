using UnityEngine;
using WishfulDroplet;
using WishfulDroplet.Extensions;

[CreateAssetMenu(menuName = "Actors/CharacterActor/States/Transition")]
public class TransitionCharacterState : CharacterActor.CharacterState {
    public AudioClip TransitionSound;
    public CharacterActor.BoxColliderInfo BoxColliderInfo;


    public override void OnEnter(CharacterActor characterActor) {
        Time.timeScale = 0f;
        characterActor.thisAnimator.PlayNoRepeat("Transition");
        Singleton.Get<IAudioController>().PlayOneShot(TransitionSound);
    }

    public override void OnUpdate(CharacterActor characterActor) {
        if(characterActor.thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("Transition")) {
            if(characterActor.thisAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > .95f) {
                Time.timeScale = 1f;
                characterActor.formStateMachine.PopState();
            }
        }
    }

    public override void OnExit(CharacterActor characterActor) {
        characterActor.thisCollisionCollider2D.offset = BoxColliderInfo.Offset;
        characterActor.thisCollisionCollider2D.size = BoxColliderInfo.Size;

        characterActor.thisInteractionCollider2D.offset = BoxColliderInfo.Offset;
        characterActor.thisInteractionCollider2D.size = BoxColliderInfo.Size;
    }
}
