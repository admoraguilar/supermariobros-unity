using UnityEngine;
using System;
using WishfulDroplet;
using WishfulDroplet.Extensions;


public class MarioController : MonoBehaviour {
    //public StateMachine<MarioController> FormStateMachine { get { return formStateMachine; } }
    //public States.FormStateMachineData CurFormStateMachineData { get { return curFormStateMachineData; } }
    //public StateMachine<MarioController> StatusStateMachine { get { return statusStateMachine; } }
    //public States.StatusStateMachineData StatusStateMachineData { get { return statusStateMachineData; } }

    //[Header("Form State")]
    //[SerializeField] private StateMachine<MarioController> formStateMachine = new StateMachine<MarioController>();
    //[SerializeField] private States.FormStateMachineData[] formStateMachineData = new States.FormStateMachineData[0];
    //[SerializeField] private States.FormStateMachineData curFormStateMachineData;

    //[Header("Status State")]
    //[SerializeField] private StateMachine<MarioController> statusStateMachine = new StateMachine<MarioController>();
    //[SerializeField] private States.StatusStateMachineData statusStateMachineData = new States.StatusStateMachineData();

    //[Header("Shared State")]
    //[SerializeField] private StateController<MarioController> stateController = new StateController<MarioController>();
    //[SerializeField] private float landMoveSpeed = .7f;
    //[SerializeField] private float landSprintSpeed = 4.2f;
    //[SerializeField] private float airMoveSpeed = .5f;
    //[SerializeField] private float airSprintSpeed = .75f;
    //[SerializeField] private float jumpSpeed = 10f;
    //[SerializeField] private float maxJumpHeight = 2.4f;
    //[SerializeField] private bool isFlipOnX = true;
    //[SerializeField] private bool isFlipOnY = false;

    //[Header("Debug")]
    //[SerializeField] private Vector2 inputAxis;
    //[SerializeField] private Vector2 lastJumpPos;
    //[SerializeField] private bool isPressingSprintKey;
    //[SerializeField] private bool isPressingJumpKey;

    //[Header("References")]
    //[SerializeField] private Transform thisTransform;
    //[SerializeField] private Rigidbody2D thisRigidbody2D;
    //[SerializeField] private BoxCollider2D thisSolidCollider2D;
    //[SerializeField] private BoxCollider2D thisInteractCollider2D;
    //[SerializeField] private Character2D thisCharacter2D;
    //[SerializeField] private Animator thisAnimator;
    //[SerializeField] private Transform thisCharacterObject;

    //private IAudioController thisAudioController;


    //public States.FormStateMachineData GetFormStateMachineData(Ids.FormId formId) {
    //    States.FormStateMachineData stateData = null;

    //    for(int i = 0; i < formStateMachineData.Length; i++) {
    //        if(formStateMachineData[i].Id == formId) {
    //            stateData = formStateMachineData[i];
    //        }
    //    }

    //    return stateData;
    //}

    //public void SetForm(Ids.FormId formId) {
    //    curFormStateMachineData = GetFormStateMachineData(formId);
    //    formStateMachine.PushState(curFormStateMachineData.Transition);
    //}

    //public void Dead(bool value) {
    //    if(value) {
    //        statusStateMachine.PushState(statusStateMachineData.Dead);
    //    } else {
    //        if(statusStateMachine.currentState == statusStateMachineData.Dead) {
    //            statusStateMachine.PopState();
    //        }
    //    }
    //}

    //private void UpdateInput() {
    //    inputAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    //    isPressingSprintKey = Input.GetKey(KeyCode.LeftShift);
    //    isPressingJumpKey = Input.GetKey(KeyCode.Space);

    //    // Debug input
    //    if(Input.GetKeyDown(KeyCode.Alpha1)) {
    //        SetForm(Ids.FormId.Small);
    //    }

    //    if(Input.GetKeyDown(KeyCode.Alpha2)) {
    //        SetForm(Ids.FormId.Big);
    //    }

    //    if(Input.GetKeyDown(KeyCode.Alpha3)) {
    //        SetForm(Ids.FormId.Power);
    //    }

    //    if(Input.GetKeyDown(KeyCode.T)) {
    //        Dead(true);
    //    }

    //    if(Input.GetKeyDown(KeyCode.Y)) {
    //        Dead(false);
    //    }
    //}

    //private void UpdateCharacterObjectFlipping() {
    //    Vector2 characterFlip = thisCharacterObject.localScale;

