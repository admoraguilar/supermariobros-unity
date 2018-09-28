using UnityEngine;
using System;
using System.Collections.Generic;


public class DirectionalInteractor : Interactor {
    public new event Action<Direction, Interactable> OnInteractEnter = delegate { };
    public new event Action<Direction, Interactable> OnInteractStay = delegate { };
    public new event Action<Direction, Interactable> OnInteractExit = delegate { };

    [Header("References")]
    [SerializeField] private DirectionalCollider2DDetector thisCollider2DDetectorDirectional;


    private void OnColliderEnter(Direction direction, Collider2D collider) {
        Interactable interactable = collider.GetComponent<Interactable>();
        if(!interactable) return;

        if(!interactables.Contains(interactable)) {
            // Abstract class compatibility
            interactables.AddToFront(interactable);
            RaiseOnInteractEnter(interactable);

            OnInteractEnter(direction, interactable);
        }
    }

    private void OnColliderStay(Direction direction, Collider2D collider) {
        Interactable interactable = collider.GetComponent<Interactable>();
        if(!interactable) return;

        if(interactables.Contains(interactable)) {
            // Abstract class compatibility
            RaiseOnInteractStay(interactable);

            OnInteractStay(direction, interactable);
        }
    }

    private void OnColliderExit(Direction direction, Collider2D collider) {
        Interactable interactable = collider.GetComponent<Interactable>();
        if(!interactable) return;

        if(!thisCollider2DDetectorDirectional.IsColliding(Direction.Any, false, collider)) {
            // Abstract class compatibility
            interactables.Remove(interactable);
            RaiseOnInteractExit(interactable);

            OnInteractExit(direction, interactable);
        }
    }

    public Interactable GetInteractable(Direction direction) {
        foreach(var collider in thisCollider2DDetectorDirectional.GetDetectedColliders(direction)) {
            return collider.GetComponent<Interactable>();
        }

        return null;
    }

    public IEnumerable<Interactable> GetInteractables(Direction direction) {
        List<Interactable> interactables = new List<Interactable>();
        foreach(var collider in thisCollider2DDetectorDirectional.GetDetectedColliders(direction)) {
            Interactable interactable = collider.GetComponent<Interactable>();
            if(interactable) {
                interactables.Add(interactable);
            }
        }

        return interactables;
    }

    public void Init(DirectionalCollider2DDetector collider2DDetectorDirectional) {
        thisCollider2DDetectorDirectional = collider2DDetectorDirectional;
    }

    private void Awake() {
        if(!thisCollider2DDetectorDirectional) thisCollider2DDetectorDirectional = GetComponent<DirectionalCollider2DDetector>();
    }

    private void Start() {
        thisCollider2DDetectorDirectional.OnColliderEnter += OnColliderEnter;
        thisCollider2DDetectorDirectional.OnColliderStay += OnColliderStay;
        thisCollider2DDetectorDirectional.OnColliderExit += OnColliderExit;
    }
}
