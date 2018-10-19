using UnityEngine;
using System;
using WishfulDroplet;
using WishfulDroplet.Extensions;


public class CharacterActor : MonoBehaviour {
    public GameObject thisGameObject {
        get { return _thisGameObject; }
        private set { _thisGameObject = value; }
    }

    public Transform thisTransform {
        get { return _thisTransform; }
        private set { _thisTransform = value; }
    }

    public Rigidbody2D thisRigidbody2D {
        get { return _thisRigidbody2D; }
        private set { _thisRigidbody2D = value; }
    }

    public BoxCollider2D thisCollisionCollider2D {
        get { return _thisCollisionCollider2D; }
        private set { _thisCollisionCollider2D = value; }
    }

    public BoxCollider2D thisInteractionCollider2D {
        get { return _thisInteractionCollider2D; }
        private set { _thisInteractionCollider2D = value; }
    }

    public Character2D thisCharacter2D {
        get { return _thisCharacter2D; }
        private set { _thisCharacter2D = value; }
    }

    public Animator thisAnimator {
        get { return _thisAnimator; }
        private set { _thisAnimator = value; }
    }

    public Transform thisCharacterObject {
        get { return _thisCharacterObject; }
        private set { _thisCharacterObject = value; }
    }

    [Header("Data")]
    public CharacterBrain brain;
    [SerializeField] private CharacterBrain[] enemyBrains;
    [SerializeField] private CharacterBrain[] powerUpBrains;
    [SerializeField] private CharacterBrain[] buffableBrains;
    public Vector2 inputAxis = Vector2.zero;
    public Vector2 lastJumpPos = Vector2.zero;
    public float landMoveSpeed = .7f;
    public float landSprintSpeed = 4.2f;
    public float airMoveSpeed = .5f;
    public float airSprintSpeed = .75f;
    public float jumpSpeed = 10f;
    public float maxJumpHeight = 2.4f;
    public bool isFlipOnX = true;
    public bool isFlipOnY = true;
    public bool isSprinting = false;
    public bool isJumping = false;

    [Header("States")]
    public FormStates.Small smallFormState;
    public FormStates.Big bigFormState;
    public FormStates.Power powerFormState;
    [HideInInspector] public MovementStates.Idle idleMovementState;
    [HideInInspector] public MovementStates.Walk walkMovementState;
    [HideInInspector] public MovementStates.Slide slideMovementState;
    [HideInInspector] public MovementStates.Duck duckMovementState;
    [HideInInspector] public MovementStates.Jump jumpMovementState;
    [HideInInspector] public MovementStates.Fall fallMovementState;
    public MovementStates.Bounce bounceMovementState;
    [HideInInspector] public MovementStates.Transition transitionMovementState;
    [HideInInspector] public StatusStates.Normal normalStatusState;
    [HideInInspector] public StatusStates.Invincible invincibleStatusState;
    [HideInInspector] public StatusStates.Star starStatusState;
    [HideInInspector] public StatusStates.Dead deadStatusState;

    [Header("Internal")]
    public StateController<CharacterActor> stateController = new StateController<CharacterActor>();
    public StateMachine<CharacterActor> formStateMachine = new StateMachine<CharacterActor>("FORM");
    public StateMachine<CharacterActor> movementStateMachine = new StateMachine<CharacterActor>("MOVEMENT");
    public StateMachine<CharacterActor> statusStateMachine = new StateMachine<CharacterActor>("STATUS");

    [Header("Editor Internal")]
    [SerializeField] private CharacterBrain oldBrain;

    [Header("References")]
    [SerializeField] private GameObject _thisGameObject;
    [SerializeField] private Transform _thisTransform;
    [SerializeField] private Rigidbody2D _thisRigidbody2D;
    [SerializeField] private BoxCollider2D _thisCollisionCollider2D;
    [SerializeField] private BoxCollider2D _thisInteractionCollider2D;
    [SerializeField] private Character2D _thisCharacter2D;
    [SerializeField] private Animator _thisAnimator;
    [SerializeField] private Transform _thisCharacterObject;


    public T IsThisCharactersEnemy<T>(T brain) where T : CharacterBrain {
        return CheckBrainIfOnSet<T>(enemyBrains, brain);
    }

    public T IsThisCharactersPowerUp<T>(T brain) where T : CharacterBrain {
        return CheckBrainIfOnSet<T>(powerUpBrains, brain);
    }

    public T IsThisCharactersBuffable<T>(T brain) where T : CharacterBrain {
        return CheckBrainIfOnSet<T>(buffableBrains, brain);
    }

