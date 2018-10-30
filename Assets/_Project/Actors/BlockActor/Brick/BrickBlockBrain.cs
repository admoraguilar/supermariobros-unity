using UnityEngine;
using WishfulDroplet;
using WishfulDroplet.Extensions;


[CreateAssetMenu(menuName = "Actors/Blocks/Brains/Block")]
public class BrickBlockBrain : BlockActor.BlockBrain {
	public override bool DoInteract(BlockActor blockActor, GameObject interactor) {
		Debug.Log(string.Format("Brick interacted: {0}", interactor.name));

		CharacterActor otherCharacterActor = interactor.GetComponent<CharacterActor>();
		if(otherCharacterActor) {
			Collider2D collision = otherCharacterActor.thisInteractionCollider2D;

			if(Utilities.CheckOtherColliderDirection2D(Direction.Down, blockActor.thisInteractionCollider2D, collision, 5f)) {
				if(otherCharacterActor.formStateMachine.currentState.isCanBreakBrick) {
					ActionTemplates.RunActionAfterSeconds("BlockBrain_DelayedDisable", .05f, () => { blockActor.gameObject.SetActive(false); });
					if (blockActor.content) {
						Instantiate(blockActor.content, blockActor.thisTransform.position, blockActor.thisTransform.rotation);
					}
					Singleton.Get<IAudioController>().PlayOneShot(blockActor.contentAppearSound);
					Debug.Log("Destroy");
					return true;
				} else {
					blockActor.thisAnimator.PlayNoRepeat("Interacted");
					Singleton.Get<IAudioController>().PlayOneShot(blockActor.hitSound);
					Debug.Log("Interact");
					return true;
				}
			}
		}

		return false;
	}

	public override void DoTriggerEnter2D(BlockActor blockActor, Collider2D collision) {
		//if(Utilities.CheckOtherColliderDirection2D(Direction.Left, blockActor.thisInteractionCollider2D, collision, 5f)) {

		//}

		//if(Utilities.CheckOtherColliderDirection2D(Direction.Down, blockActor.thisInteractionCollider2D, collision, 5f)) {
		//	CharacterActor otherCharacterActor = collision.transform.root.GetComponent<CharacterActor>();

		//	Debug.Log(string.Format("Brick interacted below: {0}", collision.name));

		//	if(otherCharacterActor) {
		//		if(otherCharacterActor.formStateMachine.currentState.isCanBreakBrick) {
		//			ActionTemplates.RunActionAfterSeconds("BlockBrain_DelayedDisable", .05f, () => { blockActor.gameObject.SetActive(false); });

		//			if(blockActor.content) {
		//				Instantiate(blockActor.content, blockActor.thisTransform.position, blockActor.thisTransform.rotation);
		//			}
		//			Singleton.Get<IAudioController>().PlayOneShot(blockActor.contentAppearSound);
		//		} else {
		//			blockActor.thisAnimator.PlayNoRepeat("Interacted");

		//			Singleton.Get<IAudioController>().PlayOneShot(blockActor.hitSound);
		//		}
		//	}
		//}

		Interactable interactable = collision.transform.root.GetComponent<Interactable>();
		if(interactable) {
			interactable.Interact(blockActor.gameObject);
		}
	}
}