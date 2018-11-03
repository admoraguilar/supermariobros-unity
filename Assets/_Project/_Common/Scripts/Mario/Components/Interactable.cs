using UnityEngine;
using System;
using WishfulDroplet;


public class Interactable : MonoBehaviour {
	public event Func<Direction, GameObject, bool>		OnInteract = delegate (Direction direction, GameObject gameObject) { return false; };

	public bool											isInfiniteInteract = true;
	public int											maxInteractCount = 1;

	[Header("Debug")]
	[SerializeField] private int						_remainingInteractCount;


	public void Interact(Direction direction, GameObject interactor) {
		if(!isInfiniteInteract && _remainingInteractCount <= 0) return;
		if(OnInteract(direction, interactor)) {
			_remainingInteractCount--;
		}
	}

	private void Start() {
		_remainingInteractCount = maxInteractCount;
	}
}
