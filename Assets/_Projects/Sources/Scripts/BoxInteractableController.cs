using UnityEngine;


[RequireComponent(typeof(Interactable)), RequireComponent(typeof(BoxColliderDetector2D))]
public class BoxInteractableController : MonoBehaviour {
    [Header("Controller")]
    [SerializeField] private Type type;
    [SerializeField] private Content content;

    [Header("Animators")]
    [SerializeField] private Animator boxAnimator;
    [SerializeField] private Animator coinBoxAnimator;

    [Header("Audio")]
    [SerializeField] private AudioClip boxHitSound;
    [SerializeField] private AudioClip coinPickupSound;

    [Header("Sprites")]
    [SerializeField] private SpriteRenderer boxSprite;
    [SerializeField] private Sprite boxPowerFilled;
    [SerializeField] private Sprite boxPowerEmpty;

    [Header("References")]
    private Interactable thisInteractable;
    private AudioController thisAudioController;
    private BoxColliderDetector2D thisBoxColliderDetector2D;


    private void OnInteract(Interactor interactor) {
        if(!thisBoxColliderDetector2D.IsColliding(Direction.Down)) return;

        Mario2DController mario = interactor.GetComponent<Mario2DController>();
        if(mario) {
            switch(mario.StateIndex) {
                case 0:
                    boxAnimator.PlayNoRepeat("Interacted");
                    boxSprite.sprite = boxPowerFilled;
                    break;
                case 1:
                    boxAnimator.PlayNoRepeat("Interacted");
                    coinBoxAnimator.PlayNoRepeat("Interacted");
                    boxSprite.sprite = boxPowerEmpty;
                    thisAudioController.PlayOneShot(coinPickupSound);
                    break;
            }
        }

        thisAudioController.PlayOneShot(boxHitSound);
    }

    private void Awake() {
        thisInteractable = GetComponent<Interactable>();
        thisAudioController = SingletonController.Get<AudioController>();
        thisBoxColliderDetector2D = GetComponent<BoxColliderDetector2D>();
    }

    private void Start() {
        thisInteractable.OnInteract += OnInteract;
    }


    public enum Type {
        QuestionMark,
        Brick,
        Invisible
    }

    public enum Content {
        Powerup,
        Coin
    }

    public enum InteractResponse {
        Default,
        Weak
    }
}