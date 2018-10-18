using UnityEngine;
using System;
using WishfulDroplet;
using WishfulDroplet.Extensions;


[CreateAssetMenu(menuName = "Actors/CharacterActor/Brains/MarioBrain")]
public class MarioCharacterBrain : CharacterActor.CharacterBrain {
    [SerializeField] private MarioForm[] marioForms = new MarioForm[3];

    [Header("Form State")]
    [SerializeField] private IdleCharacterState idleState;
    [SerializeField] private WalkCharacterState walkState;
    [SerializeField] private SlideCharacterState slideState;
    [SerializeField] private DuckCharacterState duckState;
    [SerializeField] private JumpCharacterState jumpState;
    [SerializeField] private FallCharacterState fallState;
    [SerializeField] private BounceCharacterState bounceState;
    [SerializeField] private TransitionCharacterState transitionState;

    [SerializeField] private TransitionCharacterState testTransitionState;

    private MarioForm currentForm;
    private CharacterActor cachedCharacterActor;


    public override void DoAwake(CharacterActor characterActor) {
        testTransitionState = Instantiate(transitionState);
        Debug.Log(Equals(testTransitionState, transitionState).ToString());
    }

    public override void DoStart(CharacterActor characterActor) {
        characterActor.SetForm(MarioFormId.MARIOSMALL);
    }

    private void OnChangedForm(string formId) {
        for(int i = 0; i < marioForms.Length; i++) {
            if(marioForms[i].id == formId) {
                currentForm = marioForms[i];
            }
        }

        if(currentForm.id == formId) {
            cachedCharacterActor.thisAnimator.runtimeAnimatorController = currentForm.animator;

            transitionState.TransitionSound = currentForm.TransitionSound;
            transitionState.BoxColliderInfo = currentForm.TransitionBoxColliderInfo;

            duckState.BoxColliderInfo = currentForm.DuckBoxColliderInfo;

            jumpState.JumpSound = currentForm.JumpSound;

            cachedCharacterActor.formStateMachine.SetState(transitionState);
        } 
    }

    public override void DoOnEnable(CharacterActor characterActor) {
        characterActor.OnChangedForm += OnChangedForm;
        cachedCharacterActor = characterActor;
    }

    public override void DoOnDisable(CharacterActor characterActor) {
        characterActor.OnChangedForm -= OnChangedForm;
    }

    public override void UpdateInput(CharacterActor characterActor) {
        characterActor.inputAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        characterActor.isSprinting = Input.GetKey(KeyCode.LeftShift);
        characterActor.isJumping = Input.GetKey(KeyCode.Space);

        if(Input.GetKeyDown(KeyCode.E)) {
            characterActor.SetForm(MarioFormId.MARIOSMALL);
        }

        if(Input.GetKeyDown(KeyCode.R)) {
            characterActor.SetForm(MarioFormId.MARIOBIG);
        }

        if(Input.GetKeyDown(KeyCode.T)) {
            characterActor.SetForm(MarioFormId.MARIOPOWER);
        }
    }

    public override void DoUpdate(CharacterActor characterActor) {
        if(!Equals(characterActor.formStateMachine.currentState, transitionState)) {
            if(characterActor.thisCharacter2D.IsGrounded) {
                if(!characterActor.thisCharacter2D.IsMoving(Direction.Any) &&
                    characterActor.inputAxis == Vector2.zero &&
                    !characterActor.isJumping) {
                    characterActor.formStateMachine.SetState(idleState);
                }

                if(characterActor.inputAxis.x != 0 &&
                   characterActor.inputAxis.y == 0 &&
                   !characterActor.isJumping &&
                   !characterActor.thisCharacter2D.IsChangingDirection) {
                    characterActor.formStateMachine.SetState(walkState);
                }

                if(characterActor.inputAxis.x == 0 &&
                   characterActor.inputAxis.y < 0f &&
                   currentForm.IsCanDuck &&
                   !characterActor.isJumping) {
                    characterActor.formStateMachine.PushState(duckState);
                } else {
                    if(Equals(characterActor.formStateMachine.currentState, duckState)) {
                        characterActor.formStateMachine.PopState();
                    }
                }

                if(characterActor.thisCharacter2D.IsChangingDirection &&
                   characterActor.thisCharacter2D.IsMoving(Direction.Any) &&
                   !characterActor.isJumping) {
                    characterActor.formStateMachine.SetState(slideState);
                }

                if(characterActor.isJumping) {
                    characterActor.formStateMachine.SetState(jumpState);
                }
            } else {
                if((Mathf.Abs(characterActor.thisTransform.localPosition.y - characterActor.lastJumpPos.y) > characterActor.maxJumpHeight ||
                    characterActor.thisCharacter2D.IsColliding(Direction.Up) ||
                    !characterActor.isJumping) &&
                    !Equals(characterActor.formStateMachine.currentState, bounceState)) {
                    characterActor.formStateMachine.SetState(fallState);
                }
            }
        }
    }

    public override void DoDrawGizmos(CharacterActor characterActor) {
        for(int i = 0; i < marioForms.Length; i++) {
            // Duck collider
            Gizmos.color = marioForms[i].DuckBoxColliderInfo.GizmoColor;
            Gizmos.DrawWireCube(new Vector2(characterActor.thisTransform.position.x, characterActor.thisTransform.position.y) + marioForms[i].DuckBoxColliderInfo.Offset,
                                            marioForms[i].DuckBoxColliderInfo.Size);

            // Transition collider 
            Gizmos.color = marioForms[i].TransitionBoxColliderInfo.GizmoColor;
            Gizmos.DrawWireCube(new Vector2(characterActor.thisTransform.position.x, characterActor.thisTransform.position.y) + marioForms[i].TransitionBoxColliderInfo.Offset,
                                            marioForms[i].TransitionBoxColliderInfo.Size);

            
        }
    }


    public class MarioFormId {
        public const string MARIOSMALL = "MARIOSMALL";
        public const string MARIOBIG = "MARIOBIG";
        public const string MARIOPOWER = "MARIOPOWER";
    }


    [Serializable]
    public class MarioForm {
        public string id;
        public RuntimeAnimatorController animator;

        public bool IsCanDuck;
        public bool IsCanBreakBricks;

        [Header("Transition")]
        public AudioClip TransitionSound;
        public CharacterActor.BoxColliderInfo TransitionBoxColliderInfo;

        [Header("Duck")]
        public CharacterActor.BoxColliderInfo DuckBoxColliderInfo;

        [Header("Jump")]
        public AudioClip JumpSound;
    }
}
