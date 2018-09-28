using UnityEngine;
using System;
using System.Collections.Generic;


public abstract class Interactor : MonoBehaviour {
    public event Action<Interactable> OnInteractEnter = delegate { };
    public event Action<Interactable> OnInteractStay = delegate { };
    public event Action<Interactable> OnInteractExit = delegate { };

    protected void RaiseOnInteractEnter(Interactable interactable) { OnInteractEnter(interactable); }
    protected void RaiseOnInteractStay(Interactable interactable) { OnInteractStay(interactable); }
    protected void RaiseOnInteractExit(Interactable interactable) { OnInteractExit(interactable); }

    [Header("Debug")]
    [SerializeField] protected List<Interactable> interactables;


    protected virtual void UpdateInteractor() { }


    public Interactable GetInteractable() {
        return interactables[0];
    }

    public IEnumerable<Interactable> GetInteractables() {
        return interactables;
    }

    private void Update() {
        for(int i = 0; i < interactables.Count; i++) {
            OnInteractStay(interactables[i]);
        }

        UpdateInteractor();
    }
}
