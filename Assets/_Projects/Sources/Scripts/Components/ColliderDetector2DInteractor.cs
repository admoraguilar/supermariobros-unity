using UnityEngine;
using System;


public class ColliderDetector2DInteractor : Interactor {
    public event Action<ColliderDetector2D.Direction, Interactable> OnInteractEnter = delegate { };
    public event Action<ColliderDetector2D.Direction, Interactable> OnInteractStay = delegate { };
    public event Action<ColliderDetector2D.Direction, Interactable> OnInteractExit = delegate { };

    [Header("References")]
    [SerializeField] private ColliderDetector2D thisColliderDetector2D;


    private void OnColliderEnter(ColliderDetector2D.Direction direction, Collider2D collider) {
        Interactable interactable = collider.GetComponent<Interactable>();
        if(!interactable) return;

        if(!interactables.Find((Interactable i) => { return interactable == i; })) {
            interactables.AddToFront(interactable);
            OnInteractEnter(direction, interactable);
        }
    }

    private void OnColliderStay(ColliderDetector2D.Direction direction, Collider2D collider) {
        Interactable interactable = collider.GetComponent<Interactable>();
        if(!interactable) return;

        if(interactables.Contains(interactable)) {
            OnInteractStay(direction, interactable);
        }
    }

    private void OnColliderExit(ColliderDetector2D.Direction direction, Collider2D collider) {
        Interactable interactable = collider.GetComponent<Interactable>();
        if(!interactable) return;

        if(!thisColliderDetector2D.IsColliding(ColliderDetector2D.Direction.Any, collider)) {
            interactables.Remove(interactable);
            OnInteractExit(direction, interactable);
        }
    }

    public void Init(ColliderDetector2D colliderDetector2D) {
        thisColliderDetector2D = colliderDetector2D;
    }

    private void Start() {
        thisColliderDetector2D.OnColliderEnter += OnColliderEnter;
        thisColliderDetector2D.OnColliderStay += OnColliderStay;
        thisColliderDetector2D.OnColliderExit += OnColliderExit;
    }
}
