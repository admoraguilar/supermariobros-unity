using UnityEngine;
using System;
using System.Collections.Generic;
using WishfulDroplet;
using WishfulDroplet.Extensions;


public class Interactor : MonoBehaviour {
	public event Action<Direction, RaycastHit2D, Collider2D> OnDirectionalBoxCastHit = delegate { };

	[Header("Collision Detector")]
	[SerializeField] private DirectionalBoxCast2D directionalBoxCast = new DirectionalBoxCast2D();
	[SerializeField] private int maxHitBufferSize = 20;

	[Header("References")]
	[SerializeField] private GameObject _thisGameObject;
	[SerializeField] private Transform _thisTransform;
	[SerializeField] private BoxCollider2D _thisInteractionCollider;


	private void Awake() {
		directionalBoxCast.SetHitBufferSize(maxHitBufferSize);
	}

	private void Start() {
		//directionalBoxCast.OnHit += OnDirectionalBoxCastHit;
	}

	private void OnEnable() {
		directionalBoxCast.OnHit += OnDirectionalBoxCastHit;
	}

	private void OnDisable() {
		directionalBoxCast.OnHit -= OnDirectionalBoxCastHit;
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		directionalBoxCast.UpdateHits();
	}

	private void OnTriggerExit2D(Collider2D collision) {
		directionalBoxCast.UpdateHits();
	}

	private void Reset() {
		_thisGameObject = gameObject;
		_thisTransform = _thisGameObject.GetComponent<Transform>();

		// Set interaction collider
		_thisInteractionCollider = Utilities.CreateObject("Collision", _thisTransform).AddOrGetComponent<BoxCollider2D>();

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
		directionalBoxCast.referenceCollider = _thisInteractionCollider;
		directionalBoxCast.castMask = new List<Collider2D>(GetComponentsInChildren<Collider2D>(true));
	}
}
