using UnityEngine;
using WishfulDroplet;
using WishfulDroplet.Extensions;


[CreateAssetMenu(menuName = "Actors/Blocks/Brains/Block")]
public class BrickBlockBrain : BlockActor.BlockBrain {
	public override void DoTriggerEnter2D(BlockActor blockActor, Collider2D collision) {
		Debug.Log("Brick interacted");

		if(Utilities.CheckOtherColliderDirection2D(Direction.Down, blockActor.thisInteractionCollider2D, collision, 5f)) {
			CharacterActor otherCharacterActor = collision.GetComponent<CharacterActor>();

			Debug.Log(string.Format("Brick interacted below: {0}", collision.name));

			if(otherCharacterActor) {
				if(otherCharacterActor.formStateMachine.currentState.isCanBreakBrick) {
					ActionTemplates.RunActionAfterSeconds("BlockBrain_DelayedDisable", .05f, () => { blockActor.gameObject.SetActive(false); });

					if(blockActor.content) {
						Instantiate(blockActor.content, blockActor.thisTransform.position, blockActor.thisTransform.rotation);
					}
					Singleton.Get<IAudioController>().PlayOneShot(blockActor.contentAppearSound);
				} else {
					blockActor.thisAnimator.PlayNoRepeat("Interacted");

					Singleton.Get<IAudioController>().PlayOneShot(blockActor.hitSound);
				}
			}
		}
	}
}