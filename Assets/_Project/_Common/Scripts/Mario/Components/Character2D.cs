using UnityEngine;
using System;
using System.Collections.Generic;
using WishfulDroplet;
using WishfulDroplet.Extensions;


public class Character2D : MonoBehaviour {
	public event Action<Direction, RaycastHit2D, Collider2D>		OnCasterHit = delegate { };

	public Vector2									maxVelocity = new Vector2(5f, 8f);
	public bool										isUpdateFaceDirectionOnlyOnGround = true;
	public bool										isGroundCheckIncludeYVelocity = true;

	[Header("Collision Detection")]
	[SerializeField] private DirectionalBoxCast2D	_collisionCaster = new DirectionalBoxCast2D();
	[SerializeField] private int					_maxCasterHitBufferSize = 20;

	[Header("Debug")]
	[SerializeField] private Vector2				_moveDirection;
	[SerializeField] private Vector2				_faceDirection;
	[SerializeField] private bool					_isGrounded;

	[Header("References")]
	[SerializeField] private GameObject				_thisGameObject;
	[SerializeField] private Transform				_thisTransform;
	[SerializeField] private Rigidbody2D			_thisRigidbody2D;
	[SerializeField] private BoxCollider2D			_thisCollisionCollider2D;

	public bool										isChangingDirection {
		get {
			return (_thisRigidbody2D.velocity.x > 0f && IsFacing(Direction.Left)) ||
				   (_thisRigidbody2D.velocity.x < 0f && IsFacing(Direction.Right));
		}
	}

	public Vector2									faceDirection {
		get { return _faceDirection; }
		private set { _faceDirection = value; }
	}

	public bool										isGrounded {
		get { return _isGrounded; }
		private set { _isGrounded = value; }
	}

	public GameObject								thisGameObject {
		get { return _thisGameObject; }
		private set { _thisGameObject = value; }
	}

	public Transform								thisTransform {
		get { return _thisTransform; }
		private set { _thisTransform = value; }
	}

	public Rigidbody2D								thisRigidbody2D {
		get { return _thisRigidbody2D; }
		private set { _thisRigidbody2D = value; }
	}

	public BoxCollider2D							thisCollisionCollider2D {
		get { return _thisCollisionCollider2D; }
		private set { _thisCollisionCollider2D = value; }
	}


	public bool IsColliding(CollisionFilter filter) {
		return _collisionCaster.IsHitAtAnyDirection(filter);
	}

	public bool IsColliding(Collider2D collider) {
		return _collisionCaster.IsHitAtAnyDirection(collider);
	}

	public bool IsColliding(Direction direction, CollisionFilter filter) {
		return _collisionCaster.IsHitAt(direction, filter);
	}

	public bool IsColliding(Direction direction, Collider2D collider) {
		return _collisionCaster.IsHitAt(direction, collider);
	}

	public bool IsMoving() {
		return thisRigidbody2D.velocity != Vector2.zero;
	}

	public bool IsMoving(Direction direction) {
		switch(direction) {
			case Direction.Up:		return thisRigidbody2D.velocity.y > 0f;
			case Direction.Down:	return thisRigidbody2D.velocity.y < 0f;
			case Direction.Left:	return thisRigidbody2D.velocity.x < 0f;
			case Direction.Right:	return thisRigidbody2D.velocity.x > 0f;
			default: return false;
		}
	}

	public bool IsFacing(Direction direction) {
		switch(direction) {
			case Direction.Up:		return faceDirection.y > 0f;
			case Direction.Down:	return faceDirection.y < 0f;
			case Direction.Left:	return faceDirection.x < 0f;
			case Direction.Right:	return faceDirection.x > 0f;
			default: return true;
		}
	}

	public List<Collider2D> GetHits(Direction direction) {
		DirectionalBoxCast2D.BoxCastInfo info = _collisionCaster.GetBoxCastInfo(direction);
		if(info != null) {
			return info.hits;
		}

		return null;
	}

	public void Move(Vector2 direction) {
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
		_collisionCaster.SetHitBufferSize(_maxCasterHitBufferSize);
	}

