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
        get { return _thisCollisionCollider2D; }
        private set { _thisCollisionCollider2D = value; }
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
        private set { thisCharacterObject = value; }
    }

    [Header("Data")]
    public CharacterBrain brain;
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

    [Header("Internal")]
    public CharacterActorStateMachine stateMachine;
    public StateController<CharacterActor> stateController = new StateController<CharacterActor>();

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


    private void Awake() {
        brain.DoAwake(this);
    }

    private void Start() {
        brain.DoStart(this);

        stateController.SetOwner(this);
    }

    private void Update() {
        brain.UpdateInput(this);
        brain.DoUpdate(this);

        stateController.Update();
    }

    private void FixedUpdate() {
        brain.DoFixedUpdate(this);

        stateController.FixedUpdate();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        brain.DoCollisionEnter2D(this, collision);
    }

    private void OnCollisionStay2D(Collision2D collision) {
        brain.DoCollisionStay2D(this, collision);
    }

    private void OnCollisionExit2D(Collision2D collision) {
        brain.DoCollisionExit2D(this, collision);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        brain.DoTriggerEnter2D(this, collision);
    }

    private void OnTriggerStay2D(Collider2D collision) {
        brain.DoTriggerStay2D(this, collision);
    }

    private void OnTriggerExit2D(Collider2D collision) {
        brain.DoTriggerExit2D(this, collision);
    }

    private void OnDrawGizmos() {
        if(brain) {
            brain.DoDrawGizmos(this);
        }
    }

    private void OnValidate() {
        if(brain) {
            if(brain != oldBrain) brain.DoReset(this);
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

        // Setup interaction collider
        thisInteractionCollider2D = Utilities.CreateObject("Interaction", thisTransform).AddOrGetComponent<BoxCollider2D>();
        thisInteractionCollider2D.isTrigger = true;

        // Setup Animator
        thisAnimator = this.GetComponentInChildren<Animator>(true);
        if(thisAnimator) {
            thisCharacterObject = thisAnimator.GetComponent<Transform>();
        }
    }


    public abstract class CharacterBrain : ScriptableObject {
        public virtual void UpdateInput(CharacterActor characterActor) { }

        public virtual void DoAwake(CharacterActor characterActor) { }
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
    public class CharacterActorStateMachine : StateMachine<CharacterActor> {

    }


    public abstract class CharacterActorState : ScriptableObject, IState<CharacterActor> {
        public virtual void OnEnter(CharacterActor characterActor) {}
        public virtual void OnExit(CharacterActor characterActor) {}
        public virtual void OnFixedUpdate(CharacterActor characterActor) {}
        public virtual void OnLateUpdate(CharacterActor characterActor) {}
        public virtual void OnUpdate(CharacterActor characterActor) {}
    }
}