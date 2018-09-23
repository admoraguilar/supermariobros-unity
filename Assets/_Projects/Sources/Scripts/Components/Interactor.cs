using UnityEngine;
using System.Collections.Generic;


public abstract class Interactor : MonoBehaviour {
    [Header("Debug")]
    [SerializeField] protected List<Interactable> interactables = new List<Interactable>();


    public Interactable GetInteractable() {
        return interactables.Count > 0 ? interactables[0] : null;
    }

    public Interactable[] GetInteractables() {
        return interactables.Count > 0 ? interactables.ToArray() : null;
    }
}
