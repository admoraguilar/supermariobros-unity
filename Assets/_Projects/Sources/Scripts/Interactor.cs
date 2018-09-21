using UnityEngine;
using System;
using System.Collections.Generic;


public class Interactor : MonoBehaviour {
    public event Action<Direction, Interactable> OnInteractEnter = delegate { };
    public event Action<Direction, Interactable> OnInteractStay = delegate { };
    public event Action<Direction, Interactable> OnInteractExit = delegate { };

    [SerializeField] private List<Interactable> interactables = new List<Interactable>();

    [Header("References")]
    [SerializeField] private ColliderDetector2D thisColliderDetector2D;


    public Interactable GetInteractable() {
        return interactables.Count > 0 ? interactables[0] : null;
    }

    public Interactable[] GetInteractables() {
        return interactables.Count > 0 ? interactables.ToArray() : null;
    }

    private void OnColliderEnter(Direction direction, Collider2D collider) {
        Interactable interactable = collider.GetComponent<Interactable>();
        if(!interactable) return;

        if(!interactables.Find((Interactable i) => { return interactable == i; })) {
            interactables.AddToFront(interactable);
        }

        OnInteractEnter(direction, interactable);
    }

    private void OnColliderStay(Direction direction, Collider2D collider) {
        Interactable interactable = collider.GetComponent<Interactable>();
        if(!interactable) return;

        if(interactables.Contains(interactable)) {
            OnInteractStay(direction, interactable);
        }
    }

    private void OnColliderExit(Direction direction, Collider2D collider) {
        Interactable interactable = collider.GetComponent<Interactable>();
        if(!interactable) return;

        if(!thisColliderDetector2D.IsColliding(Direction.Any)) {
            interactables.Remove(interactable);
        }

        OnInteractExit(direction, interactable);
    }

    private void Awake() {
        if(!thisColliderDetector2D) thisColliderDetector2D = GetComponent<ColliderDetector2D>();
    }

    private void Start() {
        thisColliderDetector2D.OnColliderEnter += OnColliderEnter;
        thisColliderDetector2D.OnColliderStay += OnColliderStay;
        thisColliderDetector2D.OnColliderExit += OnColliderExit;
    }
}