	private void OnEnable() {
		_collisionCaster.OnHit += OnCasterHit;
	}

	private void OnDisable() {
		_collisionCaster.OnHit -= OnCasterHit;
	}

	private void Update() {
		// Bugs: thisRigidbody.velocity.y is having some funny values for some reason
		//       when walking or sprinting. Investigate this when you have some time
		//       For now we HOTFIX it by doing a "||" operator instead of an "&&" 
		//       operator
		// FIXED: It was not the code, but the composite collider issues, it seems to be 
		//        a Unity bug where the vertex snapping leaves very small gaps that could
		//        screw around with collisions
		_isGrounded = IsColliding(Direction.Down, CollisionFilter.OnlyNonTrigger) && (isGroundCheckIncludeYVelocity ? _thisRigidbody2D.velocity.y == 0f : true);

		// Debug
		//directionalBoxCast.GetHits();
	}

	private void FixedUpdate() {
		if(isUpdateFaceDirectionOnlyOnGround && _isGrounded) {
			_faceDirection.x = _moveDirection.x < 0f ? -1f : _moveDirection.x > 0f ? 1f : faceDirection.x;
			_faceDirection.y = _moveDirection.y < 0f ? -1f : _moveDirection.y > 0f ? 1f : faceDirection.y;
		}

		thisRigidbody2D.AddForce(_moveDirection, ForceMode2D.Force);
		//thisRigidbody2D.velocity += moveDirection;
		_moveDirection = Vector2.zero;

		ClampVelocity();
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		_collisionCaster.UpdateHits();
	}

	private void OnCollisionStay2D(Collision2D collision) {
		_collisionCaster.UpdateHits();
	}

	private void OnCollisionExit2D(Collision2D collision) {
		_collisionCaster.UpdateHits();
	}

	private void OnDrawGizmos() {
		// Draw gizmos for the collision caster
		Gizmos.color = Color.red;
		if(_collisionCaster.boxCastInfos != null) {
			for(int i = 0; i < _collisionCaster.boxCastInfos.Count; i++) {
				Gizmos.DrawWireCube(_collisionCaster.boxCastInfos[i].origin,
									_collisionCaster.boxCastInfos[i].size);

				Gizmos.DrawRay(_collisionCaster.boxCastInfos[i].origin,
							   _collisionCaster.boxCastInfos[i].castDirection * _collisionCaster.boxCastInfos[i].distance);
			}
		}
	}

	private void Reset() {
		thisGameObject = gameObject;
		thisTransform = thisGameObject.GetComponent<Transform>();
		thisRigidbody2D = thisGameObject.AddOrGetComponent<Rigidbody2D>();

		// Set collision collider
		thisCollisionCollider2D = Utilities.CreateOrGetObject("Collision", thisTransform).AddOrGetComponent<BoxCollider2D>();

		// Set directional boxcast
		_collisionCaster.boxCastInfos = new List<DirectionalBoxCast2D.BoxCastInfo> {
			new DirectionalBoxCast2D.BoxCastInfo {
				direction = Direction.Up,
				referenceSizeMultiplier = .9f,
				directionSizeMultiplier = .02f,
				distance = .02f
			},
			new DirectionalBoxCast2D.BoxCastInfo {
				direction = Direction.Down,
				referenceSizeMultiplier = 1f,
				directionSizeMultiplier = .02f,
				distance = .05f
			},
			new DirectionalBoxCast2D.BoxCastInfo {
				direction = Direction.Left,
				referenceSizeMultiplier = 1f,
				directionSizeMultiplier = .02f,
				distance = .02f
			},
			new DirectionalBoxCast2D.BoxCastInfo {
				direction = Direction.Right,
				referenceSizeMultiplier = 1f,
				directionSizeMultiplier = .02f,
				distance = .02f
			}
		};
		_collisionCaster.boxCastMask = new List<Collider2D>(GetComponentsInChildren<Collider2D>(true));
		_collisionCaster.referenceCollider = thisCollisionCollider2D;
		_collisionCaster.layerMask = LayerMask.GetMask("Default");
	}
}
