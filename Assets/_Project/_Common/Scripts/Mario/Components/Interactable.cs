using UnityEngine;
using System;


public class Interactable : MonoBehaviour {
	public event Func<GameObject, bool>		OnInteract = delegate (GameObject gameObject) { return false; };

	public bool								isInfiniteInteract = true;
	public int								maxInteractCount = 1;

	[Header("Debug")]
	[SerializeField] private int			_remainingInteractCount;


	public void Interact(GameObject interactor) {
		if(!isInfiniteInteract && _remainingInteractCount <= 0) return;
		if(OnInteract(interactor)) {
			_remainingInteractCount--;
		}
	}

	private void Start() {
		_remainingInteractCount = maxInteractCount;
	}
}