    //    if(isFlipOnX && thisCharacter2D.FaceAxis.x != 0f) {
    //        if(thisCharacter2D.FaceAxis.x != Mathf.Sign(thisCharacterObject.localScale.x)) {
    //            characterFlip.x *= -1f;
    //        }
    //    }

    //    if(isFlipOnY && thisCharacter2D.FaceAxis.y != 0f) {
    //        if(thisCharacter2D.FaceAxis.y != Mathf.Sign(thisCharacterObject.localScale.y)) {
    //            characterFlip.y *= -1f;
    //        }
    //    }

    //    thisCharacterObject.localScale = characterFlip;
    //}

    //private void UpdateStatesHandling() {
    //    if(statusStateMachine.currentState != statusStateMachineData.Dead &&
    //       formStateMachine.currentState != curFormStateMachineData.Transition) {
    //        if(thisCharacter2D.IsGrounded) {
    //            if(!thisCharacter2D.IsMoving(Direction.Any) &&
    //                inputAxis == Vector2.zero &&
    //                !isPressingJumpKey) {
    //                formStateMachine.SetState(curFormStateMachineData.Idle);
    //            }

    //            if(inputAxis.x != 0 &&
    //               inputAxis.y == 0 &&
    //               !isPressingJumpKey &&
    //               !thisCharacter2D.IsChangingDirection) {
    //                formStateMachine.SetState(curFormStateMachineData.Walk);
    //            }

    //            if(inputAxis.x == 0 &&
    //               inputAxis.y < 0f &&
    //               curFormStateMachineData.IsCanDuck &&
    //               !isPressingJumpKey) {
    //                formStateMachine.PushState(curFormStateMachineData.Duck);
    //            } else {
    //                if(formStateMachine.currentState == curFormStateMachineData.Duck) {
    //                    formStateMachine.PopState();
    //                }
    //            }

    //            if(thisCharacter2D.IsChangingDirection &&
    //               thisCharacter2D.IsMoving(Direction.Any) &&
    //               !isPressingJumpKey) {
    //                formStateMachine.SetState(curFormStateMachineData.Slide);
    //            }

    //            if(isPressingJumpKey) {
    //                formStateMachine.SetState(curFormStateMachineData.Jump);
    //            }
    //        } else {
    //            if((Mathf.Abs(thisTransform.localPosition.y - lastJumpPos.y) > maxJumpHeight ||
    //                thisCharacter2D.IsColliding(Direction.Up) ||
    //                !isPressingJumpKey) &&
    //                formStateMachine.currentState != curFormStateMachineData.Bounce) {
    //                formStateMachine.SetState(curFormStateMachineData.Fall);
    //            }
    //        }
    //    }
    //}

    //private void HandleTriggerInteractResponse(Collider2D collision) {
    //    GoombaController goomba = collision.GetComponent<GoombaController>();
    //    if(goomba) {
    //        //Debug.Log(string.Format("Mario: {0} | Goomba: {1} | Distance: {2}",
    //        //                        thisInteractCollider2D.bounds.min.y,
    //        //                        collision.bounds.min.y,
    //        //                        Mathf.Abs(thisInteractCollider2D.bounds.min.y - collision.bounds.min.y)));
    //        if(Mathf.Abs(thisInteractCollider2D.bounds.min.y - collision.bounds.min.y) > .05f &&
    //            thisInteractCollider2D.bounds.min.y >= collision.bounds.min.y) {
    //            formStateMachine.PushState(curFormStateMachineData.Bounce);
    //            Destroy(goomba.gameObject);
    //        }
    //    }

    //    PowerUpController powerUp = collision.GetComponent<PowerUpController>();
    //    if(powerUp) {
    //        Destroy(powerUp.gameObject);
    //    }
    //}

    //private void DrawFormStateGizmos() {
    //    for(int i = 0; i < formStateMachineData.Length; i++) {
    //        // Duck collider
    //        Gizmos.color = formStateMachineData[i].Duck.BoxColliderInfo.GizmoColor;
    //        Gizmos.DrawWireCube(new Vector2(thisTransform.position.x, thisTransform.position.y) + formStateMachineData[i].Duck.BoxColliderInfo.Offset,
    //                                        formStateMachineData[i].Duck.BoxColliderInfo.Size);

    //        // Transition collider 
    //        Gizmos.color = formStateMachineData[i].Transition.BoxColliderInfo.GizmoColor;
    //        Gizmos.DrawWireCube(new Vector2(thisTransform.position.x, thisTransform.position.y) + formStateMachineData[i].Transition.BoxColliderInfo.Offset,
    //                                        formStateMachineData[i].Transition.BoxColliderInfo.Size);
    //    }
    //}

