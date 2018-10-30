using UnityEngine;
using System;
using WishfulDroplet;
using WishfulDroplet.Components;
using WishfulDroplet.Extensions;


public class CharacterActor : Actor<CharacterActor, CharacterActor.CharacterBrain> {
	public Rigidbody2D thisRigidbody2D {
		get { return _thisRigidbody2D; }
		private set { _thisRigidbody2D = value; }
	}

	public Character2D thisCharacter2D {
		get { return _thisCharacter2D; }
		private set { _thisCharacter2D = value; }
	}

	public Interactable thisInteractable {
		get { return _thisInteractable; }
		private set { _thisInteractable = value; }
	}

	public Animator thisAnimator {
		get { return _thisAnimator; }
		private set { _thisAnimator = value; }
	}

	public Transform thisCharacterObject {
		get { return _thisCharacterObject; }
		private set { _thisCharacterObject = value; }
	}

	public SpriteRenderer thisSpriteRenderer {
		get { return _thisSpriteRenderer; }
		private set { _thisSpriteRenderer = value; }
	}

	public BoxCollider2D thisCollisionCollider2D {
		get { return _thisCollisionCollider2D; }
		private set { _thisCollisionCollider2D = value; }
	}

	public BoxCollider2D thisInteractionCollider2D {
		get { return _thisInteractionCollider2D; }
		private set { _thisInteractionCollider2D = value; }
	}

	public CollisionEvents2D thisInteractionColliderEvents {
		get { return _thisInteractionColliderEvents; }
		private set { _thisInteractionColliderEvents = value; }
	}


	[InspectorNote("Character Actor")]
	[Header("Data")]
	[SerializeField] private CharacterBrain[] enemyBrains;
	[SerializeField] private CharacterBrain[] powerUpBrains;
	public float gravity = 9.8f;
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
	public Vector2 inputAxis = Vector2.zero;
	public Vector2 lastJumpPos = Vector2.zero;
	public StateController stateController = new StateController();
	public StateMachine<CharacterActor, FormStates.FormState> formStateMachine = new StateMachine<CharacterActor, FormStates.FormState>("FORM");
	public StateMachine<CharacterActor, CharacterState> movementStateMachine = new StateMachine<CharacterActor, CharacterState>("MOVEMENT");
	public StateMachine<CharacterActor, CharacterState> statusStateMachine = new StateMachine<CharacterActor, CharacterState>("STATUS");

	[Header("References")]
	[SerializeField] private Rigidbody2D _thisRigidbody2D;
	[SerializeField] private Character2D _thisCharacter2D;
	[SerializeField] private Interactable _thisInteractable;
	[SerializeField] private Animator _thisAnimator;
	[SerializeField] private Transform _thisCharacterObject;
	[SerializeField] private SpriteRenderer _thisSpriteRenderer;
	[SerializeField] private BoxCollider2D _thisCollisionCollider2D;
	[SerializeField] private BoxCollider2D _thisInteractionCollider2D;
	[SerializeField] private CollisionEvents2D _thisInteractionColliderEvents;


	public bool IsBrainEnemy(CharacterBrain brain) {
		return IsBrainOnSet(enemyBrains, brain);
	}

