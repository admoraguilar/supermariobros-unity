using UnityEngine;
using UnityEngine.Events;
using System;
using System.Linq;


public class InteractableController : MonoBehaviour {
    [SerializeField] private OnInteractEvent[] events;

    private Interactable thisInteractable;


    private void OnInteract(Interactor interactor) {
        //if(interactor.Length > 0) {
        //    OnInteractEvent @event = events.FirstOrDefault(e => e.InteractId == interactor);
        //    if(@event != null) {
        //        @event.Event.Invoke();
        //    }
        //} else {
        //    events[0].Event.Invoke();
        //}
    }

    private void Awake() {
        thisInteractable = GetComponent<Interactable>();
    }

    private void Start() {
        thisInteractable.OnInteract += OnInteract;
    }


    [Serializable]
    public class OnInteractEvent {
        public string InteractId;
        public UnityEvent Event;
    }
}
