using UnityEngine;
using System;


public class InteractController : MonoBehaviour {
    public Interactable CheckInteractable(Interactor interactor) {
        Interactable interactable = interactor.GetInteractable();
        return interactable;
    }

    public Interactable[] CheckInteractables(Interactor interactor) {
        Interactable[] interactables = interactor.GetInteractables();
        return interactables;
    }
}
