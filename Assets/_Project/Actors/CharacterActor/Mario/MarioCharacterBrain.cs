using UnityEngine;
using System;
using WishfulDroplet;
using WishfulDroplet.Extensions;


[CreateAssetMenu(menuName = "Actors/CharacterActor/Brains/Mario")]
public class MarioCharacterBrain : CharacterActor.CharacterBrain {
	public override void DoStart(CharacterActor characterActor) {
		characterActor.formStateMachine.SetState(characterActor.smallFormState);
		characterActor.movementStateMachine.SetState(characterActor.idleMovementState);
		characterActor.statusStateMachine.SetState(characterActor.normalStatusState);
	}

	public override void UpdateInput(CharacterActor characterActor) {
		characterActor.inputAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		characterActor.isSprinting = Input.GetKey(KeyCode.LeftShift);
		characterActor.isJumping = Input.GetKey(KeyCode.Space);

		if(Input.GetKeyDown(KeyCode.E)) {
			characterActor.SetForm(characterActor.smallFormState);
		}

		if(Input.GetKeyDown(KeyCode.R)) {
			characterActor.SetForm(characterActor.bigFormState);
		}

		if(Input.GetKeyDown(KeyCode.T)) {
			characterActor.SetForm(characterActor.powerFormState);
		}
	}

	public override bool DoInteract(CharacterActor characterActor, GameObject interactor) {
		CharacterActor otherCharacterActor = interactor.GetComponent<CharacterActor>();
		if(otherCharacterActor) {
			Collider2D collision = otherCharacterActor.thisInteractionCollider2D;

			// Stomp/Destroy enemies if they are below mario
			if(characterActor.IsBrainEnemy(otherCharacterActor.brain)) {
				if(Utilities.CheckOtherColliderDirection2D(Direction.Down, characterActor.thisInteractionCollider2D, collision)) {
					characterActor.movementStateMachine.PushState(characterActor.bounceMovementState);
					Destroy(otherCharacterActor.gameObject);
					return true;
				}
			}
		}

		BlockActor otherBlockActor = interactor.GetComponent<BlockActor>();
		if(otherBlockActor) {
			Collider2D collision = otherBlockActor.thisInteractionCollider2D;

			Debug.Log("Block interacted");
			if(Utilities.CheckOtherColliderDirection2D(Direction.Up, characterActor.thisInteractionCollider2D, collision)) {
				if(characterActor.formStateMachine.currentState.isCanBreakBrick) {
					otherBlockActor.Destroy();
					return true;
				} else {
					otherBlockActor.Interact();
					return true;
				}
			}
		}

		return false;
	}

	public override void DoUpdate(CharacterActor characterActor) {
		if(characterActor.statusStateMachine.currentState != characterActor.deadStatusState &&
		   characterActor.movementStateMachine.currentState != characterActor.transitionMovementState) {
			if(characterActor.thisCharacter2D.IsGrounded) {
				if(!characterActor.thisCharacter2D.IsMoving() &&
					characterActor.inputAxis == Vector2.zero &&
					!characterActor.isJumping) {
					characterActor.movementStateMachine.SetState(characterActor.idleMovementState);
				}

				if(characterActor.inputAxis.x != 0 &&
				   characterActor.inputAxis.y == 0 &&
				   !characterActor.isJumping &&
				   !characterActor.thisCharacter2D.IsChangingDirection) {
					characterActor.movementStateMachine.SetState(characterActor.walkMovementState);
				}

				if(characterActor.inputAxis.x == 0 &&
				   characterActor.inputAxis.y < 0f &&
				   characterActor.formStateMachine.currentState.isCanDuck &&
				   !characterActor.isJumping) {
					characterActor.movementStateMachine.PushState(characterActor.duckMovementState);
				} else {
					if(Equals(characterActor.movementStateMachine.currentState, characterActor.duckMovementState)) {
						characterActor.movementStateMachine.PopState();
					}
				}

				if(characterActor.thisCharacter2D.IsChangingDirection &&
				   characterActor.thisCharacter2D.IsMoving() &&
				   !characterActor.isJumping) {
					characterActor.movementStateMachine.SetState(characterActor.slideMovementState);
				}

				if(characterActor.isJumping) {
					characterActor.movementStateMachine.SetState(characterActor.jumpMovementState);
				}
			} else {
				if((Mathf.Abs(characterActor.thisTransform.localPosition.y - characterActor.lastJumpPos.y) > characterActor.maxJumpHeight ||
					characterActor.thisCharacter2D.IsColliding(Direction.Up) ||
					!characterActor.isJumping) &&
					!Equals(characterActor.movementStateMachine.currentState, characterActor.bounceMovementState)) {
					characterActor.movementStateMachine.SetState(characterActor.fallMovementState);
				}
			}
		}
	}


	public override void DoCollisionEnter2D(CharacterActor actor, Collision2D collision) {
		Debug.Log(string.Format("{0} collided with {1}", actor.name, collision.collider.name));
	}

