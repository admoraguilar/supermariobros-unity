using UnityEngine;
using System;
using WishfulDroplet;
using WishfulDroplet.Extensions;


public class PowerupActor : Actor<PowerupActor, PowerupActor.PowerupBrain> {
	[InspectorNote("Powerup Actor")]
	[Header("Data")]
	public ScriptableActorBrain[]											buffableBrains;
	public float															moveSpeed;

	[Header("States")]
	[HideInInspector] public MovementStates.Interact						interactMovementState = new MovementStates.Interact();
	[HideInInspector] public MovementStates.Move							moveMovementState = new MovementStates.Move();

	[Header("Internal")]
	public Vector2															moveDirection;
	[HideInInspector] public StateMachine<PowerupActor, PowerupState>		movementStateMachine = new StateMachine<PowerupActor, PowerupState>();

	[Header("References")]
	[SerializeField] private Rigidbody2D									_thisRigidbody2D;
	[SerializeField] private Character2D									_thisCharacter2D;
	[SerializeField] private Interactable									_thisInteractable;
	[SerializeField] private Animator										_thisAnimator;
	[SerializeField] private Transform										_thisCharacterObject;
	[SerializeField] private SpriteRenderer									_thisSpriteRenderer;
	[SerializeField] private BoxCollider2D									_thisCollisionCollider2D;
	[SerializeField] private BoxCollider2D									_thisInteractionCollider2D;

	public Rigidbody2D														thisRigidbody2D {
		get { return _thisRigidbody2D; }
		private set { _thisRigidbody2D = value; }
	}

	public Character2D														thisCharacter2D {
		get { return _thisCharacter2D; }
		private set { _thisCharacter2D = value; }
	}

	public Interactable														thisInteractable {
		get { return _thisInteractable; }
		private set { _thisInteractable = value; }
	}

	public Animator															thisAnimator {
		get { return _thisAnimator; }
		private set { _thisAnimator = value; }
	}

	public Transform														thisCharacterObject {
		get { return _thisCharacterObject; }
		private set { _thisCharacterObject = value; }
	}

	public SpriteRenderer													thisSpriteRenderer {
		get { return _thisSpriteRenderer; }
		private set { _thisSpriteRenderer = value; }
	}

	public BoxCollider2D													thisCollisionCollider2D {
		get { return _thisCollisionCollider2D; }
		private set { _thisCollisionCollider2D = value; }
	}

	public BoxCollider2D													thisInteractionCollider2D {
		get { return _thisInteractionCollider2D; }
		private set { _thisInteractionCollider2D = value; }
	}


	public bool IsBrainBuffable(ScriptableActorBrain brain) {
		return IsBrainOnSet(buffableBrains, brain);
	}

	private bool OnInteracted(Direction direction, GameObject interactor) {
		if(brain) {
			return brain.DoInteracted(this, direction, interactor);
		}

		return false;
	}

	private void OnEnable() {
		if(brain) {
			brain.DoOnEnable(this);
		}

		thisInteractable.OnInteract += OnInteracted;
	}

	private void OnDisable() {
		if(brain) {
			brain.DoOnDisable(this);
		}

		thisInteractable.OnInteract -= OnInteracted;
	}

	private void Start() {
		movementStateMachine.SetOwner(this);

		if(brain) {
			brain.DoStart(this);
		}
	}

	private void Update() {
		if(brain) {
			brain.DoUpdate(this);
		}

		movementStateMachine.Update();
	}

	private void FixedUpdate() {
		if(brain) {
			brain.DoFixedUpdate(this);
		}

		movementStateMachine.FixedUpdate();
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if(brain) {
			brain.DoTriggerEnter2D(this, collision);
		}
	}

	protected override void Reset() {
		base.Reset();

		// Setup Rigidbody2D
		thisRigidbody2D = thisGameObject.AddOrGetComponent<Rigidbody2D>();
		thisRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
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

		// Setup Interactable
		thisInteractable = thisGameObject.AddOrGetComponent<Interactable>();

		// Setup Animator
		thisAnimator = this.GetComponentInChildren<Animator>(true);
		if(thisAnimator) {
			thisCharacterObject = thisAnimator.GetComponent<Transform>();
		}

		// Setup SpriteRenderer
		thisSpriteRenderer = thisCharacterObject.GetComponentInChildren<SpriteRenderer>(true);

		// Setup Collision Collider
		thisCollisionCollider2D = Utilities.CreateOrGetObject("Collision", thisTransform).AddOrGetComponent<BoxCollider2D>();
		thisCollisionCollider2D.size = thisCharacterObject ? thisSpriteRenderer.size * thisSpriteRenderer.GetComponent<Transform>().localScale :
															 thisCollisionCollider2D.size;
		
		// Setup Interaction Collider
		thisInteractionCollider2D = Utilities.CreateOrGetObject("Interaction", thisTransform).AddOrGetComponent<BoxCollider2D>();
		thisInteractionCollider2D.isTrigger = true;
	}


	public abstract class PowerupBrain : ActorBrain<PowerupActor> {
		public virtual bool DoInteracted(PowerupActor powerupActor, Direction direction, GameObject interactor) { return false; }
	}


	public abstract class PowerupState : State<PowerupActor> {
		public override void DoEnter(PowerupActor owner) { }
		public override void DoExit(PowerupActor owner) { }
	}


	public class MovementStates {
		public class Interact : PowerupState {

		}


		public class Move : PowerupState {
			public override void DoFixedUpdate(PowerupActor owner) {
				//owner.thisRigidbody2D.AddForce(owner.moveDirection * owner.moveSpeed * Time.fixedDeltaTime);
				owner.thisCharacter2D.Move(owner.moveDirection * owner.moveSpeed * Time.fixedDeltaTime);
			}
		}
	}
}