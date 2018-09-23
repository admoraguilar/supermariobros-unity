using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Character2D))]
public class Mario2DController : MonoBehaviour {
    [Header("Controller")]
    [SerializeField] private float                              moveSpeed = .7f;
    [SerializeField] private float                              moveSpeedMultiplier = 6f;
    [SerializeField] private float                              airMoveSpeed = .5f;
    [SerializeField] private float                              airMoveSpeedMultiplier = 1.5f;
    [SerializeField] private float                              jumpSpeed = 10f;
    [SerializeField] private float                              maxJumpHeight = 2.4f;
    [SerializeField] private bool                               isFlipX = true;
    [SerializeField] private bool                               isFlipY = false;
    [SerializeField] private State[]                            states;

    [Header("Debug")]
    [SerializeField] private BoxColliderInfo                    overrideColliderInfo;
    [SerializeField] private Vector2                            inputAxis;
    [SerializeField] private Vector2                            lastPositionBeforeJumping;
    [SerializeField] private float                              speed;
    [SerializeField] private bool                               isJumping;
    [SerializeField] private bool                               isSprinting;
    [SerializeField] private bool                               isTransitioning;
    [SerializeField] private int                                curStateIndex;
    [SerializeField] private int                                prevStateIndex;

    [Header("References")]
    [SerializeField] private Transform                          thisTransform;
    [SerializeField] private BoxCollider2D                      thisGroundCollider;
    [SerializeField] private BoxCollider2D                      thisCharacterCollider;
    [SerializeField] private Rigidbody2D                        thisRigidbody2D;
    [SerializeField] private BoxCastColliderDetector2D          thisBoxCastColliderDetector2D;
    [SerializeField] private RayCastColliderDetector2D          thisRayCastColliderDetector2D;
    [SerializeField] private ColliderDetector2DInteractor       thisColliderDetector2DInteractor;
    [SerializeField] private Character2D                        thisCharacter2D;
    [SerializeField] private Animator                           thisAnimator;
    [SerializeField] private Transform                          thisCharacterObject;

    private IAudioController                                    thisAudioController;
    

    public void SetState(int stateIndex) {
        prevStateIndex = curStateIndex;
        curStateIndex = stateIndex;
        isTransitioning = true;

        // Play some transition sound
        thisAudioController.PlayOneShot(states[curStateIndex].TransitionSound);
    }

    public void SetInputAxis(Vector2 axis) {
        inputAxis = axis;
    }

    private void SetOverrideCollider(BoxColliderInfo colliderInfo) {
        overrideColliderInfo = colliderInfo;
    }

    private void HookInteraction() {
        thisColliderDetector2DInteractor.OnInteractEnter += (ColliderDetector2D.Direction direction, Interactable interactable) => {
            interactable.Interact(thisColliderDetector2DInteractor);
            Debug.Log(interactable.name);
        };
    }

    private void UpdateAnimation() {
        if(isTransitioning) {
            // Pause while transitioning, this is that mario effect that
            // everything pauses when picking up a power-up
            Time.timeScale = 0f;

            if(thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("Transition" + curStateIndex)) {
                if(thisAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > .95f) {
                    Time.timeScale = 1f;
                    isTransitioning = false;
                }
            } else {
                thisAnimator.PlayNoRepeat("Transition" + curStateIndex);
            }
        } else {
            if(thisCharacter2D.IsGrounded && !thisCharacter2D.IsMoving(ColliderDetector2D.Direction.Any) &&
               inputAxis == Vector2.zero) {
                thisAnimator.PlayNoRepeat("Idle" + curStateIndex);
            }

            if(!thisCharacter2D.IsGrounded && isJumping) {
                thisAnimator.PlayNoRepeat("Jump" + curStateIndex);
            }

            if(!thisCharacter2D.IsGrounded && !isJumping && thisRigidbody2D.velocity.y < 0f) {
                thisAnimator.PlayNoRepeat("Fall" + curStateIndex);
            }

            if(thisCharacter2D.IsGrounded && inputAxis.y < 0f &&
               states[curStateIndex].IsCanDuck) {
                thisAnimator.PlayNoRepeat("Duck" + curStateIndex);
                thisCharacterCollider.offset = states[curStateIndex].DuckColliderInfo.Offset;
                thisCharacterCollider.size = states[curStateIndex].DuckColliderInfo.Size;
            } else {
                thisCharacterCollider.offset = states[curStateIndex].ColliderInfo.Offset;
                thisCharacterCollider.size = states[curStateIndex].ColliderInfo.Size;
            }

            if(thisCharacter2D.IsGrounded) {
                if(thisCharacter2D.IsChangingDirection) {
                    thisAnimator.PlayNoRepeat("Slide" + curStateIndex);
                } else if(thisCharacter2D.Velocity.x != 0f && inputAxis.y == 0f) {
                    thisAnimator.PlayNoRepeat("Walk" + curStateIndex);
                    thisAnimator.SetFloat("WalkSpeedMultiplier",
                                          2.5f * Mathf.Max(0.1f, Mathf.InverseLerp(0f, thisCharacter2D.MaxVelocity.x, Mathf.Abs(thisCharacter2D.Velocity.x))));
                }
            }
        }
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
        HookInteraction();
    }