    //private void Awake() {
    //    thisAudioController = Singleton.Get<IAudioController>();
    //}

    //private void Start() {
    //    stateController.SetOwner(this);

    //    formStateMachine = stateController.AddStateMachine(Ids.StateMachineId.FORM, null);
    //    statusStateMachine = stateController.AddStateMachine(Ids.StateMachineId.STATUS, null);

    //    curFormStateMachineData = GetFormStateMachineData(Ids.FormId.Small);
    //}

    //private void Update() {
    //    UpdateInput();
    //    UpdateCharacterObjectFlipping();
    //    UpdateStatesHandling();

    //    stateController.Update();
    //}

    //private void FixedUpdate() {
    //    stateController.FixedUpdate();
    //}

    //private void OnTriggerEnter2D(Collider2D collision) {
    //    HandleTriggerInteractResponse(collision);
    //}

    //private void OnDrawGizmos() {
    //    DrawFormStateGizmos();
    //}

    //private void Reset() {
    //    thisTransform = GetComponent<Transform>();

    //    // Setup some state machine data
    //    States.BoxColliderInfo duckColliderInfo = new States.BoxColliderInfo {
    //        Offset = new Vector2(0f, 0.15f),
    //        Size = new Vector2(0.8f, 1.11f),
    //        GizmoColor = Color.cyan
    //    };

    //    States.BoxColliderInfo smallFormColliderInfo = new States.BoxColliderInfo {
    //        Offset = new Vector2(0f, 0f),
    //        Size = new Vector2(0.65f, 0.8f),
    //        GizmoColor = Color.blue
    //    };

    //    States.BoxColliderInfo bigFormColliderInfo = new States.BoxColliderInfo {
    //        Offset = new Vector2(0f, 0.4f),
    //        Size = new Vector2(0.8f, 1.6f),
    //        GizmoColor = Color.blue
    //    };

    //    formStateMachineData = new States.FormStateMachineData[3] {
    //        new States.FormStateMachineData {
    //            Id = Ids.FormId.Small,
    //            IsCanBreakBrick = false,
    //            IsCanDuck = false,
    //            Duck = new States.DuckFormState { BoxColliderInfo = duckColliderInfo },
    //            Transition = new States.TransitionFormState { BoxColliderInfo = smallFormColliderInfo }
    //        },
    //        new States.FormStateMachineData {
    //            Id = Ids.FormId.Big,
    //            IsCanBreakBrick = true,
    //            IsCanDuck = true,
    //            Duck = new States.DuckFormState { BoxColliderInfo = duckColliderInfo },
    //            Transition = new States.TransitionFormState { BoxColliderInfo = bigFormColliderInfo }
    //        },
    //        new States.FormStateMachineData {
    //            Id = Ids.FormId.Power,
    //            IsCanBreakBrick = true,
    //            IsCanDuck = true,
    //            Duck = new States.DuckFormState { BoxColliderInfo = duckColliderInfo },
    //            Transition = new States.TransitionFormState { BoxColliderInfo = bigFormColliderInfo }
    //        }
    //    };

    //    // Setup rigidbody2D
    //    thisRigidbody2D = gameObject.AddOrGetComponent<Rigidbody2D>();
    //    thisRigidbody2D.sharedMaterial = new PhysicsMaterial2D("Slippery") {
    //        friction = .05f
    //    };
    //    thisRigidbody2D.mass = 0.001f;
    //    thisRigidbody2D.drag = 2f;
    //    thisRigidbody2D.gravityScale = 5f;
    //    thisRigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    //    thisRigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;

    //    // Setup character2D
    //    thisCharacter2D = gameObject.AddOrGetComponent<Character2D>();

    //    // Setup colliders
    //    thisSolidCollider2D = gameObject.AddOrGetComponent<BoxCollider2D>();

    //    thisInteractCollider2D = Utilities.CreateObject("Interactor", thisTransform).AddOrGetComponent<BoxCollider2D>();
    //    thisInteractCollider2D.isTrigger = true;

    //    // Setup animator
    //    thisAnimator = this.GetComponentInChildren<Animator>(true);
    //    if(thisAnimator) {
    //        thisCharacterObject = thisAnimator.GetComponent<Transform>();
    //    }
    //}


    //public class States {
    //    #region FORM STATE

    //    [Serializable]
    //    public class FormStateMachineData {
    //        public Ids.FormId Id;

    //        public bool IsCanDuck;
    //        public bool IsCanBreakBrick;