	public bool IsBrainPowerup(CharacterBrain brain) {
		return IsBrainOnSet(powerUpBrains, brain);
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

	private bool OnInteract(GameObject interactor) {
		if(brain) {
			return brain.DoInteract(this, interactor);
		}

		return false;
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

		thisInteractable.OnInteract += OnInteract;

		thisInteractionColliderEvents.OnCollisionEnter2DCallback += _OnCollisionEnter2D;
		thisInteractionColliderEvents.OnCollisionStay2DCallback += _OnCollisionStay2D;
		thisInteractionColliderEvents.OnCollisionExit2DCallback += _OnCollisionExit2D;
		thisInteractionColliderEvents.OnTriggerEnter2DCallback += _OnTriggerEnter2D;
		thisInteractionColliderEvents.OnTriggerStay2DCallback += _OnTriggerStay2D;
		thisInteractionColliderEvents.OnTriggerExit2DCallback += _OnTriggerExit2D;
	}

	private void OnDisable() {
		if(brain) {
			brain.DoOnDisable(this);
		}

		thisInteractable.OnInteract -= OnInteract;

		thisInteractionColliderEvents.OnCollisionEnter2DCallback -= _OnCollisionEnter2D;
		thisInteractionColliderEvents.OnCollisionStay2DCallback -= _OnCollisionStay2D;
		thisInteractionColliderEvents.OnCollisionExit2DCallback -= _OnCollisionExit2D;
		thisInteractionColliderEvents.OnTriggerEnter2DCallback -= _OnTriggerEnter2D;
		thisInteractionColliderEvents.OnTriggerStay2DCallback -= _OnTriggerStay2D;
		thisInteractionColliderEvents.OnTriggerExit2DCallback -= _OnTriggerExit2D;
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

	private void _OnCollisionEnter2D(Collision2D collision) {
		if(brain) {
			brain.DoCollisionEnter2D(this, collision);
		}
	}

	private void _OnCollisionStay2D(Collision2D collision) {
		if(brain) {
			brain.DoCollisionStay2D(this, collision);
		}
	}

	private void _OnCollisionExit2D(Collision2D collision) {
		if(brain) {
			brain.DoCollisionExit2D(this, collision);
		}
	}

	private void _OnTriggerEnter2D(Collider2D collision) {
		if(brain) {
			brain.DoTriggerEnter2D(this, collision);
		}
	}

	private void _OnTriggerStay2D(Collider2D collision) {
		if(brain) {
			brain.DoTriggerStay2D(this, collision);
		}
	}

	private void _OnTriggerExit2D(Collider2D collision) {
		if(brain) {
			brain.DoTriggerExit2D(this, collision);
		}
	}

	private void OnDrawGizmos() {
		if(brain) {
			brain.DoDrawGizmos(this);
		}
	}

	protected override void Reset() {
		base.Reset();

		// Setup Rigidbody2D
		thisRigidbody2D = _thisGameObject.AddOrGetComponent<Rigidbody2D>();
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

		// Setup Animator
		thisAnimator = this.GetComponentInChildren<Animator>(true);
		if(thisAnimator) {
			thisCharacterObject = thisAnimator.GetComponent<Transform>();
		}

		// Setup SpriteRenderer
		thisSpriteRenderer = thisCharacterObject.GetComponentInChildren<SpriteRenderer>(true); 

		// Setup collision collider
		thisCollisionCollider2D = Utilities.CreateObject("Collision", thisTransform).AddOrGetComponent<BoxCollider2D>();
		thisCollisionCollider2D.size = thisCharacterObject ? thisSpriteRenderer.size * thisSpriteRenderer.GetComponent<Transform>().localScale :
															 thisCollisionCollider2D.size;

		// Setup interaction collider
		thisInteractionCollider2D = Utilities.CreateObject("Interaction", thisTransform).AddOrGetComponent<BoxCollider2D>();
		thisInteractionCollider2D.isTrigger = true;
		thisInteractionCollider2D.size = thisCollisionCollider2D.size;

		// Setup interaction collider events
		thisInteractionColliderEvents = thisInteractionCollider2D.gameObject.AddOrGetComponent<CollisionEvents2D>();
	}


	public abstract class CharacterBrain : ActorBrain<CharacterActor> {
		public bool isUpdateInput = true;


		public virtual void UpdateInput(CharacterActor characterActor) { }
		public virtual bool DoInteract(CharacterActor characterActor, GameObject interactor) { return false; }
	}


	[Serializable]
	public abstract class CharacterState : State<CharacterActor> {
		public override void DoEnter(CharacterActor characterActor) { }
		public override void DoExit(CharacterActor characterActor) { }
	}


	#region STATES 

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

	#endregion
}