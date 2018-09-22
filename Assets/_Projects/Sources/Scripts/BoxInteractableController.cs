using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Interactable))]
public class BoxInteractableController : MonoBehaviour {
    [Header("Controller")]
    [SerializeField] private Type type;
    [SerializeField] private Content content;
    [SerializeField] private InteractDirection interactDirection;

    [Header("Animators")]
    [SerializeField] private Animator boxAnimator;
    [SerializeField] private Animator boxContentAnimator;
    [SerializeField] private ParticleSystem particleObject;

    [Header("Audio")]
    [SerializeField] private AudioClip boxHitSound;
    [SerializeField] private AudioClip boxContentAppearSound;

    [Header("Sprites")]
    [SerializeField] private Sprite boxFilled;
    [SerializeField] private Sprite boxEmpty;

    [Header("References")]
    [SerializeField] private SpriteRenderer boxSprite;

    [Header("Debug")]
    [SerializeField] private Collider2D[] colliders;

    private Interactable thisInteractable;
    private AudioController thisAudioController;


    private void OnInteract(Interactor interactor) {
        BoxColliderDetector2D colliderDetector = interactor.GetComponent<BoxColliderDetector2D>();
        if(colliderDetector) {
            if(interactDirection == InteractDirection.FromBelow && !colliderDetector.IsColliding(Direction.Up)) return;
            if(interactDirection == InteractDirection.FromAbove && !colliderDetector.IsColliding(Direction.Down)) return;
            if(interactDirection == InteractDirection.Pickup && !colliderDetector.IsColliding(Direction.Left)) return;
            if(interactDirection == InteractDirection.Pickup && !colliderDetector.IsColliding(Direction.Right)) return;
        }

        Mario2DController mario = interactor.GetComponent<Mario2DController>();
        if(mario) {
            ProcessBox(mario);
        }

        thisAudioController.PlayOneShot(boxHitSound);
    }

    private IEnumerator DisableCollision(Mario2DController mario) {
        BoxColliderDetector2D ch = mario.GetComponent<BoxColliderDetector2D>();

        while(ch.IsColliding(Direction.Up)) {
            yield return null;
        }

        foreach(var collider in colliders) {
            collider.enabled = false;
        }

        while(!particleObject.isStopped) {
            yield return null;
        }

        gameObject.SetActive(false);
    }

    private void ProcessBox(Mario2DController mario) {
        switch(type) {
            case Type.Empty:
                switch(mario.StateIndex) {
                    case 0:
                        boxAnimator.PlayNoRepeat("Interacted");
                        break;
                    case 1:
                    case 2:
                        //boxAnimator.PlayNoRepeat("Destroy");
                        boxAnimator.gameObject.SetActive(false);
                        boxContentAnimator.gameObject.SetActive(false);
                        particleObject.gameObject.SetActive(true);
                        StartCoroutine(DisableCollision(mario));
                        thisAudioController.PlayOneShot(boxContentAppearSound);
                        break;
                }
                break;
            case Type.Filled:
                boxAnimator.PlayNoRepeat("Interacted");
                boxContentAnimator.PlayNoRepeat("Interacted");
                boxSprite.sprite = boxEmpty;
                thisAudioController.PlayOneShot(boxContentAppearSound);
                break;
        }

        switch(content) {
            case Content.Empty:

                break;
            case Content.Coin:
                // Add score

                break;
            case Content.Powerup:
                // Spawn powerup
                break;
        }
    }

    private void Awake() {
        thisInteractable = GetComponent<Interactable>();
        thisAudioController = SingletonController.Get<AudioController>();
        colliders = GetComponents<Collider2D>();
    }

    private void Start() {
        thisInteractable.OnInteract += OnInteract;
        if(particleObject) particleObject.gameObject.SetActive(false);
    }


    public enum Type {
        Empty,
        Filled
    }

    public enum Content {
        Empty,
        Coin,
        Powerup
    }

    public enum InteractDirection {
        FromBelow,
        FromAbove,
        Pickup
    }
}