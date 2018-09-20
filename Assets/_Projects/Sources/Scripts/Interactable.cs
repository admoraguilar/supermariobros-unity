using UnityEngine;
using System;


[RequireComponent(typeof(BoxCollider2D))]
public class Interactable : MonoBehaviour {
    public event Action<Interactor> OnInteract = delegate { };

    [SerializeField] private int maxInteract = 1;

    [SerializeField] private int remainingInteract;


    public void Interact(Interactor interactor) {
        if(remainingInteract <= 0) return;
        OnInteract(interactor);
        remainingInteract--;
    }

    private void Start() {
        remainingInteract = maxInteract;
    }
}
