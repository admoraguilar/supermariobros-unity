using UnityEngine;
using WishfulDroplet;
using WishfulDroplet.Components;
using WishfulDroplet.Extensions;


public class BlockActor : Actor<BlockActor, BlockActor.BlockBrain> {
	public Interactable thisInteractable {
		get { return _thisInteractable; }
		private set { _thisInteractable = value; }
	}

	public Animator thisAnimator {
        get { return _thisAnimator; }
        private set { _thisAnimator = value; }
    }

    public BoxCollider2D thisCollisionCollider2D {
        get { return _thisCollisionCollider2D; }
        private set { _thisCollisionCollider2D = value; }
    }

    public BoxCollider2D thisInteractionCollider2D {
        get { return _thisInteractionCollider2D; }
        private set { _thisInteractionCollider2D = value; }
    }

	public CollisionEvents2D thisInteractionColliderEvents {
		get { return _thisInteractionColliderEvents; }
		private set { _thisInteractionColliderEvents = value; }
	}

    [InspectorNote("Block Actor")]
    [Header("Data")]
    public _InternalActorBrain[] interactorBrains;
    public GameObject content;
    public Sprite filledSprite;
    public Sprite emptySprite;
    public int contentCount;
    public AudioClip hitSound;
    public AudioClip contentAppearSound;

	[Header("References")]
	[SerializeField] private Interactable _thisInteractable;
    [SerializeField] private Animator _thisAnimator;
    [SerializeField] private SpriteRenderer _thisSpriteRenderer;
    [SerializeField] private BoxCollider2D _thisCollisionCollider2D;
    [SerializeField] private BoxCollider2D _thisInteractionCollider2D;
	[SerializeField] private CollisionEvents2D _thisInteractionColliderEvents;


    public bool IsBrainInteractor(_InternalActorBrain brain) {
        return IsBrainOnSet(interactorBrains, brain);
    }

	public void Interact() {
		thisAnimator.PlayNoRepeat("Interacted");

		Singleton.Get<IAudioController>().PlayOneShot(hitSound);
	}

	public void Destroy() {
		ActionTemplates.RunActionAfterSeconds("BlockBrain_DelayedDisable", .05f, () => { gameObject.SetActive(false); });

		if(content) {
			Instantiate(content, thisTransform.position, thisTransform.rotation);
		}
		Singleton.Get<IAudioController>().PlayOneShot(contentAppearSound);
	}

	private bool OnInteract(GameObject interactor) {
		if(brain) {
			brain.DoInteract(this, interactor);
		}

		return false;
	}

	private void OnEnable() {
		thisInteractable.OnInteract += OnInteract;

		//thisInteractionColliderEvents.OnCollisionEnter2DCallback += _OnCollisionEnter2D;
		//thisInteractionColliderEvents.OnCollisionStay2DCallback += _OnCollisionStay2D;
		//thisInteractionColliderEvents.OnCollisionExit2DCallback += _OnCollisionExit2D;
		thisInteractionColliderEvents.OnTriggerEnter2DCallback += _OnTriggerEnter2D;
		//thisInteractionColliderEvents.OnTriggerStay2DCallback += _OnTriggerStay2D;
		//thisInteractionColliderEvents.OnTriggerExit2DCallback += _OnTriggerExit2D;
	}

	private void OnDisable() {
		thisInteractable.OnInteract -= OnInteract;

		//thisInteractionColliderEvents.OnCollisionEnter2DCallback -= _OnCollisionEnter2D;
		//thisInteractionColliderEvents.OnCollisionStay2DCallback -= _OnCollisionStay2D;
		//thisInteractionColliderEvents.OnCollisionExit2DCallback -= _OnCollisionExit2D;
		thisInteractionColliderEvents.OnTriggerEnter2DCallback -= _OnTriggerEnter2D;
		//thisInteractionColliderEvents.OnTriggerStay2DCallback -= _OnTriggerStay2D;
		//thisInteractionColliderEvents.OnTriggerExit2DCallback -= _OnTriggerExit2D;
	}

	private void _OnTriggerEnter2D(Collider2D collision) {
        if(brain) {
            brain.DoTriggerEnter2D(this, collision);
        }
    }

    protected override void Reset() {
        base.Reset();

        _thisAnimator = gameObject.GetComponentInChildren<Animator>(true);

        _thisSpriteRenderer = _thisAnimator.GetComponentInChildren<SpriteRenderer>(true);

        _thisCollisionCollider2D = gameObject.AddOrGetComponent<BoxCollider2D>();

        _thisInteractionCollider2D = Utilities.CreateObject("Interaction", thisTransform).AddOrGetComponent<BoxCollider2D>();
        _thisInteractionCollider2D.isTrigger = true;
    }


    public class BlockBrain : ActorBrain<BlockActor> {
		public virtual bool DoInteract(BlockActor blockActor, GameObject interactor) { return false; }
	}
}
