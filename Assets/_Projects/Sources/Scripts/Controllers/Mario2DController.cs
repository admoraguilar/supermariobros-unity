using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;


[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCastColliderDetector2D))]
[RequireComponent(typeof(RayCastColliderDetector2D))]
[RequireComponent(typeof(ColliderDetector2DInteractor))]
[RequireComponent(typeof(Character2D))]
public class Mario2DController : MonoBehaviour {
    public MarioStates CurrentStateSet { get { return marioStates[curStateSetIndex]; } }

    [Header("Controller")]
    [SerializeField] private MarioStateMachine                          stateMachine = new MarioStateMachine();
    [SerializeField] private MarioStates[]                              marioStates = new MarioStates[0];
    [SerializeField] private float                                      moveSpeed = .7f;
    [SerializeField] private float                                      moveSpeedMultiplier = 6f;
    [SerializeField] private float                                      airMoveSpeed = .5f;
    [SerializeField] private float                                      airMoveSpeedMultiplier = 1.5f;
    [SerializeField] private float                                      jumpSpeed = 10f;
    [SerializeField] private float                                      maxJumpHeight = 2.4f;
    [SerializeField] private bool                                       isFlipX = true;
    [SerializeField] private bool                                       isFlipY = false;

    [Header("Debug")]
    [SerializeField] private Vector2                                    inputAxis;
    [SerializeField] private Vector2                                    lastJumpPos;
    [SerializeField] private bool                                       isSprinting;
    [SerializeField] private bool                                       isTransitioning;
    [SerializeField] private int                                        curStateSetIndex;

    [Header("References")]
    [SerializeField] private Transform                                  thisTransform;
    [SerializeField] private BoxCollider2D                              thisGroundCollider;
    [SerializeField] private BoxCollider2D                              thisCharacterCollider;
    [SerializeField] private Rigidbody2D                                thisRigidbody2D;
    [SerializeField] private BoxCastColliderDetector2D                  thisBoxCastColliderDetector2D;
    [SerializeField] private RayCastColliderDetector2D                  thisRayCastColliderDetector2D;
    [SerializeField] private ColliderDetector2DInteractor               thisColliderDetector2DInteractor;
    [SerializeField] private Character2D                                thisCharacter2D;
    [SerializeField] private Animator                                   thisAnimator;
    [SerializeField] private Transform                                  thisCharacterObject;

    private IAudioController                                            thisAudioController;


    public void SetStateSetIndex(int index) {
        curStateSetIndex = index;
        stateMachine.PushState(marioStates[curStateSetIndex].Transition);
    }

    public void PowerUp(bool value) {
        int index = curStateSetIndex;

        if(value) {
            //if(index++ == marioStates.Length - 1) index = 0;
            if(index++ == marioStates.Length - 1) return;
        } else {
            //if(index++ == 0) index = marioStates.Length - 1;
            if(index == 0) return;
            else index = 0;
        }

        SetStateSetIndex(index);
    }

    public void Kill(bool value) {
        if(value) {
            stateMachine.PushState(marioStates[curStateSetIndex].Dead);
        } else {
            if(stateMachine.CurrentState == marioStates[curStateSetIndex].Dead) {
                stateMachine.PopState();
            }
        }
    }

    private void HookInteraction() {
        thisColliderDetector2DInteractor.OnInteractEnter += (ColliderDetector2D.Direction direction, Interactable interactable) => {
            interactable.Interact(thisColliderDetector2DInteractor);
        };
    }

