using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Interactable))]
public class BoxInteractableController : MonoBehaviour {
    [Header("Controller")]
    [SerializeField] private Type type;
    [SerializeField] private InteractDirection interactDirection;
    [SerializeField] private GameObject content;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip contentAppearSound;
    [SerializeField] private Sprite filledSprite;
    [SerializeField] private Sprite emptySprite;

    [Header("References")]
    [SerializeField] private Transform thisTransform;
    [SerializeField] private Animator thisAnimator;
    [SerializeField] private SpriteRenderer thisSpriteRenderer;
    [SerializeField] private BoxCollider2D thisBoxCollider2D;
    [SerializeField] private Interactable thisInteractable;

    private IAudioController thisAudioController;


    private void OnInteract(Interactor interactor) {
        DirectionalCollider2DDetector collider2DDetectorDirectional = interactor.GetComponent<DirectionalCollider2DDetector>();
        if(collider2DDetectorDirectional) {
            if(interactDirection == InteractDirection.FromBelow && 
               !collider2DDetectorDirectional.IsColliding(Direction.Up)) return;
            if(interactDirection == InteractDirection.FromAbove && 
               !collider2DDetectorDirectional.IsColliding(Direction.Down)) return;
        }

        Mario2DController mario = interactor.GetComponent<Mario2DController>();
        if(mario) {
            ProcessBox(mario);
        }
    }

    private void ProcessBox(Mario2DController mario) {
        switch(type) {
            case Type.Empty:
                if(mario.CurrentStateSet.IsCanBreakBrick) {
                    ActionTemplates.RunActionAfterSeconds("DelayedDisable", .05f, () => { gameObject.SetActive(false); });
                    if(content) {
                        Instantiate(content, thisTransform.position, thisTransform.rotation);
                    }
                    thisAudioController.PlayOneShot(contentAppearSound);
                } else {
                    thisAnimator.PlayNoRepeat("Interacted");
                }
                break;
            case Type.Filled:
                thisAnimator.PlayNoRepeat("Interacted");
                thisSpriteRenderer.sprite = emptySprite;
                if(content) {
                    Instantiate(content, thisTransform.position, thisTransform.rotation);
                }
                thisAudioController.PlayOneShot(contentAppearSound);
                break;
        }

        thisAudioController.PlayOneShot(hitSound);
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
        thisAnimator = this.AddComponentAsChildObject<Animator>(thisTransform, "Box");

        /*
         * Setup sprite
         */
        thisSpriteRenderer = this.AddComponentAsChildObject<SpriteRenderer>(thisTransform, "Box/Sprite");

        /*
         * Setup collider
         */
        thisBoxCollider2D = this.AddOrGetComponent<BoxCollider2D>();
        thisBoxCollider2D.size = thisSpriteRenderer.size;

        thisInteractable = this.AddOrGetComponent<Interactable>();
    }


    public enum Type {
        Empty,
        Filled
    }

    public enum InteractDirection {
        FromBelow,
        FromAbove
    }
}
