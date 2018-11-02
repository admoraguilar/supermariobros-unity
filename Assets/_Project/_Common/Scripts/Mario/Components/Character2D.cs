using UnityEngine;
using System;
using System.Collections.Generic;
using WishfulDroplet;
using WishfulDroplet.Extensions;


[ExecuteInEditMode]
public class Character2D : MonoBehaviour {
	public event Action<Direction, RaycastHit2D, Collider2D> OnDirectionalBoxCastHit = delegate { };

	public Vector2 faceAxis {
		get { return _faceAxis; }
		private set { _faceAxis = value; }
	}

	public bool isGrounded {
		get { return _isGrounded; }
	}

	public bool isChangingDirection {
		get {
			return (_thisRigidbody2D.velocity.x > 0f && IsFacing(Direction.Left)) ||
				   (_thisRigidbody2D.velocity.x < 0f && IsFacing(Direction.Right));
		}
	}

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

	public BoxCollider2D thisBoxCollider2D {
		get { return _thisBoxCollider2D; }
		private set { _thisBoxCollider2D = value; }
	}

	public Vector2 maxVelocity = new Vector2(5f, 8f);
	public bool isUpdateFaceAxisOnlyOnGround = true;
	public bool isGroundCheckIncludeYVelocity = true;

	[Header("Collision Detector")]
	[SerializeField] private DirectionalBoxCast2D directionalBoxCast = new DirectionalBoxCast2D();
	[SerializeField] private int maxHitBufferSize = 20;

	[Header("Debug")]
	[SerializeField] private Vector2 _moveDirection;
	[SerializeField] private Vector2 _faceAxis;
	[SerializeField] private bool _isGrounded;

	[Header("References")]
	[SerializeField] private GameObject _thisGameObject;
	[SerializeField] private Transform _thisTransform;
	[SerializeField] private Rigidbody2D _thisRigidbody2D;
	[SerializeField] private BoxCollider2D _thisBoxCollider2D;


	public bool IsColliding(CollisionFilter filter) {
		return directionalBoxCast.IsHittingAtAnyDirection(filter);
	}

	public bool IsColliding(Collider2D collider) {
		return directionalBoxCast.IsHittingAtAnyDirection(collider);
	}

	public bool IsColliding(Direction direction, CollisionFilter filter) {
		return directionalBoxCast.IsHittingAt(direction, filter);
	}

	public bool IsColliding(Direction direction, Collider2D collider) {
		return directionalBoxCast.IsHittingAt(direction, collider);
	}

	public bool IsMoving() {
		return thisRigidbody2D.velocity != Vector2.zero;
	}

	public bool IsMoving(Direction direction) {
		switch(direction) {
			case Direction.Up: return thisRigidbody2D.velocity.y > 0f;
			case Direction.Down: return thisRigidbody2D.velocity.y < 0f;
			case Direction.Left: return thisRigidbody2D.velocity.x < 0f;
			case Direction.Right: return thisRigidbody2D.velocity.x > 0f;
			default: return false;
		}
	}

	public bool IsFacing(Direction direction) {
		switch(direction) {
			case Direction.Up: return faceAxis.y > 0f;
			case Direction.Down: return _faceAxis.y < 0f;
			case Direction.Left: return _faceAxis.x < 0f;
			case Direction.Right: return _faceAxis.x > 0f;
			default: return true;
		}
	}

	public List<Collider2D> GetHits(Direction direction) {
		DirectionalBoxCast2D.BoxCastInfo info = directionalBoxCast.GetBoxCastInfo(direction);
		if(info != null) {
			return info.hits;
		}

		return null;
	}

	public void SetVelocity(Vector2 velocity) {
		thisRigidbody2D.velocity = velocity;
	}

	public void Move(Vector2 direction) {
		// This could be optimized
		if(IsColliding(Direction.Up, CollisionFilter.OnlyNonTrigger) && direction.y > 0) {
			direction.y = 0f;
		}
		if(IsColliding(Direction.Down, CollisionFilter.OnlyNonTrigger) && direction.y < 0) {
			direction.y = 0f;
		}
		if(IsColliding(Direction.Left, CollisionFilter.OnlyNonTrigger) && direction.x < 0) {
			direction.x = 0f;
		}
		if(IsColliding(Direction.Right, CollisionFilter.OnlyNonTrigger) && direction.x > 0) {
			direction.x = 0f;
		}

		_moveDirection += direction;
	}