    //        public IdleFormState Idle;
    //        public WalkFormState Walk;
    //        public SlideFormState Slide;
    //        public DuckFormState Duck;
    //        public JumpFormState Jump;
    //        public FallFormState Fall;
    //        public BounceFormState Bounce;
    //        public TransitionFormState Transition;
    //    }

    //    [Serializable]
    //    public class BoxColliderInfo {
    //        public Vector2 Offset;
    //        public Vector2 Size;

    //        [Header("Debug")]
    //        public Color GizmoColor = new Color(1f, 1f, 1f, 1f);
    //    }


    //    [Serializable]
    //    public class IdleFormState : IState<MarioController> {
    //        public void OnEnter(MarioController owner) {
    //            owner.thisAnimator.PlayNoRepeat("Idle" + (int)owner.curFormStateMachineData.Id);
    //        }

    //        public void OnExit(MarioController owner) {
                
    //        }
    //    }


    //    [Serializable]
    //    public class WalkFormState : IState<MarioController> {
    //        public override void OnEnter(MarioController owner) {
    //            owner.thisAnimator.PlayNoRepeat("Walk" + (int)owner.curFormStateMachineData.Id);
    //        }

    //        public override void OnUpdate(MarioController owner) {
    //            owner.thisAnimator.SetFloat("WalkSpeedMultiplier",
    //                                        2.5f * Mathf.Max(0.1f, Mathf.InverseLerp(0f, owner.thisCharacter2D.MaxVelocity.x,
    //                                                                                     Mathf.Abs(owner.thisCharacter2D.Velocity.x))));
    //        }

    //        public override void OnFixedUpdate(MarioController owner) {
    //            float speed = owner.isPressingSprintKey ? owner.landSprintSpeed : owner.landMoveSpeed;
    //            owner.thisCharacter2D.Move(new Vector2(owner.inputAxis.x, 0f) * speed * Time.fixedDeltaTime);
    //        }

    //        public override void OnExit(MarioController owner) {
                
    //        }
    //    }


    //    [Serializable]
    //    public class SlideFormState : IState<MarioController> {
    //        public override void OnEnter(MarioController owner) {
    //            owner.thisAnimator.PlayNoRepeat("Slide" + (int)owner.curFormStateMachineData.Id);
    //        }

    //        public override void OnExit(MarioController owner) {
                
    //        }
    //    }


    //    [Serializable]
    //    public class DuckFormState : IState<MarioController> {
    //        public BoxColliderInfo BoxColliderInfo;


    //        public override void OnEnter(MarioController owner) {
    //            owner.thisAnimator.PlayNoRepeat("Duck" + (int)owner.curFormStateMachineData.Id);

    //            owner.thisSolidCollider2D.offset = BoxColliderInfo.Offset;
    //            owner.thisSolidCollider2D.size = BoxColliderInfo.Size;

    //            owner.thisInteractCollider2D.offset = BoxColliderInfo.Offset;
    //            owner.thisInteractCollider2D.size = BoxColliderInfo.Size;
    //        }

    //        public override void OnExit(MarioController owner) {
                
    //        }
    //    }


    //    [Serializable]
    //    public class JumpFormState : IState<MarioController> {
    //        public AudioClip JumpSound;


    //        public override void OnEnter(MarioController owner) {
    //            owner.lastJumpPos = owner.thisTransform.localPosition;
    //            owner.thisAnimator.PlayNoRepeat("Jump" + (int)owner.curFormStateMachineData.Id);
    //            owner.thisAudioController.PlayOneShot(JumpSound);
    //        }

    //        public override void OnFixedUpdate(MarioController owner) {
    //            float speed = owner.isPressingSprintKey ? owner.airSprintSpeed : owner.airMoveSpeed;
    //            owner.thisCharacter2D.Move(new Vector2(owner.inputAxis.x * speed, 1f * owner.jumpSpeed) * Time.fixedDeltaTime);
    //        }

    //        public override void OnExit(MarioController owner) {
                
    //        }
    //    }


    //    [Serializable]
    //    public class FallFormState : IState<MarioController> {
    //        public override void OnEnter(MarioController owner) {
    //            owner.thisAnimator.PlayNoRepeat("Fall" + (int)owner.curFormStateMachineData.Id);
    //        }

    //        public override void OnFixedUpdate(MarioController owner) {
    //            float speed = owner.isPressingSprintKey ? owner.airSprintSpeed : owner.airMoveSpeed;
    //            owner.thisCharacter2D.Move(new Vector2(owner.inputAxis.x * speed, 0f) * Time.fixedDeltaTime);
    //        }

    //        public override void OnExit(MarioController owner) {
                
    //        }
    //    }


