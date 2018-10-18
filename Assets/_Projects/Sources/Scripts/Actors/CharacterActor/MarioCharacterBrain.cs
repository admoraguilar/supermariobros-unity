using UnityEngine;
using System;
using WishfulDroplet;
using WishfulDroplet.Extensions;


[CreateAssetMenu(menuName = "Actors/CharacterActor/Brains/MarioBrain")]
public class MarioCharacterBrain : CharacterActor.CharacterBrain {
    [SerializeField] private CharacterActor.CharacterBrain enemyBrain;


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

    public override void DoUpdate(CharacterActor characterActor) {
        if(characterActor.statusStateMachine.currentState != characterActor.deadStatusState &&
           characterActor.movementStateMachine.currentState != characterActor.transitionMovementState) {
            if(characterActor.thisCharacter2D.IsGrounded) {
                if(!characterActor.thisCharacter2D.IsMoving(Direction.Any) &&
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
                   ((CharacterActor.FormStates.FormState)characterActor.formStateMachine.currentState).isCanDuck &&
                   !characterActor.isJumping) {
                    characterActor.movementStateMachine.PushState(characterActor.duckMovementState);
                } else {
                    if(Equals(characterActor.movementStateMachine.currentState, characterActor.duckMovementState)) {
                        characterActor.movementStateMachine.PopState();
                    }
                }

                if(characterActor.thisCharacter2D.IsChangingDirection &&
                   characterActor.thisCharacter2D.IsMoving(Direction.Any) &&
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


    public override void DoTriggerEnter2D(CharacterActor characterActor, Collider2D collision) {
        CharacterActor otherCharacterActor = collision.GetComponent<CharacterActor>();
        if(otherCharacterActor) {
            if(otherCharacterActor.brain == enemyBrain || otherCharacterActor.brain == null) {
                if(Mathf.Abs(characterActor.thisInteractionCollider2D.bounds.min.y - collision.bounds.min.y) > .05f &&
                    characterActor.thisInteractionCollider2D.bounds.min.y >= collision.bounds.min.y) {
                    characterActor.movementStateMachine.PushState(characterActor.bounceMovementState);
                    Destroy(otherCharacterActor.gameObject);
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