	private void ClampVelocity() {
		if(Mathf.Abs(_thisRigidbody2D.velocity.x) > maxVelocity.x) {
			thisRigidbody2D.velocity = new Vector2(maxVelocity.x * Mathf.Sign(thisRigidbody2D.velocity.x), thisRigidbody2D.velocity.y);
		}

		if(Mathf.Abs(_thisRigidbody2D.velocity.y) > maxVelocity.y) {
			thisRigidbody2D.velocity = new Vector2(thisRigidbody2D.velocity.x, maxVelocity.y * Mathf.Sign(thisRigidbody2D.velocity.y));
		}
	}

	private void Awake() {
		directionalBoxCast.SetHitBufferSize(maxHitBufferSize);
	}

	private void OnEnable() {
		directionalBoxCast.OnHit += OnDirectionalBoxCastHit;
	}

	private void OnDisable() {
		directionalBoxCast.OnHit -= OnDirectionalBoxCastHit;
	}

	private void DoDirectionalBoxCastHit(Direction direction, RaycastHit2D hit, Collider2D collider) {
		OnDirectionalBoxCastHit(direction, hit, collider);
	}

	private void Update() {
		// Bugs: thisRigidbody.velocity.y is having some funny values for some reason
		//       when walking or sprinting. Investigate this when you have some time
		//       For now we HOTFIX it by doing a "||" operator instead of an "&&" 
		//       operator
		// FIXED: It was not the code, but the composite collider issues, it seems to be 
		//        a Unity bug where the vertex snapping leaves very small gaps that could
		//        screw around with collisions
		//isGrounded = IsColliding(Direction.Down) && thisRigidbody2D.velocity.y == 0f;
		_isGrounded = IsColliding(Direction.Down, CollisionFilter.OnlyNonTrigger) && (isGroundCheckIncludeYVelocity ? _thisRigidbody2D.velocity.y == 0f : true);

		// Debug
		//directionalBoxCast.GetHits();
	}

	private void FixedUpdate() {
		if(isUpdateFaceAxisOnlyOnGround && _isGrounded) {
			_faceAxis.x = _moveDirection.x < 0f ? -1f : _moveDirection.x > 0f ? 1f : _faceAxis.x;
			_faceAxis.y = _moveDirection.y < 0f ? -1f : _moveDirection.y > 0f ? 1f : _faceAxis.y;
		}

		thisRigidbody2D.AddForce(_moveDirection, ForceMode2D.Force);
		//thisRigidbody2D.velocity += moveDirection;
		_moveDirection = Vector2.zero;

		ClampVelocity();
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		directionalBoxCast.UpdateHits();
	}

	private void OnCollisionStay2D(Collision2D collision) {
		directionalBoxCast.UpdateHits();
	}

	private void OnCollisionExit2D(Collision2D collision) {
		directionalBoxCast.UpdateHits();
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.red;
		if(directionalBoxCast.boxCastInfos != null) {
			for(int i = 0; i < directionalBoxCast.boxCastInfos.Count; i++) {
				Gizmos.DrawWireCube(directionalBoxCast.boxCastInfos[i].origin,
									directionalBoxCast.boxCastInfos[i].size);

				Gizmos.DrawRay(directionalBoxCast.boxCastInfos[i].origin,
							   directionalBoxCast.boxCastInfos[i].castDirection * directionalBoxCast.boxCastInfos[i].distance);
			}
		}
	}

	private void Reset() {
		thisGameObject = gameObject;
		thisTransform = thisGameObject.GetComponent<Transform>();
		thisRigidbody2D = thisGameObject.AddOrGetComponent<Rigidbody2D>();

		// Set collision collider
		thisBoxCollider2D = Utilities.CreateObject("Collision", thisTransform).AddOrGetComponent<BoxCollider2D>();

		// Set directional boxcast
		directionalBoxCast.boxCastInfos = new List<DirectionalBoxCast2D.BoxCastInfo> {
			new DirectionalBoxCast2D.BoxCastInfo {
				direction = Direction.Up,
				directionSizeMultiplier = .02f,
				referenceSizeMultiplier = .9f,
				distance = .02f
			},
			new DirectionalBoxCast2D.BoxCastInfo {
				direction = Direction.Down,
				directionSizeMultiplier = .02f,
				referenceSizeMultiplier = 1f,
				distance = .1f
			},
			new DirectionalBoxCast2D.BoxCastInfo {
				direction = Direction.Left,
				directionSizeMultiplier = .02f,
				referenceSizeMultiplier = 1f,
				distance = .02f
			},
			new DirectionalBoxCast2D.BoxCastInfo {
				direction = Direction.Right,
				directionSizeMultiplier = .02f,
				referenceSizeMultiplier = 1f,
				distance = .02f
			}
		};
		directionalBoxCast.referenceCollider = thisBoxCollider2D;
		directionalBoxCast.castMask = new List<Collider2D>(GetComponentsInChildren<Collider2D>(true));
	}
}