    private void UpdateFlipping() {
        Vector2 characterFlip = thisCharacterObject.localScale;
        Vector2 rayDetectorFlip = thisRayCastColliderDetector2D.GetDetector(ColliderDetector2D.Direction.Up).Offset;

        if(isFlipX && thisCharacter2D.FaceAxis.x != 0f) {
            if(thisCharacter2D.FaceAxis.x != Mathf.Sign(thisCharacterObject.localScale.x)) {
                characterFlip.x *= -1f;
            }

            if(thisCharacter2D.FaceAxis.x != Mathf.Sign(rayDetectorFlip.x)) {
                rayDetectorFlip.x *= -1f;
            }
        }

        if(isFlipY && thisCharacter2D.FaceAxis.y != 0f) {
            if(thisCharacter2D.FaceAxis.y != Mathf.Sign(thisCharacterObject.localScale.y)) {
                characterFlip.y *= -1f;
            }

            if(thisCharacter2D.FaceAxis.y != Mathf.Sign(rayDetectorFlip.y)) {
                rayDetectorFlip.y *= -1f;
            }
        }

        thisCharacterObject.localScale = characterFlip;
        thisRayCastColliderDetector2D.GetDetector(ColliderDetector2D.Direction.Up).Offset = rayDetectorFlip;
    }

    private void Awake() {
        thisAudioController = Singleton.Get<IAudioController>();
    }