    //    [Serializable]
    //    public class BounceFormState : IState<MarioController> {
    //        public AudioClip StepSound;
    //        public float BounceTime;

    //        private float bounceTime;


    //        public override void OnEnter(MarioController owner) {
    //            bounceTime = 0f;

    //            owner.thisAnimator.PlayNoRepeat("Jump" + (int)owner.curFormStateMachineData.Id);
    //            owner.thisCharacter2D.SetVelocity(new Vector2(owner.thisCharacter2D.Velocity.x, 0f));
    //            owner.thisAudioController.PlayOneShot(StepSound);
    //        }

    //        public override void OnFixedUpdate(MarioController owner) {
    //            if(bounceTime > BounceTime) {
    //                owner.formStateMachine.PopState();
    //            } else {
    //                bounceTime += Time.fixedDeltaTime;

    //                float speed = owner.isPressingSprintKey ? owner.airSprintSpeed : owner.airMoveSpeed;
    //                owner.thisCharacter2D.Move(new Vector2(owner.inputAxis.x * speed, 1f * owner.jumpSpeed) * Time.fixedDeltaTime);
    //            }
    //        }

    //        public override void OnExit(MarioController owner) {

    //        }
    //    }


    //    [Serializable]
    //    public class TransitionFormState : IState<MarioController> {
    //        public AudioClip TransitionSound;
    //        public BoxColliderInfo BoxColliderInfo;


    //        public override void OnEnter(MarioController owner) {
    //            Time.timeScale = 0f;
    //            owner.thisAnimator.PlayNoRepeat("Transition" + (int)owner.curFormStateMachineData.Id);
    //            owner.thisAudioController.PlayOneShot(TransitionSound);
    //        }

    //        public override void OnUpdate(MarioController owner) {
    //            if(owner.thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("Transition" + (int)owner.curFormStateMachineData.Id)) {
    //                if(owner.thisAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > .95f) {
    //                    Time.timeScale = 1f;
    //                    owner.formStateMachine.PopState();
    //                }
    //            }
    //        }

    //        public override void OnExit(MarioController owner) {
    //            owner.thisSolidCollider2D.offset = BoxColliderInfo.Offset;
    //            owner.thisSolidCollider2D.size = BoxColliderInfo.Size;

    //            owner.thisInteractCollider2D.offset = BoxColliderInfo.Offset;
    //            owner.thisInteractCollider2D.size = BoxColliderInfo.Size;
    //        }
    //    }

    //    #endregion


    //    #region STATUS STATE

    //    [Serializable]
    //    public class StatusStateMachineData {
    //        public NormalStatusState Normal;
    //        public DeadStatusState Dead;
    //        public InvincibleStatusState Invincible;
    //        public StarStatusState Star;
    //    }


    //    [Serializable]
    //    public class NormalStatusState : IState<MarioController> {
    //        public override void OnEnter(MarioController owner) {
                
    //        }

    //        public override void OnExit(MarioController owner) {
                
    //        }
    //    }


    //    [Serializable]
    //    public class DeadStatusState : IState<MarioController> {
    //        public AudioClip DeathSound;


    //        public override void OnEnter(MarioController owner) {
    //            Time.timeScale = 0f;
    //            owner.thisAnimator.PlayNoRepeat("Dead" + (int)owner.curFormStateMachineData.Id);
    //            owner.thisAudioController.PlayOneShot(DeathSound);
    //        }

    //        public override void OnExit(MarioController owner) {
    //            Time.timeScale = 1f;
    //        }
    //    }


    //    [Serializable]
    //    public class InvincibleStatusState : IState<MarioController> {
    //        public override void OnEnter(MarioController owner) {
                
    //        }

    //        public override void OnExit(MarioController owner) {
                
    //        }
    //    }


    //    [Serializable]
    //    public class StarStatusState : IState<MarioController> {
    //        public override void OnEnter(MarioController owner) {
                
    //        }

    //        public override void OnExit(MarioController owner) {
                
    //        }
    //    }

    //    #endregion
    //}

    //public class Ids {
    //    public class StateMachineId {
    //        public const string FORM = "FORM";
    //        public const string SPEED = "SPEED";
    //        public const string STATUS = "STATUS";
    //    }

    //    public enum FormId {
    //        Small, Big, Power
    //    }

    //    public enum FormStateId {
    //        Idle, Walk, Slide,
    //        Duck, Jump, Fall,
    //        Bounce, Transition
    //    }

    //    public enum StatusStateId {
    //        Normal, Dead, Invincible,
    //        Star
    //    }
    //}
}
