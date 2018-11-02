using UnityEngine;
using System;
using System.Collections.Generic;
using WishfulDroplet;
using WishfulDroplet.Extensions;


public class Interactor : MonoBehaviour {
	public event Action<Direction, RaycastHit2D, Collider2D>		OnCasterHit = delegate { };

	[Header("Collision Detector")]
	[SerializeField] private DirectionalBoxCast2D					_interactionCaster = new DirectionalBoxCast2D();
	[SerializeField] private int									_maxCasterBufferSize = 20;

	[Header("References")]
	[SerializeField] private GameObject								_thisGameObject;
	[SerializeField] private Transform								_thisTransform;
	[SerializeField] private BoxCollider2D							_thisInteractionCollider;

	public GameObject												thisGameObject {
		get { return _thisGameObject; }
		private set { _thisGameObject = value; }
	}

	public Transform												thisTransform {
		get { return _thisTransform; }
		private set { _thisTransform = value; }
	}

	public BoxCollider2D											thisInteractionCollider {
		get { return _thisInteractionCollider; }
		private set { _thisInteractionCollider = value; }
	}


	private void Awake() {
		_interactionCaster.SetHitBufferSize(_maxCasterBufferSize);
	}

	private void OnEnable() {
		_interactionCaster.OnHit += OnCasterHit;
	}

	private void OnDisable() {
		_interactionCaster.OnHit -= OnCasterHit;
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		//Debug.Log(string.Format("Interacted by: {0}.{1}", collision.transform.root.name, collision.name));
		_interactionCaster.UpdateHits();
	}

	private void OnTriggerExit2D(Collider2D collision) {
		_interactionCaster.UpdateHits();
	}

	private void Reset() {
		thisGameObject = gameObject;
		thisTransform = _thisGameObject.GetComponent<Transform>();

		// Set interaction collider
		thisInteractionCollider = Utilities.CreateObject("Collision", _thisTransform).AddOrGetComponent<BoxCollider2D>();

		// Set directional boxcast
		_interactionCaster.boxCastInfos = new List<DirectionalBoxCast2D.BoxCastInfo> {
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
		_interactionCaster.boxCastMask = new List<Collider2D>(GetComponentsInChildren<Collider2D>(true));
		_interactionCaster.referenceCollider = _thisInteractionCollider;
		_interactionCaster.layerMask = LayerMask.GetMask("Interaction");
	}
}