    private T CheckBrainIfOnSet<T>(CharacterBrain[] brainSet, T brain) where T : CharacterBrain {
        if(brain == null) return null;

        for(int i = 0; i < brainSet.Length; i++) {
            if(brainSet[i] == brain) {
                return (T)brainSet[i];
            }
        }

        return null;
    }

    public void SetForm(FormStates.FormState form, bool isDoTranstion = true) {
        formStateMachine.SetState(form);
        if(isDoTranstion) movementStateMachine.PushState(transitionMovementState);
    }

    private void UpdateCharacterObjectFlipping() {
        Vector2 characterFlip = thisCharacterObject.localScale;

        if(isFlipOnX && thisCharacter2D.FaceAxis.x != 0f) {
            if(thisCharacter2D.FaceAxis.x != Mathf.Sign(thisCharacterObject.localScale.x)) {
                characterFlip.x *= -1f;
            }
        }

        if(isFlipOnY && thisCharacter2D.FaceAxis.y != 0f) {
            if(thisCharacter2D.FaceAxis.y != Mathf.Sign(thisCharacterObject.localScale.y)) {
                characterFlip.y *= -1f;
            }
        }

        thisCharacterObject.localScale = characterFlip;
    }

    private void Awake() {
        if(brain) {
            brain.DoAwake(this);
        }
    }

    private void OnEnable() {
        if(brain) {
            brain.DoOnEnable(this);
        }
    }

    private void OnDisable() {
        if(brain) {
            brain.DoOnDisable(this);
        }
    }

    private void Start() {
        stateController.AddStateMachine(formStateMachine, this);
        stateController.AddStateMachine(movementStateMachine, this);
        stateController.AddStateMachine(statusStateMachine, this);

        if(brain) {
            brain.DoStart(this);
        }
    }

    private void Update() {
        if(brain) {
            brain.UpdateInput(this);
        }

        UpdateCharacterObjectFlipping();

        if(brain) {
            brain.DoUpdate(this);
        } 

        stateController.Update();
    }

    private void FixedUpdate() {
        if(brain) {
            brain.DoFixedUpdate(this);
        }

        stateController.FixedUpdate();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(brain) {
            brain.DoCollisionEnter2D(this, collision);
        }
    }

