using UnityEngine;
using System;


public class Interactable : MonoBehaviour {
    public event Action<Interactor> OnInteract = delegate { };

    [SerializeField] private bool isInfiniteInteract = true;
    [SerializeField] private int maxInteract = 1;

    [Header("Debug")]
    [SerializeField] private int remainingInteract;


    public void Interact(Interactor interactor) {
        if(remainingInteract <= 0) return;
        OnInteract(interactor);
        if(!isInfiniteInteract) remainingInteract--;
    }

    private void Start() {
        remainingInteract = maxInteract;
    }
}
