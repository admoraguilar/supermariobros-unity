using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Interactable))]
public class MarioInteractableController : MonoBehaviour {
    [SerializeField] private InteractDirection direction;
    [SerializeField] private UnityEvent[] interactEvent;

    private Interactable thisInteractable;


    private void OnInteract(Interactor interactor) {
        ColliderDetector2D colliderDetector = interactor.GetComponent<ColliderDetector2D>();
        if(colliderDetector) {
            if(direction == InteractDirection.FromBelow && !colliderDetector.IsColliding(Direction.Up)) return;
            if(direction == InteractDirection.FromAbove && !colliderDetector.IsColliding(Direction.Down)) return;
            if(direction == InteractDirection.Pickup && !colliderDetector.IsColliding(Direction.Left)) return;
            if(direction == InteractDirection.Pickup && !colliderDetector.IsColliding(Direction.Right)) return;
        }

        Mario2DController mario = interactor.GetComponent<Mario2DController>();
        if(mario) {
            if(mario.StateIndex >= interactEvent.Length - 1) {
                interactEvent[mario.StateIndex].Invoke();
            } 
        }
    }

    private void Awake() {
        thisInteractable = GetComponent<Interactable>();
    }

    private void Start() {
        thisInteractable.OnInteract += OnInteract;
    }


    public enum InteractDirection {
        FromBelow,
        FromAbove,
        Pickup
    }
}
