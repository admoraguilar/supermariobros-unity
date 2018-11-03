using UnityEngine;
using System;
using System.Collections.Generic;
using WishfulDroplet;
using WishfulDroplet.Extensions;


[CreateAssetMenu(menuName = "Actors/CharacterActor/Brains/Mario")]
public class MarioCharacterBrain : CharacterActor.CharacterBrain {
	public override void DoCharacter2DCasterHit(CharacterActor characterActor, Direction direction, RaycastHit2D hit, Collider2D collider) {
		switch(direction) {
			case Direction.Up:
				// Fall if we bump on something above 
				if(Mathf.Abs(characterActor.transform.position.x - collider.transform.position.x) < .7f) {
					characterActor.movementStateMachine.SetState(characterActor.fallMovementState);
				}
				break;
		}
	}

	public override void DoInteractorCasterHit(CharacterActor characterActor, Direction direction, RaycastHit2D hit, Collider2D collider) {
		Interactable interactable = collider.GetComponentInParent<Interactable>();
		if(interactable) {
			switch(direction) {
				case Direction.Up:
					interactable.Interact(Direction.Down, characterActor.thisGameObject);
					break;
				case Direction.Down:
					interactable.Interact(Direction.Up, characterActor.thisGameObject);
					break;
				case Direction.Left:
					interactable.Interact(Direction.Right, characterActor.thisGameObject);
					break;
				case Direction.Right:
					interactable.Interact(Direction.Left, characterActor.thisGameObject);
					break;
			}
		}
	}

	public override bool DoInteracted(CharacterActor characterActor, Direction direction, GameObject interactor) {
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

		return false;
	}

	public override void DoStart(CharacterActor characterActor) {
		characterActor.formStateMachine.SetState(characterActor.smallFormState);
		characterActor.movementStateMachine.SetState(characterActor.idleMovementState);
		characterActor.statusStateMachine.SetState(characterActor.normalStatusState);
	}

	public override void UpdateInput(CharacterActor characterActor) {
		if(!isUpdateInput) return;

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

		if(Input.GetKeyDown(KeyCode.F)) {
			characterActor.statusStateMachine.SetState(characterActor.normalStatusState);
		}

		if(Input.GetKeyDown(KeyCode.G)) {
			characterActor.statusStateMachine.SetState(characterActor.deadStatusState);
		}
	}

	public override void DoUpdate(CharacterActor characterActor) {
		if(characterActor.statusStateMachine.currentState != characterActor.deadStatusState &&
		   characterActor.movementStateMachine.currentState != characterActor.transitionMovementState) {
			if(characterActor.thisCharacter2D.isGrounded) {
				if(!characterActor.thisCharacter2D.IsMoving() &&
					characterActor.inputAxis == Vector2.zero &&
					!characterActor.isJumping) {
					characterActor.movementStateMachine.SetState(characterActor.idleMovementState);
				}

				if(characterActor.inputAxis.x != 0 &&
				   characterActor.inputAxis.y == 0 &&
				   !characterActor.isJumping &&
				   !characterActor.thisCharacter2D.isChangingDirection) {
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

				if(characterActor.thisCharacter2D.isChangingDirection &&
				   characterActor.thisCharacter2D.IsMoving() &&
				   !characterActor.isJumping) {
					characterActor.movementStateMachine.SetState(characterActor.slideMovementState);
				}

				if(characterActor.isJumping) {
					characterActor.movementStateMachine.SetState(characterActor.jumpMovementState);
				}
			} else {
				if((Mathf.Abs(characterActor.thisTransform.localPosition.y - characterActor.lastJumpPos.y) > characterActor.maxJumpHeight ||
					characterActor.thisCharacter2D.IsColliding(Direction.Up, CollisionFilter.OnlyNonTrigger) ||
					!characterActor.isJumping) &&
					!Equals(characterActor.movementStateMachine.currentState, characterActor.bounceMovementState)) {
					characterActor.movementStateMachine.SetState(characterActor.fallMovementState);
				}
			}
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