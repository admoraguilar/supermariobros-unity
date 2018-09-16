using UnityEngine;


public class Interactor : MonoBehaviour {
    private Character2D thisCharacter2D;


    private void Awake() {
        thisCharacter2D = GetComponent<Character2D>();
    }

    private void Update() {
        Collider2D colliderUp = thisCharacter2D.IsColliding(Character2D.Direction.Up);
        if(colliderUp) {
            Interactable interactable = colliderUp.GetComponent<Interactable>();
            if(interactable) {
                interactable.Interact(Interactable.InteractType.FromBelow);
                Destroy(interactable.gameObject);
            }
        }
    }
}
