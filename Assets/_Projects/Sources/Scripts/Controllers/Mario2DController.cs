using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;


public class Mario2DController : MonoBehaviour {
    [Header("Controller")]
    [SerializeField] private MarioStateMachine stateMachine = new MarioStateMachine();
    [SerializeField] private MarioStates[] marioStates = new MarioStates[0];
    [SerializeField] private float moveSpeed = .7f;
    [SerializeField] private float moveSpeedMultiplier = 6f;
    [SerializeField] private float airMoveSpeed = .5f;
    [SerializeField] private float airMoveSpeedMultiplier = 1.5f;
    [SerializeField] private float jumpSpeed = 10f;
    [SerializeField] private float maxJumpHeight = 2.4f;
    [SerializeField] private bool isFlipX = true;
    [SerializeField] private bool isFlipY = false;

    [Header("Debug")]
    [SerializeField] private Vector2 inputAxis;
    [SerializeField] private Vector2 lastJumpPos;
    [SerializeField] private bool isSprinting;
    [SerializeField] private bool isTransitioning;
    [SerializeField] private int curStateSetIndex;

    [Header("References")]
    [SerializeField] private Transform thisTransform;
    [SerializeField] private BoxCollider2D thisGroundCollider;
    [SerializeField] private BoxCollider2D thisCharacterCollider;
    [SerializeField] private Character2D thisCharacter2D;
    [SerializeField] private Animator thisAnimator;
    [SerializeField] private Transform thisCharacterObject;

    private IAudioController thisAudioController;


    public void SetStateSetIndex(int index) {
        curStateSetIndex = index;
        stateMachine.PushState(marioStates[curStateSetIndex].Transition);
    }

    public void Kill() {
        stateMachine.PushState(marioStates[curStateSetIndex].Dead);
    }

    public void Revive() {
        if(stateMachine.CurrentState == marioStates[curStateSetIndex].Dead) {
            stateMachine.PopState();
        }
    }

    private void UpdateFlipping() {
        Vector2 characterFlip = thisCharacterObject.localScale;
        //Vector2 rayDetectorFlip = thisRayCastColliderDetector2D.GetDetector(ColliderDetector2D.Direction.Up).Offset;

        if(isFlipX && thisCharacter2D.FaceAxis.x != 0f) {
            if(thisCharacter2D.FaceAxis.x != Mathf.Sign(thisCharacterObject.localScale.x)) {
                characterFlip.x *= -1f;
            }

            //if(thisCharacter2D.FaceAxis.x != Mathf.Sign(rayDetectorFlip.x)) {
            //    rayDetectorFlip.x *= -1f;
            //}
        }

        if(isFlipY && thisCharacter2D.FaceAxis.y != 0f) {
            if(thisCharacter2D.FaceAxis.y != Mathf.Sign(thisCharacterObject.localScale.y)) {
                characterFlip.y *= -1f;
            }

            //if(thisCharacter2D.FaceAxis.y != Mathf.Sign(rayDetectorFlip.y)) {
            //    rayDetectorFlip.y *= -1f;
            //}
        }

        thisCharacterObject.localScale = characterFlip;
        //thisRayCastColliderDetector2D.GetDetector(ColliderDetector2D.Direction.Up).Offset = rayDetectorFlip;
    }

    private void Awake() {
        thisAudioController = Singleton.Get<IAudioController>();
    }

    private void Start() {
        stateMachine.Init(this);
    }

