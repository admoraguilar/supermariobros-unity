using UnityEngine;
using System;


public class BoxContentInteractableController : MonoBehaviour {
    [Header("Controller")]
    [SerializeField] private Type type;
    [SerializeField] private AudioClip interactSound;

    [Header("References")]
    [SerializeField] private Transform thisTransform;
    [SerializeField] private Animator thisAnimator;
    [SerializeField] private SpriteRenderer thisSpriteRenderer;
    [SerializeField] private BoxCollider2D thisBoxCollider2D;
    [SerializeField] private Interactable thisInteractable;

    private IAudioController thisAudioController;


    private void OnInteract(Interactor interactor) {
        BoxInteractableController box = interactor.GetComponent<BoxInteractableController>();
        if(box) {
            ProcessContent(box);    
        }

        Mario2DController mario = interactor.GetComponent<Mario2DController>();
        if(mario) {
            ProcessContent(mario);
        }
    }

    private void ProcessContent(BoxInteractableController box) {
        thisAnimator.PlayNoRepeat("Interacted");
    }

    private void ProcessContent(Mario2DController mario) {
        switch(type) {
            case Type.Mushroom:
                mario.PowerUp(true);
                break;
            case Type.Flower:
                mario.PowerUp(true);
                break;
            case Type.Mushroom_Poison:
                mario.PowerUp(false);
                break;
            case Type.KillZone:
                mario.Kill(true);
                break;
        }

        if(interactSound) {
            thisAudioController.PlayOneShot(interactSound);
        }
    }

    private void Awake() {
        thisAudioController = Singleton.Get<IAudioController>();    
    }

    private void Start() {
        thisInteractable.OnInteract += OnInteract;
    }

    private void Reset() {
        thisTransform = GetComponent<Transform>();

        /*
         * Setup animator
         */
        thisAnimator = this.AddComponentAsChildObject<Animator>(thisTransform, "Content");

        /*
         * Setup sprite
         */
        thisSpriteRenderer = this.AddComponentAsChildObject<SpriteRenderer>(thisTransform, "Content/Sprite");

        /*
         * Setup collider
         */
        thisBoxCollider2D = this.AddOrGetComponent<BoxCollider2D>();
        thisBoxCollider2D.size = thisSpriteRenderer.size;

        thisInteractable = this.AddOrGetComponent<Interactable>();
    }


    public enum Type {
        Coin,
        Flower,
        Mushroom,
        Flag,
        Star,
        Mushroom_1UP,
        Mushroom_Poison,
        KillZone
    }
}
