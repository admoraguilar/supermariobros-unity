using UnityEngine;
using WishfulDroplet;
using WishfulDroplet.Extensions;


[CreateAssetMenu(menuName = "Actors/Blocks/Brains/Block")]
public class BrickBlockBrain : BlockActor.BlockBrain {
    public override void DoTriggerEnter2D(BlockActor blockActor, Collider2D collision) {
        if(Utilities.CheckOtherColliderDirection2D(Direction.Down, blockActor.thisInteractionCollider2D, collision, 5f)) {
            CharacterActor otherCharacterActor = collision.GetComponent<CharacterActor>();
            
            if(otherCharacterActor) {
                if(otherCharacterActor.formStateMachine.currentState.isCanBreakBrick) {
                    otherCharacterActor.movementStateMachine.SetState(otherCharacterActor.fallMovementState);
                    Singleton.Get<IAudioController>().PlayOneShot(blockActor.breakSound);
                    Destroy(blockActor.gameObject);
                } else {
                    Singleton.Get<IAudioController>().PlayOneShot(blockActor.hitSound);
                    blockActor.thisAnimator.PlayNoRepeat("Interacted");
                }
            }
        }
    }
}
