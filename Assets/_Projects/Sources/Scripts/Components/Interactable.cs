using UnityEngine;
using System;


public class Interactable : MonoBehaviour {
    public event Action<Interactor>         OnInteract = delegate { };

    [SerializeField] protected bool         isInfiniteInteract = true;
    [SerializeField] protected int          maxInteract = 1;

    [Header("Debug")]
    [SerializeField] protected int          remainingInteract;


    protected virtual void DoInteract(Interactor interactor) { }

    public void Interact(Interactor interactor) {
        if(remainingInteract <= 0 && !isInfiniteInteract) return;
        else remainingInteract--;

        OnInteract(interactor);
        DoInteract(interactor);
    }

    private void Start() {
        remainingInteract = maxInteract;
    }
}
