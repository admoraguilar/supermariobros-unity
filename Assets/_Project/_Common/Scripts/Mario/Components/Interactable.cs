using UnityEngine;
using System;


public class Interactable : MonoBehaviour {
	public event Func<GameObject, bool> OnInteract = delegate (GameObject gameObject) { return false; };

	public bool isInfiniteInteract = true;
	public int maxInteractCount = 1;

	[Header("Debug")]
	[SerializeField] private int remainingInteractCount;


	public void Interact(GameObject interactor) {
		if(!isInfiniteInteract && remainingInteractCount <= 0) return;
		if(OnInteract(interactor)) {
			remainingInteractCount--;
		}
	}

	private void Start() {
		remainingInteractCount = maxInteractCount;
	}
}
