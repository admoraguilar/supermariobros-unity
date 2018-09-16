using UnityEngine;


public class Interactable : MonoBehaviour {
    [SerializeField] private InteractType interactType;


    public void Interact(InteractType type) {
        if(interactType != type) return;

        Debug.Log("Interacted!");
    }

    public enum InteractType {
        FromAbove,
        FromBelow,
        Pickup
    }
}