    private void OnCollisionStay2D(Collision2D collision) {
        if(brain) {
            brain.DoCollisionStay2D(this, collision);
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if(brain) {
            brain.DoCollisionExit2D(this, collision);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(brain) {
            brain.DoTriggerEnter2D(this, collision);
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if(brain) {
            brain.DoTriggerStay2D(this, collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if(brain) {
            brain.DoTriggerExit2D(this, collision);
        }
    }

    private void OnDrawGizmos() {
        if(brain) {
            brain.DoDrawGizmos(this);
        }
    }

    private void OnValidate() {
        if(brain) {
            if(brain != oldBrain) {
                brain.DoReset(this);
                oldBrain = brain;
            }
        }
    }

    private void Reset() {
        thisGameObject = gameObject;
        thisTransform = GetComponent<Transform>();

        // Setup Rigidbody2D
        thisRigidbody2D = _thisGameObject.AddOrGetComponent<Rigidbody2D>();
        thisRigidbody2D.sharedMaterial = new PhysicsMaterial2D("Slippery") {
            friction = .05f
        };
        thisRigidbody2D.mass = 0.001f;
        thisRigidbody2D.drag = 2f;
        thisRigidbody2D.gravityScale = 5f;
        thisRigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        thisRigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Setup Character2D
        thisCharacter2D = thisGameObject.AddOrGetComponent<Character2D>();

        // Setup collision collider
        thisCollisionCollider2D = thisGameObject.AddOrGetComponent<BoxCollider2D>();
        thisCollisionCollider2D.size = thisCharacterObject ? thisCharacterObject.GetComponentInChildren<SpriteRenderer>().size :
                                                             thisCollisionCollider2D.size;

        // Setup interaction collider
        thisInteractionCollider2D = Utilities.CreateObject("Interaction", thisTransform).AddOrGetComponent<BoxCollider2D>();
        thisInteractionCollider2D.isTrigger = true;
        thisInteractionCollider2D.size = thisCollisionCollider2D.size;

        // Setup Animator
        thisAnimator = this.GetComponentInChildren<Animator>(true);
        if(thisAnimator) {
            thisCharacterObject = thisAnimator.GetComponent<Transform>();
        }
    }


    public abstract class CharacterBrain : ScriptableObject {
        public virtual void UpdateInput(CharacterActor characterActor) { }

        public virtual void DoAwake(CharacterActor characterActor) { }
        public virtual void DoOnEnable(CharacterActor characterActor) { }
        public virtual void DoOnDisable(CharacterActor characterActor) { }
        public virtual void DoStart(CharacterActor characterActor) { }
        public virtual void DoUpdate(CharacterActor characterActor) { }
        public virtual void DoFixedUpdate(CharacterActor characterActor) { }
        public virtual void DoCollisionEnter2D(CharacterActor characterActor, Collision2D collision) { }
        public virtual void DoCollisionStay2D(CharacterActor characterActor, Collision2D collision) { }
        public virtual void DoCollisionExit2D(CharacterActor characterActor, Collision2D collision) { }
        public virtual void DoTriggerEnter2D(CharacterActor characterActor, Collider2D collision) { }
        public virtual void DoTriggerStay2D(CharacterActor characterActor, Collider2D collision) { }
        public virtual void DoTriggerExit2D(CharacterActor characterActor, Collider2D collision) { }
        public virtual void DoDrawGizmos(CharacterActor characterActor) { }
        public virtual void DoReset(CharacterActor characterActor) { }
    }


    [Serializable]
    public abstract class CharacterState : IState<CharacterActor> {
        public virtual void DoEnter(CharacterActor characterActor) {}
        public virtual void DoExit(CharacterActor characterActor) {}
        public virtual void DoFixedUpdate(CharacterActor characterActor) {}
        public virtual void DoLateUpdate(CharacterActor characterActor) {}
        public virtual void DoUpdate(CharacterActor characterActor) {}
    }


    public class FormStates {
        [Serializable]
        public class Small : FormState {

        }


        [Serializable]
        public class Big : FormState {

        }


        [Serializable]
        public class Power : FormState {

        }


        [Serializable]
        public class BoxColliderInfo {
            public Vector2 offset;
            public Vector2 size;

            [Header("Debug")]
            public Color gizmoColor = new Color(1f, 1f, 1f, 1f);
        }


        [Serializable]
        public abstract class FormState : CharacterState {
            [Header("Form")]
            public RuntimeAnimatorController runtimeAnimatorController;
            public bool isCanDuck;
            public bool isCanBreakBrick;

            [Header("Transition")]
            public BoxColliderInfo transitionBoxColliderInfo;
            public AudioClip transitionSound;

            [Header("Duck")]
            public BoxColliderInfo duckBoxColliderInfo;

            [Header("Jump")]
            public AudioClip jumpSound;


            public override void DoEnter(CharacterActor characterActor) {
                characterActor.thisAnimator.runtimeAnimatorController = runtimeAnimatorController;

                characterActor.transitionMovementState.transitionSound = transitionSound;
                characterActor.transitionMovementState.boxColliderInfo = transitionBoxColliderInfo;

                characterActor.duckMovementState.boxColliderInfo = duckBoxColliderInfo;

                characterActor.jumpMovementState.jumpSound = jumpSound;
            }
        }
    }


    public class MovementStates {
        [Serializable]
        public class Idle : CharacterState {
            public override void DoEnter(CharacterActor characterActor) {
                characterActor.thisAnimator.PlayNoRepeat("Idle");
            }
        }


        [Serializable]
        public class Walk : CharacterState {
            public override void DoEnter(CharacterActor characterActor) {
                characterActor.thisAnimator.PlayNoRepeat("Walk");
            }

            public override void DoUpdate(CharacterActor characterActor) {
                characterActor.thisAnimator.SetFloat("WalkSpeedMultiplier",
                                            2.5f * Mathf.Max(0.1f, Mathf.InverseLerp(0f, characterActor.thisCharacter2D.MaxVelocity.x,
                                                                                         Mathf.Abs(characterActor.thisCharacter2D.Velocity.x))));
            }

            public override void DoFixedUpdate(CharacterActor characterActor) {
                float speed = characterActor.isSprinting ? characterActor.landSprintSpeed : characterActor.landMoveSpeed;
                characterActor.thisCharacter2D.Move(new Vector2(characterActor.inputAxis.x, 0f) * speed * Time.fixedDeltaTime);
            }
        }


        [Serializable]
        public class Slide : CharacterState {
            public override void DoEnter(CharacterActor characterActor) {
                characterActor.thisAnimator.PlayNoRepeat("Slide");
            }
        }


        [Serializable]
        public class Duck : CharacterState {
            public FormStates.BoxColliderInfo boxColliderInfo;


            public override void DoEnter(CharacterActor characterActor) {
                characterActor.thisAnimator.PlayNoRepeat("Duck");

                characterActor.thisCollisionCollider2D.offset = boxColliderInfo.offset;
                characterActor.thisCollisionCollider2D.size = boxColliderInfo.size;

                characterActor.thisInteractionCollider2D.offset = boxColliderInfo.offset;
                characterActor.thisInteractionCollider2D.size = boxColliderInfo.size;
            }
        }


        [Serializable]
        public class Jump : CharacterState {
            public AudioClip jumpSound;


            public override void DoEnter(CharacterActor characterActor) {
                characterActor.lastJumpPos = characterActor.thisTransform.localPosition;
                characterActor.thisAnimator.PlayNoRepeat("Jump");
                Singleton.Get<IAudioController>().PlayOneShot(jumpSound);
            }

            public override void DoFixedUpdate(CharacterActor characterActor) {
                float speed = characterActor.isSprinting ? characterActor.airSprintSpeed : characterActor.airMoveSpeed;
                characterActor.thisCharacter2D.Move(new Vector2(characterActor.inputAxis.x * speed, 1f * characterActor.jumpSpeed) * Time.fixedDeltaTime);

            }
        }


        [Serializable]
        public class Fall : CharacterState {
            public override void DoEnter(CharacterActor characterActor) {
                characterActor.thisAnimator.PlayNoRepeat("Fall");
            }

            public override void DoFixedUpdate(CharacterActor characterActor) {
                float speed = characterActor.isSprinting ? characterActor.airSprintSpeed : characterActor.airMoveSpeed;
                characterActor.thisCharacter2D.Move(new Vector2(characterActor.inputAxis.x * speed, 0f) * Time.fixedDeltaTime);
            }
        }


        [Serializable]
        public class Bounce : CharacterState {
            public float bounceTime = .2f;
            public AudioClip stepSound;

            private float timer;


            public override void DoEnter(CharacterActor characterActor) {
                timer = 0f;

                characterActor.thisAnimator.PlayNoRepeat("Jump");
                characterActor.thisCharacter2D.SetVelocity(new Vector2(characterActor.thisCharacter2D.Velocity.x, 0f));
                Singleton.Get<IAudioController>().PlayOneShot(stepSound);
            }

            public override void DoFixedUpdate(CharacterActor characterActor) {
                if(timer > bounceTime) {
                    characterActor.movementStateMachine.PopState();
                } else {
                    timer += Time.fixedDeltaTime;

                    float speed = characterActor.isSprinting ? characterActor.airSprintSpeed : characterActor.airMoveSpeed;
                    characterActor.thisCharacter2D.Move(new Vector2(characterActor.inputAxis.x * speed, 1f * characterActor.jumpSpeed) * Time.fixedDeltaTime);
                }
            }
        }


        [Serializable]
        public class Transition : CharacterState {
            public AudioClip transitionSound;
            public FormStates.BoxColliderInfo boxColliderInfo;


            public override void DoEnter(CharacterActor characterActor) {
                Time.timeScale = 0f;
                characterActor.thisAnimator.PlayNoRepeat("Transition");
                Singleton.Get<IAudioController>().PlayOneShot(transitionSound);
            }

            public override void DoUpdate(CharacterActor characterActor) {
                if(characterActor.thisAnimator.GetCurrentAnimatorStateInfo(0).IsName("Transition")) {
                    if(characterActor.thisAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > .95f) {
                        characterActor.movementStateMachine.PopState();
                    }
                }
            }

            public override void DoExit(CharacterActor characterActor) {
                Time.timeScale = 1f;
                characterActor.thisCollisionCollider2D.offset = boxColliderInfo.offset;
                characterActor.thisCollisionCollider2D.size = boxColliderInfo.size;
                characterActor.thisInteractionCollider2D.offset = boxColliderInfo.offset;
                characterActor.thisInteractionCollider2D.size = boxColliderInfo.size;
            }
        }
    }

   
    public class StatusStates {
        [Serializable]
        public class Normal : CharacterState {

        }


        [Serializable]
        public class Invincible : CharacterState {

        }


        [Serializable]
        public class Star : CharacterState {

        }


        [Serializable]
        public class Dead : CharacterState {
            public AudioClip deathSound;


            public override void DoEnter(CharacterActor characterActor) {
                Time.timeScale = 0f;
                characterActor.thisAnimator.PlayNoRepeat("Dead");
                Singleton.Get<IAudioController>().PlayOneShot(deathSound);
            }

            public override void DoExit(CharacterActor characterActor) {
                Time.timeScale = 1f;
            }
        }
    }
}