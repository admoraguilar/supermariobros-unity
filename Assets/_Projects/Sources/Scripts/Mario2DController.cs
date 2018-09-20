using UnityEngine;
using System;
using System.Collections.Generic;


[RequireComponent(typeof(Character2D))]
public class Mario2DController : MonoBehaviour {
    public int StateIndex { get { return currentStateIndex; } }

    [Header("Controller")]
    [SerializeField] private float moveSpeed = .7f;
    [SerializeField] private float moveSpeedMultiplier = 6f;
    [SerializeField] private float airMoveSpeed = .5f;
    [SerializeField] private float airMoveSpeedMultiplier = 1.5f;
    [SerializeField] private float jumpSpeed = 10f;
    [SerializeField] private float maxJumpHeight = 2.4f;
    [SerializeField] private bool isFlipX = true, isFlipY = false;
    [SerializeField] private MarioState[] marioStates;
    [SerializeField] private Transform characterObject;

    [Header("Audio")]
    [SerializeField] private AudioClip powerUpSound;
    [SerializeField] private AudioClip powerDownSound;

    [Header("Debug")]
    [SerializeField] private Vector2 inputAxis;
    [SerializeField] private Vector2 lastPositionBeforeJumping;
    [SerializeField] private BoxCollider2DInfo overrideCollider = new BoxCollider2DInfo();
    [SerializeField] private float speed;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isSprinting;
    [SerializeField] private bool isTransitioning;
    [SerializeField] private int currentStateIndex;

    [Header("References")]
    [SerializeField] private Animator thisAnimator;
    [SerializeField] private AnimationEventsHook thisAnimationEventsHook;
    private Transform thisTransform;
    private Character2D thisCharacter2D;
    private AudioController thisAudioController;
    private BoxCollider2D thisCharacterCollider2D;
    private Interactor thisInteractor;


    public void SetState(int stateIndex) {
        currentStateIndex = stateIndex;
        isTransitioning = true;
    }

    private void HookAudio() {
        thisAnimationEventsHook.AddEvent("OnJump", () => {
            thisAudioController.PlayOneShot(marioStates[currentStateIndex].JumpSound);
        });

        thisAnimationEventsHook.AddEvent("OnPowerUp", () => {
            thisAudioController.PlayOneShot(powerUpSound);
        });

        thisAnimationEventsHook.AddEvent("OnPowerDown", () => {
            thisAudioController.PlayOneShot(powerDownSound);
        });
    }

    private void HookInteraction() {
        thisInteractor.OnInteractEnter += (Direction direction, Interactable interactable) => {
            interactable.Interact(thisInteractor);
        };
    }

    private void UpdateFlipping() {
        Vector2 characterFlip = characterObject.localScale;
        Vector2 rayDetectorFlip = thisCharacter2D.RayColliderDetector.UpRay.Offset;

        if(isFlipX && thisCharacter2D.FaceAxis.x != 0f) {
            if(thisCharacter2D.FaceAxis.x != Mathf.Sign(characterObject.localScale.x)) {
                characterFlip.x *= -1f;
            }

            if(thisCharacter2D.FaceAxis.x != Mathf.Sign(rayDetectorFlip.x)) {
                rayDetectorFlip.x *= -1f;
            }
        }

        if(isFlipY && thisCharacter2D.FaceAxis.y != 0f) {
            if(thisCharacter2D.FaceAxis.y != Mathf.Sign(characterObject.localScale.y)) {
                characterFlip.y *= -1f;
            }

            if(thisCharacter2D.FaceAxis.y != Mathf.Sign(rayDetectorFlip.y)) {
                rayDetectorFlip.y *= -1f;
            }
        }

        characterObject.localScale = characterFlip;
        thisCharacter2D.RayColliderDetector.UpRay.Offset = rayDetectorFlip;
    }

    private void UpdateAnimation() {
        if(isTransitioning) {
            // Pause while transitioning, this is that mario effect that
            // everything pauses when picking up a power-up
            Time.timeScale = 0f;

            if(thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("Transition" + currentStateIndex)) {
                if(thisAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > .95f) {
                    thisCharacterCollider2D.offset = marioStates[currentStateIndex].ColliderInfo[0].Offset;
                    thisCharacterCollider2D.size = marioStates[currentStateIndex].ColliderInfo[0].Size;

                    Time.timeScale = 1f;
                    isTransitioning = false;
                }
            } else {
                thisAnimator.PlayNoRepeat("Transition" + currentStateIndex);
            }
        } else {
            if(thisCharacter2D.IsGrounded && !thisCharacter2D.IsMoving(Direction.Any) &&
               inputAxis == Vector2.zero) {
                thisAnimator.PlayNoRepeat("Idle" + currentStateIndex);
            }

            if(!thisCharacter2D.IsGrounded && isJumping) {
                thisAnimator.PlayNoRepeat("Jump" + currentStateIndex);
            }

            if(!thisCharacter2D.IsGrounded && !isJumping) {
                thisAnimator.PlayNoRepeat("Fall" + currentStateIndex);
            }

            if(thisCharacter2D.IsGrounded && inputAxis.y < 0f &&
               marioStates[currentStateIndex].IsCanDuck) {
                thisAnimator.PlayNoRepeat("Duck" + currentStateIndex);
                overrideCollider.Offset = marioStates[currentStateIndex].ColliderInfo[1].Offset;
                overrideCollider.Size = marioStates[currentStateIndex].ColliderInfo[1].Size;
            }

            if(thisCharacter2D.IsGrounded) {
                if(thisCharacter2D.IsChangingDirection) {
                    thisAnimator.PlayNoRepeat("Slide" + currentStateIndex);
                } else if(thisCharacter2D.Velocity.x != 0f && inputAxis.y == 0f) {
                    thisAnimator.PlayNoRepeat("Walk" + currentStateIndex);
                    thisAnimator.SetFloat("WalkSpeedMultiplier",
                                          2.5f * Mathf.Max(0.1f, Mathf.InverseLerp(0f, thisCharacter2D.MaxVelocity.x, Mathf.Abs(thisCharacter2D.Velocity.x))));
                }
            }
        }
    }