    private void Update() {
        inputAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        isSprinting = Input.GetKey(KeyCode.LeftShift);

        // There must be a better way to handle the
        // state transitions than this horrendous
        // condition checking
        if(stateMachine.CurrentState != marioStates[curStateSetIndex].Transition &&
           stateMachine.CurrentState != marioStates[curStateSetIndex].Dead) {
            if(thisCharacter2D.IsGrounded) {
                if(!thisCharacter2D.IsMoving(ColliderDetector2D.Direction.Any) &&
                    inputAxis == Vector2.zero) {
                    stateMachine.SetState(marioStates[curStateSetIndex].Idle);
                }

                if(inputAxis.x != 0 &&
                   inputAxis.y == 0 &&
                   !Input.GetKey(KeyCode.Space) &&
                   !thisCharacter2D.IsChangingDirection) {
                    stateMachine.SetState(marioStates[curStateSetIndex].Walk);
                }

                if(inputAxis.x == 0 &&
                   inputAxis.y < 0f &&
                   marioStates[curStateSetIndex].IsCanDuck &&
                   !Input.GetKey(KeyCode.Space)) {
                    stateMachine.PushState(marioStates[curStateSetIndex].Duck);
                } else {
                    if(stateMachine.CurrentState == marioStates[curStateSetIndex].Duck) {
                        stateMachine.PopState();
                    }
                }

                if(thisCharacter2D.IsChangingDirection) {
                    stateMachine.SetState(marioStates[curStateSetIndex].Slide);
                }

                if(Input.GetKey(KeyCode.Space)) {
                    stateMachine.SetState(marioStates[curStateSetIndex].Jump);
                }
            } else {
                if(Mathf.Abs(thisTransform.localPosition.y - lastJumpPos.y) > maxJumpHeight ||
                    thisCharacter2D.ColliderDetector.IsColliding(ColliderDetector2D.Direction.Up) ||
                    !Input.GetKey(KeyCode.Space)) {
                    stateMachine.SetState(marioStates[curStateSetIndex].Fall);
                }
            }
        }

        UpdateFlipping();

        stateMachine.CurrentStateText = stateMachine.CurrentState.ToString();
        stateMachine.Update();

        // Debug code
        if(Input.GetKeyDown(KeyCode.E)) {
            if(curStateSetIndex == 0) curStateSetIndex = marioStates.Length - 1;
            else curStateSetIndex--;

            SetStateSetIndex(curStateSetIndex);
        }

        if(Input.GetKeyDown(KeyCode.R)) {
            if(curStateSetIndex == marioStates.Length - 1) curStateSetIndex = 0;
            else curStateSetIndex++;

            SetStateSetIndex(curStateSetIndex);
        }

        if(Input.GetKeyDown(KeyCode.T)) {
            Kill();
        }

        if(Input.GetKeyDown(KeyCode.Y)) {
            Revive();
        }
    }

    private void FixedUpdate() {
        stateMachine.FixedUpdate();
    }


    [Serializable]
    public class MarioStateMachine : StateMachine<Mario2DController> {
        public string CurrentStateText;
    }


    [Serializable]
    public class MarioStates {
        public bool IsCanDuck;

        public IdleState Idle;
        public JumpState Jump;
        public DeadState Dead;
        public SlideState Slide;
        public WalkState Walk;
        public FallState Fall;
        public DuckState Duck;
        public TransitionState Transition;
    }


    [Serializable]
    public class IdleState : State<Mario2DController> {
        public override void OnEnter(Mario2DController owner) {
            owner.thisAnimator.PlayNoRepeat("Idle" + owner.curStateSetIndex);
        }

        public override void OnUpdate(Mario2DController owner) {
            
        }

        public override void OnExit(Mario2DController owner) {
            
        }
    }


    [Serializable]
    public class JumpState : State<Mario2DController> {
        [SerializeField] private AudioClip jumpSound;


        public override void OnEnter(Mario2DController owner) {
            owner.lastJumpPos = owner.thisTransform.localPosition;
            owner.thisAnimator.PlayNoRepeat("Jump" + owner.curStateSetIndex);
            owner.thisAudioController.PlayOneShot(jumpSound);
        }

        public override void OnUpdate(Mario2DController owner) {
            
        }

        public override void OnFixedUpdate(Mario2DController owner) {
            float airMoveSpeed = owner.airMoveSpeed * (owner.isSprinting ? owner.airMoveSpeedMultiplier : 1f);
            owner.thisCharacter2D.Move(new Vector2(owner.inputAxis.x * airMoveSpeed, 1f * owner.jumpSpeed) * Time.fixedDeltaTime);
        }