    private void Update() {
        inputAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        isSprinting = Input.GetKey(KeyCode.LeftShift);

        // Enable jumping when we are grounded
        if(thisCharacter2D.IsGrounded) {
            if(Input.GetKeyDown(KeyCode.Space)) {
                lastPositionBeforeJumping = thisTransform.localPosition;
                thisAudioController.PlayOneShot(states[curStateIndex].JumpSound);
                isJumping = true;
            }
        }

        // Jump constraints
        if(Mathf.Abs(thisTransform.localPosition.y - lastPositionBeforeJumping.y) > maxJumpHeight ||
           Input.GetKeyUp(KeyCode.Space) ||
           thisCharacter2D.ColliderDetector.IsColliding(ColliderDetector2D.Direction.Up)) {
            isJumping = false;
        }

        UpdateFlipping();
        UpdateAnimation();

        // Debug code
        if(Input.GetKeyDown(KeyCode.E)) {
            if(curStateIndex == 0) curStateIndex = states.Length - 1;
            else curStateIndex--;

            SetState(curStateIndex);
        } else if(Input.GetKeyDown(KeyCode.R)) {
            if(curStateIndex == states.Length - 1) curStateIndex = 0;
            else curStateIndex++;

            SetState(curStateIndex);
        }
    }

    private void FixedUpdate() {
        // Horizontal movement
        if(inputAxis != Vector2.zero) {
            if(inputAxis.y == 0) {
                speed = thisCharacter2D.IsGrounded ? (moveSpeed * (isSprinting ? moveSpeedMultiplier : 1f)) 
                        : airMoveSpeed * (isSprinting ? airMoveSpeedMultiplier : 1f);
                thisCharacter2D.Move(new Vector2(inputAxis.x, 0f) * speed * Time.fixedDeltaTime);
            }
        }

        // Vertical movement
        if(isJumping) {
            thisCharacter2D.Move(new Vector2(0f, 1f) * jumpSpeed * Time.fixedDeltaTime);
        }
    }

    private void Reset() {
        thisTransform = GetComponent<Transform>();


        /*
         *  Setup Mario2DController2D: 
         */
        // Setup states
        states = new State[3] {
            new State() {
                StateLabel = StateLabel.Small,
                ColliderInfo = new BoxColliderInfo {
                    Offset = Vector2.zero,
                    Size = new Vector3(0.65f, 0.8f),
                    GizmoColor = Color.green
                },
                DuckColliderInfo = new BoxColliderInfo {
                    Offset = new Vector2(0f, 0.15f),
                    Size = new Vector2(0.8f, 1.11f),
                    GizmoColor = Color.clear
                },
                IsCanDuck = false,
                IsCanFire = false
            },
            new State() {
                StateLabel = StateLabel.Big,
                ColliderInfo = new BoxColliderInfo {
                    Offset = new Vector2(0f, 0.4f),
                    Size = new Vector2(0.8f, 1.6f),
                    GizmoColor = Color.blue
                },
                DuckColliderInfo = new BoxColliderInfo {
                    Offset = new Vector2(0f, 0.15f),
                    Size = new Vector2(0.8f, 1.11f),
                    GizmoColor = Color.cyan
                },
                IsCanDuck = true,
                IsCanFire = false
            },
            new State() {
                StateLabel = StateLabel.Power,
                ColliderInfo = new BoxColliderInfo {
                    Offset =  new Vector2(0f, 0.4f),
                    Size = new Vector2(0.8f, 1.6f),
                    GizmoColor = Color.blue
                },
                DuckColliderInfo = new BoxColliderInfo {
                    Offset = new Vector2(0f, 0.15f),
                    Size = new Vector2(0.8f, 1.11f),
                    GizmoColor = Color.cyan
                },
                IsCanDuck = true,
                IsCanFire = false
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
        for(int i = 0; i < states.Length; i++) {
            Gizmos.color = states[i].ColliderInfo.GizmoColor;
            Gizmos.DrawWireCube(thisTransform.position + states[i].ColliderInfo.Offset, states[i].ColliderInfo.Size);

            Gizmos.color = states[i].DuckColliderInfo.GizmoColor;
            Gizmos.DrawWireCube(thisTransform.position + states[i].DuckColliderInfo.Offset, states[i].DuckColliderInfo.Size);
        }
    }


    [Serializable]
    public class State {
        public StateLabel           StateLabel;
        public BoxColliderInfo      ColliderInfo;
        public BoxColliderInfo      DuckColliderInfo;
        public AudioClip            JumpSound;
        public AudioClip            TransitionSound;
        public bool                 IsCanDuck;
        public bool                 IsCanFire;
        public bool                 IsStateAdditive;
    }

    [Serializable]
    public class BoxColliderInfo {
        public Vector3 Offset;
        public Vector3 Size;

        [Header("Debug")]
        public Color GizmoColor = new Color(1f, 1f, 1f, 1f);
    }

    public enum StateLabel {
        Small,
        Big,
        Power
    }
}
