using UnityEngine;
using System;
using WishfulDroplet;
using WishfulDroplet.Extensions;


[CreateAssetMenu(menuName = "Actors/Blocks/Brains/Block")]
public class BrickBlockBrain : BlockActor.BlockBrain {
	public override bool DoInteracted(BlockActor blockActor, Direction direction, GameObject interactor) {
		//Debug.Log("Interacted");

		switch(direction) {
			case Direction.Down:
				CharacterActor otherCharacterActor = interactor.GetComponent<CharacterActor>();
				if(otherCharacterActor) {
					Collider2D collision = otherCharacterActor.thisInteractionCollider2D;

					if(otherCharacterActor.formStateMachine.currentState.isCanBreakBrick) {
						OperationQueueHelper.RunActionAfterSeconds("BlockBrain_DelayedDisable", 1f, () => { blockActor.gameObject.SetActive(false); });

						if(blockActor.content) {
							Instantiate(blockActor.content, blockActor.thisTransform.position, blockActor.thisTransform.rotation);
						}
						Singleton.Get<IAudioController>().PlayOneShot(blockActor.contentAppearSound);
						//Debug.Log("Destroy");
						return true;
					} else {
						blockActor.thisAnimator.PlayNoRepeat("Interacted");
						Singleton.Get<IAudioController>().PlayOneShot(blockActor.hitSound);
						//Debug.Log("Interact");
						return true;
					}
				}
				break;
		}

		return false;
	}
}