	public override void DoTriggerEnter2D(CharacterActor characterActor, Collider2D collision) {
		//Debug.Log(string.Format("{0} | {1}", characterActor.name, collision.transform.root.name));
		//Debug.Log(string.Format("Getting parent: {0}", collision.transform.root.GetComponentInParent<Rigidbody2D>().name));

		//CharacterActor otherCharacterActor = collision.transform.root.GetComponent<CharacterActor>();
		//if(otherCharacterActor) {
		//	// Stomp/Destroy enemies if they are below mario
		//	if(characterActor.IsBrainEnemy(otherCharacterActor.brain)) {
		//		if(Utilities.CheckOtherColliderDirection2D(Direction.Down, characterActor.thisInteractionCollider2D, collision)) {
		//			characterActor.movementStateMachine.PushState(characterActor.bounceMovementState);
		//			Destroy(otherCharacterActor.gameObject);
		//		}
		//	}
		//}

		//BlockActor otherBlockActor = collision.transform.parent.GetComponent<BlockActor>();
		//if(otherBlockActor) {
		//	Debug.Log("Block interacted");
		//	if(Utilities.CheckOtherColliderDirection2D(Direction.Up, characterActor.thisInteractionCollider2D, collision)) {
		//		if(characterActor.formStateMachine.currentState.isCanBreakBrick) {
		//			otherBlockActor.Destroy();
		//		} else {
		//			otherBlockActor.Interact();
		//		}
		//	}
		//}

		//// Fall if we bump on something above
		//if(Utilities.CheckOtherColliderDirection2D(Direction.Up, characterActor.thisInteractionCollider2D, collision, 5f)) {
		//	characterActor.movementStateMachine.SetState(characterActor.fallMovementState);
		//}

		// Fall if we bump on something above
		if(Utilities.CheckOtherColliderDirection2D(Direction.Up, characterActor.thisInteractionCollider2D, collision, 5f)) {
			characterActor.movementStateMachine.SetState(characterActor.fallMovementState);
		}

		Interactable interactable = collision.transform.root.GetComponent<Interactable>();
		if(interactable) {
			interactable.Interact(characterActor.gameObject);
		}
	}


	public override void DoDrawGizmos(CharacterActor characterActor) {
		// Small duck collider
		Gizmos.color = characterActor.smallFormState.duckBoxColliderInfo.gizmoColor;
		Gizmos.DrawWireCube(new Vector2(characterActor.thisTransform.position.x, characterActor.thisTransform.position.y) +
										characterActor.smallFormState.duckBoxColliderInfo.offset,
							characterActor.smallFormState.duckBoxColliderInfo.size);

		// Small transition collider 
		Gizmos.color = characterActor.smallFormState.transitionBoxColliderInfo.gizmoColor;
		Gizmos.DrawWireCube(new Vector2(characterActor.thisTransform.position.x, characterActor.thisTransform.position.y) +
										characterActor.smallFormState.transitionBoxColliderInfo.offset,
							characterActor.smallFormState.transitionBoxColliderInfo.size);

		// Big duck collider
		Gizmos.color = characterActor.bigFormState.duckBoxColliderInfo.gizmoColor;
		Gizmos.DrawWireCube(new Vector2(characterActor.thisTransform.position.x, characterActor.thisTransform.position.y) +
										characterActor.bigFormState.duckBoxColliderInfo.offset,
							characterActor.bigFormState.duckBoxColliderInfo.size);

		// Big transition collider 
		Gizmos.color = characterActor.bigFormState.transitionBoxColliderInfo.gizmoColor;
		Gizmos.DrawWireCube(new Vector2(characterActor.thisTransform.position.x, characterActor.thisTransform.position.y) +
										characterActor.bigFormState.transitionBoxColliderInfo.offset,
							characterActor.bigFormState.transitionBoxColliderInfo.size);

		// Power duck collider
		Gizmos.color = characterActor.powerFormState.duckBoxColliderInfo.gizmoColor;
		Gizmos.DrawWireCube(new Vector2(characterActor.thisTransform.position.x, characterActor.thisTransform.position.y) +
										characterActor.powerFormState.duckBoxColliderInfo.offset,
							characterActor.powerFormState.duckBoxColliderInfo.size);

		// Power transition collider 
		Gizmos.color = characterActor.powerFormState.transitionBoxColliderInfo.gizmoColor;
		Gizmos.DrawWireCube(new Vector2(characterActor.thisTransform.position.x, characterActor.thisTransform.position.y) +
										characterActor.powerFormState.transitionBoxColliderInfo.offset,
							characterActor.powerFormState.transitionBoxColliderInfo.size);
	}


	public override void DoReset(CharacterActor characterActor) {
		characterActor.smallFormState.transitionBoxColliderInfo = new CharacterActor.FormStates.BoxColliderInfo {
			offset = new Vector2(0f, 0f),
			size = new Vector2(0.65f, 0.8f),
			gizmoColor = Color.blue
		};

		characterActor.smallFormState.duckBoxColliderInfo = new CharacterActor.FormStates.BoxColliderInfo {
			offset = new Vector2(0f, 0.15f),
			size = new Vector2(0.8f, 1.11f),
			gizmoColor = Color.cyan
		};

		characterActor.bigFormState.transitionBoxColliderInfo = new CharacterActor.FormStates.BoxColliderInfo {
			offset = new Vector2(0f, 0.4f),
			size = new Vector2(0.8f, 1.6f),
			gizmoColor = Color.blue
		};

		characterActor.bigFormState.duckBoxColliderInfo = new CharacterActor.FormStates.BoxColliderInfo {
			offset = new Vector2(0f, 0.15f),
			size = new Vector2(0.8f, 1.11f),
			gizmoColor = Color.cyan
		};

		characterActor.powerFormState.transitionBoxColliderInfo = new CharacterActor.FormStates.BoxColliderInfo {
			offset = new Vector2(0f, 0.4f),
			size = new Vector2(0.8f, 1.6f),
			gizmoColor = Color.blue
		};

		characterActor.powerFormState.duckBoxColliderInfo = new CharacterActor.FormStates.BoxColliderInfo {
			offset = new Vector2(0f, 0.15f),
			size = new Vector2(0.8f, 1.11f),
			gizmoColor = Color.cyan
		};
	}
}