    private void UpdateCollision() {
        if(overrideCollider.Offset != Vector2.zero &&
           overrideCollider.Size != Vector2.zero) {
            thisCharacterCollider2D.offset = overrideCollider.Offset;
            thisCharacterCollider2D.size = overrideCollider.Size;

            overrideCollider.Offset = Vector2.zero;
            overrideCollider.Size = Vector2.zero;
        }

        thisCharacterCollider2D.offset = marioStates[currentStateIndex].ColliderInfo[0].Offset;
        thisCharacterCollider2D.size = marioStates[currentStateIndex].ColliderInfo[0].Size;
    }

    private void Awake() {
        thisTransform = GetComponent<Transform>();
        thisCharacter2D = GetComponent<Character2D>();
        thisAudioController = SingletonController.Get<AudioController>();
        thisCharacterCollider2D = thisCharacter2D.CharacterCollider;
        if(thisAnimator == null) thisAnimator = GetComponentInChildren<Animator>(true);
        if(thisAnimationEventsHook == null) thisAnimationEventsHook = GetComponentInChildren<AnimationEventsHook>(true);
        thisInteractor = GetComponent<Interactor>();
    }

    private void Start() {
        HookAudio();
        HookInteraction();
    }

    private void Update() {
        inputAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        isSprinting = Input.GetKey(KeyCode.LeftShift);

        // Enable jumping when we are grounded
        if(thisCharacter2D.IsGrounded) {
            if(Input.GetKeyDown(KeyCode.Space)) {
                lastPositionBeforeJumping = thisTransform.localPosition;
                isJumping = true;
            }
        }

        // Jump constraints
        if(Mathf.Abs(thisTransform.localPosition.y - lastPositionBeforeJumping.y) > maxJumpHeight ||
           Input.GetKeyUp(KeyCode.Space) ||
           thisCharacter2D.BoxColliderDetector.IsColliding(Direction.Up)) {
            isJumping = false;
        }

        UpdateFlipping();
        UpdateAnimation();
        UpdateCollision();

        // Debug code
        if(Input.GetKeyDown(KeyCode.E)) {
            if(currentStateIndex == 0) currentStateIndex = marioStates.Length - 1;
            else currentStateIndex--;

            SetState(currentStateIndex);
        } else if(Input.GetKeyDown(KeyCode.R)) {
            if(currentStateIndex == marioStates.Length - 1) currentStateIndex = 0;
            else currentStateIndex++;

            SetState(currentStateIndex);
        }
    }

    private void FixedUpdate() {
        if(inputAxis != Vector2.zero) {
            if(inputAxis.y == 0) {
                speed = thisCharacter2D.IsGrounded ? (moveSpeed * (isSprinting ? moveSpeedMultiplier : 1f)) : airMoveSpeed * (isSprinting ? airMoveSpeedMultiplier : 1f);
                thisCharacter2D.Move(new Vector2(inputAxis.x, 0f) * speed * Time.fixedDeltaTime);
            }
        }

        if(isJumping) {
            thisCharacter2D.Move(new Vector2(0f, 1f) * jumpSpeed * Time.fixedDeltaTime);
        }
    }

    private void OnDrawGizmos() {
        Transform tr = GetComponent<Transform>();

        foreach(var state in marioStates) {
            foreach(var colliderSize in state.ColliderInfo) {
                Gizmos.color = colliderSize.GizmoColor;
                Gizmos.DrawWireCube(tr.position + new Vector3(colliderSize.Offset.x, colliderSize.Offset.y, 0f), colliderSize.Size);
            }
        }
    }


    [Serializable]
    public class MarioState {
        // 0 - Idle Size, 
        // 1 - Ducking Size
        public string Label;
        public BoxCollider2DInfo[] ColliderInfo; 
        public AudioClip JumpSound;
        public bool IsCanDuck;
    }

    [Serializable]
    public class BoxCollider2DInfo {
        public Vector2 Offset;
        public Vector2 Size;
        public Color GizmoColor;
    }
}
