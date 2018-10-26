using UnityEngine;
using WishfulDroplet;
using WishfulDroplet.Extensions;


public class SandboxCharacterMover : MonoBehaviour {
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

	[Header("Data")]
	public float moveSpeed = 5f;

	[Header("Debug")]
	[SerializeField] private Vector2 _inputAxis;

	[Header("References")]
	[SerializeField] private GameObject _thisGameObject;
	[SerializeField] private Transform _thisTransform;
	[SerializeField] private Rigidbody2D _thisRigidbody2D;


	private void Update() {
		_inputAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		thisRigidbody2D.MovePosition(thisTransform.position + 
									(new Vector3(_inputAxis.x, _inputAxis.y, 0f) * moveSpeed * Time.deltaTime));
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		//Debug.Log(string.Format("Collided with: {0}", collision.collider.name));
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		Debug.Log(string.Format("Trigger from: {0}", collision.name));
	}

	private void Reset() {
		thisGameObject = gameObject;

		thisTransform = thisGameObject.transform;

		thisRigidbody2D = thisGameObject.AddOrGetComponent<Rigidbody2D>();
		thisRigidbody2D.isKinematic = true;
		thisRigidbody2D.useFullKinematicContacts = true;
	}
}