        public override void OnExit(Mario2DController owner) {
            
        }
    }


    [Serializable]
    public class DeadState : State<Mario2DController> {
        [SerializeField] private AudioClip deathSound;


        public override void OnEnter(Mario2DController owner) {
            Time.timeScale = 0f;
            owner.thisAnimator.PlayNoRepeat("Dead" + owner.curStateSetIndex);
            owner.thisAudioController.PlayOneShot(deathSound);
        }

        public override void OnUpdate(Mario2DController owner) {

        }

        public override void OnExit(Mario2DController owner) {
            Time.timeScale = 1f;
        }
    }


    [Serializable]
    public class SlideState : State<Mario2DController> {
        public override void OnEnter(Mario2DController owner) {
            owner.thisAnimator.PlayNoRepeat("Slide" + owner.curStateSetIndex);
        }

        public override void OnUpdate(Mario2DController owner) {

        }

        public override void OnExit(Mario2DController owner) {

        }
    }


    [Serializable]
    public class WalkState : State<Mario2DController> {
        public override void OnEnter(Mario2DController owner) {
            owner.thisAnimator.PlayNoRepeat("Walk" + owner.curStateSetIndex);
            
        }

        public override void OnUpdate(Mario2DController owner) {
            owner.thisAnimator.SetFloat("WalkSpeedMultiplier",
                                        2.5f * Mathf.Max(0.1f, Mathf.InverseLerp(0f, owner.thisCharacter2D.MaxVelocity.x,
                                                                                     Mathf.Abs(owner.thisCharacter2D.Velocity.x))));
        }

        public override void OnFixedUpdate(Mario2DController owner) {
            float moveSpeed = owner.moveSpeed * (owner.isSprinting ? owner.moveSpeedMultiplier : 1f);
            owner.thisCharacter2D.Move(new Vector2(owner.inputAxis.x, 0f) * moveSpeed * Time.fixedDeltaTime);
        }

        public override void OnExit(Mario2DController owner) {

        }
    }


    [Serializable]
    public class FallState : State<Mario2DController> {
        public override void OnEnter(Mario2DController owner) {
            
        }

        public override void OnUpdate(Mario2DController owner) {
            if(owner.thisCharacter2D.Velocity.y < 0f) {
                owner.thisAnimator.PlayNoRepeat("Fall" + owner.curStateSetIndex);
            }
        }

        public override void OnFixedUpdate(Mario2DController owner) {
            float airMoveSpeed = owner.airMoveSpeed * (owner.isSprinting ? owner.airMoveSpeedMultiplier : 1f);
            owner.thisCharacter2D.Move(new Vector2(owner.inputAxis.x * airMoveSpeed, 0f) * Time.fixedDeltaTime);
        }

        public override void OnExit(Mario2DController owner) {

        }
    }


    [Serializable]
    public class DuckState : State<Mario2DController> {
        public override void OnEnter(Mario2DController owner) {
            owner.thisAnimator.PlayNoRepeat("Duck" + owner.curStateSetIndex);
        }

        public override void OnUpdate(Mario2DController owner) {

        }

        public override void OnExit(Mario2DController owner) {

        }
    }


    [Serializable]
    public class TransitionState : State<Mario2DController> {
        [SerializeField] private AudioClip transitionSound;


        public override void OnEnter(Mario2DController owner) {
            Time.timeScale = 0f;
            owner.thisAnimator.PlayNoRepeat("Transition" + owner.curStateSetIndex);
            owner.thisAudioController.PlayOneShot(transitionSound);
        }

        public override void OnUpdate(Mario2DController owner) {
            if(owner.thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("Transition" + owner.curStateSetIndex)) {
                if(owner.thisAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > .95f) {
                    Time.timeScale = 1f;
                    owner.stateMachine.PopState();
                }
            }
        }

        public override void OnExit(Mario2DController owner) {

        }
    }
}