    private void Start() {
        stateMachine.Init(this);

        HookInteraction();
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
                    inputAxis == Vector2.zero &&
                    !Input.GetKey(KeyCode.Space)) {
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

                if(thisCharacter2D.IsChangingDirection &&
                   !Input.GetKey(KeyCode.Space)) {
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

        stateMachine.Update();

        // Debug code
        if(Input.GetKeyDown(KeyCode.E)) {
            PowerUp(false);
        }

        if(Input.GetKeyDown(KeyCode.R)) {
            PowerUp(true);
        }

        if(Input.GetKeyDown(KeyCode.T)) {
            Kill(true);
        }

        if(Input.GetKeyDown(KeyCode.Y)) {
            Kill(false);
        }
    }

    private void FixedUpdate() {
        stateMachine.FixedUpdate();
    }

    private void Reset() {
        thisTransform = GetComponent<Transform>();

        /*
         *  Setup Mario2DController
         */
        // Setup states
        marioStates = new MarioStates[3] {
            new MarioStates {
                Duck = new DuckState {
                    BoxColliderInfo = new BoxColliderInfo {
                        Offset = new Vector2(0f, 0.15f),
                        Size = new Vector2(0.8f, 1.11f),
                        GizmoColor = Color.white
                    }
                },
                Transition = new TransitionState {
                    BoxColliderInfo = new BoxColliderInfo {
                        Offset = new Vector2(0f, 0f),
                        Size = new Vector2(0.65f, 0.8f),
                        GizmoColor = Color.green
                    }
                }
            },
            new MarioStates {
                Duck = new DuckState {
                    BoxColliderInfo = new BoxColliderInfo {
                        Offset = new Vector2(0f, 0.15f),
                        Size = new Vector2(0.8f, 1.11f),
                        GizmoColor = Color.blue
                    }
                },
                Transition = new TransitionState {
                    BoxColliderInfo = new BoxColliderInfo {
                        Offset = new Vector2(0f, 0.4f),
                        Size = new Vector2(0.8f, 1.6f),
                        GizmoColor = Color.blue
                    }
                }
            },
            new MarioStates {
                Duck = new DuckState {
                    BoxColliderInfo = new BoxColliderInfo {
                        Offset = new Vector2(0f, 0.15f),
                        Size = new Vector2(0.8f, 1.11f),
                        GizmoColor = Color.red + Color.yellow
                    }
                },
                Transition = new TransitionState {
                    BoxColliderInfo = new BoxColliderInfo {
                        Offset = new Vector2(0f, 0.4f),
                        Size = new Vector2(0.8f, 1.6f),
                        GizmoColor = Color.red + Color.yellow
                    }
                }
            }
        };


        /* 
         *  Setup Collider2D:
         *      We use two colliders here because we modify the character collider at runtime.
         *      If we only use one collider, when we modify its size the Physics system will recalculate
         *      bounds and it'll make our collider fall for a bit which is not a behaviour we want.
         */
        thisGroundCollider = this.AddComponentAsChildObject<BoxCollider2D>(
           thisTransform,
           string.Format("{0}/GroundCollider",
               GetType().Name));
        thisGroundCollider.offset = new Vector2(0f, -.35f);
        thisGroundCollider.size = new Vector2(0.65f, 0.1f);

        thisCharacterCollider = this.AddComponentAsChildObject<BoxCollider2D>(
            thisTransform,
            string.Format("{0}/CharacterCollider",
                GetType().Name));
        thisCharacterCollider.offset = new Vector2(0f, 0f);
        thisCharacterCollider.size = new Vector2(0.65f, 0.8f);


        /* 
         *  Setup Rigidbody2D
         */
        thisRigidbody2D = this.AddOrGetComponent<Rigidbody2D>();
        thisRigidbody2D.sharedMaterial = new PhysicsMaterial2D("Slippery") {
            friction = .05f
        };
        thisRigidbody2D.mass = 0.001f;
        thisRigidbody2D.drag = 2f;
        thisRigidbody2D.gravityScale = 5f;
        thisRigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        thisRigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;


        /* 
         *  Setup ColliderDetector2D: 
         */
        // (Colliders)
        thisBoxCastColliderDetector2D = this.AddOrGetComponent<BoxCastColliderDetector2D>();

        // Generate preset colliders
        thisBoxCastColliderDetector2D.Init(new BoxCastColliderDetector2D.BoxCastDetectorInfo[4] {
            new BoxCastColliderDetector2D.BoxCastDetectorInfo {
                Direction = ColliderDetector2D.Direction.Up,
                ReferenceCollider = thisCharacterCollider,
                SizeMultiplier = 1f,
                Distance = 0.2f
            },
            new BoxCastColliderDetector2D.BoxCastDetectorInfo {
                Direction = ColliderDetector2D.Direction.Down,
                ReferenceCollider = thisCharacterCollider,
                SizeMultiplier = 1f,
                Distance = 0.2f
            },
            new BoxCastColliderDetector2D.BoxCastDetectorInfo {
                Direction = ColliderDetector2D.Direction.Left,
                ReferenceCollider = thisCharacterCollider,
                SizeMultiplier = 1f,
                Distance = 0.2f
            },
            new BoxCastColliderDetector2D.BoxCastDetectorInfo {
                Direction = ColliderDetector2D.Direction.Right,
                ReferenceCollider = thisCharacterCollider,
                SizeMultiplier = 1f,
                Distance = 0.2f
            }
        });

        // We ignore some colliders
        thisBoxCastColliderDetector2D.Init(GetComponentsInChildren<Collider2D>(true));

        // (Interactables)
        thisRayCastColliderDetector2D = this.AddOrGetComponent<RayCastColliderDetector2D>();

        // Generate preset colliders
        thisRayCastColliderDetector2D.Init(new RayCastColliderDetector2D.RayCastDetectorInfo[4] {
            new RayCastColliderDetector2D.RayCastDetectorInfo {
                Direction = ColliderDetector2D.Direction.Up,
                ReferenceCollider = thisCharacterCollider,
                Offset = new Vector2(0.1f, 0f),
                Distance = 0.2f
            },
            new RayCastColliderDetector2D.RayCastDetectorInfo {
                Direction = ColliderDetector2D.Direction.Down,
                ReferenceCollider = thisCharacterCollider,
                Offset = Vector2.zero,
                Distance = 0.2f
            },
            new RayCastColliderDetector2D.RayCastDetectorInfo {
                Direction = ColliderDetector2D.Direction.Left,
                ReferenceCollider = thisCharacterCollider,
                Offset = Vector2.zero,
                Distance = 0.2f
            },
            new RayCastColliderDetector2D.RayCastDetectorInfo {
                Direction = ColliderDetector2D.Direction.Right,
                ReferenceCollider = thisCharacterCollider,
                Offset = Vector2.zero,
                Distance = 0.2f
            }
        });

        // We ignore some colliders
        thisRayCastColliderDetector2D.Init(GetComponentsInChildren<Collider2D>(true));


        /*
         *  Setup ColliderDetector2DInteractor
         */
        thisColliderDetector2DInteractor = this.AddOrGetComponent<ColliderDetector2DInteractor>();
        thisColliderDetector2DInteractor.Init(thisRayCastColliderDetector2D);


        /*
         *  Setup Character2D
         */
        thisCharacter2D = this.AddOrGetComponent<Character2D>();
        thisCharacter2D.Init(thisRigidbody2D, thisBoxCastColliderDetector2D);


        /* 
         *  Setup Animator
         */
        thisAnimator = GetComponentInChildren<Animator>(true);
        if(thisAnimator) {
            thisCharacterObject = thisAnimator.GetComponent<Transform>();
        }
    }

    private void OnDrawGizmos() {
        for(int i = 0; i < marioStates.Length; i++) {
            // Duck collider
            Gizmos.color = marioStates[i].Duck.BoxColliderInfo.GizmoColor;
            Gizmos.DrawWireCube(new Vector2(thisTransform.position.x, thisTransform.position.y) + marioStates[i].Duck.BoxColliderInfo.Offset,
                                marioStates[i].Duck.BoxColliderInfo.Size);

            // Transition collider 
            Gizmos.color = marioStates[i].Transition.BoxColliderInfo.GizmoColor;
            Gizmos.DrawWireCube(new Vector2(thisTransform.position.x, thisTransform.position.y) + marioStates[i].Transition.BoxColliderInfo.Offset,
                                marioStates[i].Transition.BoxColliderInfo.Size);
            
        }
    }


    [Serializable]
    public class MarioStateMachine : StateMachine<Mario2DController> { }


    [Serializable]
    public class MarioStates {
        public bool                 IsCanDuck;
        public bool                 IsCanBreakBrick;

        public IdleState            Idle;
        public JumpState            Jump;
        public DeadState            Dead;
        public SlideState           Slide;
        public WalkState            Walk;
        public FallState            Fall;
        public DuckState            Duck;
        public TransitionState      Transition;
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
        public AudioClip JumpSound;


        public override void OnEnter(Mario2DController owner) {
            owner.lastJumpPos = owner.thisTransform.localPosition;
            owner.thisAnimator.PlayNoRepeat("Jump" + owner.curStateSetIndex);
            owner.thisAudioController.PlayOneShot(JumpSound);
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
        public AudioClip DeathSound;


        public override void OnEnter(Mario2DController owner) {
            Time.timeScale = 0f;
            owner.thisAnimator.PlayNoRepeat("Dead" + owner.curStateSetIndex);
            owner.thisAudioController.PlayOneShot(DeathSound);
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
        public BoxColliderInfo BoxColliderInfo;


        public override void OnEnter(Mario2DController owner) {
            owner.thisAnimator.PlayNoRepeat("Duck" + owner.curStateSetIndex);

            owner.thisCharacterCollider.offset = BoxColliderInfo.Offset;
            owner.thisCharacterCollider.size = BoxColliderInfo.Size;
        }

        public override void OnUpdate(Mario2DController owner) {

        }

        public override void OnExit(Mario2DController owner) {
            owner.thisCharacterCollider.offset = owner.marioStates[owner.curStateSetIndex].Transition.BoxColliderInfo.Offset;
            owner.thisCharacterCollider.size = owner.marioStates[owner.curStateSetIndex].Transition.BoxColliderInfo.Size;
        }
    }


    [Serializable]
    public class TransitionState : State<Mario2DController> {
        public AudioClip            TransitionSound;
        public BoxColliderInfo      BoxColliderInfo;


        public override void OnEnter(Mario2DController owner) {
            Time.timeScale = 0f;
            owner.thisAnimator.PlayNoRepeat("Transition" + owner.curStateSetIndex);
            owner.thisAudioController.PlayOneShot(TransitionSound);
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
            owner.thisCharacterCollider.offset = BoxColliderInfo.Offset;
            owner.thisCharacterCollider.size = BoxColliderInfo.Size;
        }
    }


    [Serializable]
    public class BoxColliderInfo {
        public Vector2 Offset;
        public Vector2 Size;

        [Header("Debug")]
        public Color GizmoColor = new Color(1f, 1f, 1f, 1f);
    }
}
