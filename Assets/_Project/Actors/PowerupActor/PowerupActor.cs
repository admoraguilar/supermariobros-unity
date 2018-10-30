using UnityEngine;
using System;
using WishfulDroplet;
using WishfulDroplet.Extensions;


public class PowerupActor : Actor<PowerupActor, PowerupActor.PowerupBrain> {
	public Rigidbody2D thisRigidbody2D {
		get { return _thisRigidbody2D; }
		private set { _thisRigidbody2D = value; }
	}

	public Character2D thisCharacter2D {
		get { return _thisCharacter2D; }
		private set { _thisCharacter2D = value; }
	}

	public BoxCollider2D thisCollisionCollider2D {
		get { return _thisCollisionCollider2D; }
		private set { _thisCollisionCollider2D = value; }
	}

	public BoxCollider2D thisInteractionCollider2D {
		get { return _thisInteractionCollider2D; }
		private set { _thisInteractionCollider2D = value; }
	}

	[InspectorNote("Powerup Actor")]
	[Header("Data")]
	public _InternalActorBrain[] buffableBrains;
	public float moveSpeed;

	[Header("States")]
	[HideInInspector] public MovementStates.Interact interactMovementState = new MovementStates.Interact();
	[HideInInspector] public MovementStates.Move moveMovementState = new MovementStates.Move();

	[Header("Internal")]
	public Vector2 moveDirection;
	[HideInInspector] public StateMachine<PowerupActor, PowerupState> movementStateMachine = new StateMachine<PowerupActor, PowerupState>("MOVEMENT");

	[Header("References")]
	[SerializeField] private Rigidbody2D _thisRigidbody2D;
	[SerializeField] private Character2D _thisCharacter2D;
	[SerializeField] private BoxCollider2D _thisCollisionCollider2D;
	[SerializeField] private BoxCollider2D _thisInteractionCollider2D;


	public bool IsBrainBuffable(_InternalActorBrain brain) {
		return IsBrainOnSet(buffableBrains, brain);
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

		thisRigidbody2D = gameObject.AddOrGetComponent<Rigidbody2D>();

		thisCharacter2D = gameObject.AddOrGetComponent<Character2D>();

		thisCollisionCollider2D = gameObject.AddOrGetComponent<BoxCollider2D>();

		thisInteractionCollider2D = Utilities.CreateObject("Interaction", thisTransform).AddOrGetComponent<BoxCollider2D>();
		thisInteractionCollider2D.isTrigger = true;
	}


	public abstract class PowerupBrain : ActorBrain<PowerupActor> {

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