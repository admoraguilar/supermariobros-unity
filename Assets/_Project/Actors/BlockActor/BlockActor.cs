using UnityEngine;
using WishfulDroplet;
using WishfulDroplet.Components;
using WishfulDroplet.Extensions;


public class BlockActor : Actor<BlockActor, BlockActor.BlockBrain> {
    [InspectorNote("Block Actor")]
    [Header("Data")]
    public ScriptableActorBrain[]					interactorBrains;
    public GameObject								content;
    public Sprite									filledSprite;
    public Sprite									emptySprite;
    public int										contentCount;
    public AudioClip								hitSound;
    public AudioClip								contentAppearSound;

	[Header("References")]
	[SerializeField] private Interactable			_thisInteractable;
    [SerializeField] private Animator				_thisAnimator;
    [SerializeField] private SpriteRenderer			_thisSpriteRenderer;
    [SerializeField] private BoxCollider2D			_thisCollisionCollider;
    [SerializeField] private BoxCollider2D			_thisInteractionCollider;

	public Interactable								thisInteractable {
		get { return _thisInteractable; }
		private set { _thisInteractable = value; }
	}

	public Animator									thisAnimator {
		get { return _thisAnimator; }
		private set { _thisAnimator = value; }
	}

	public SpriteRenderer							thisSpriteRenderer {
		get { return _thisSpriteRenderer; }
		private set { _thisSpriteRenderer = value; }
	}

	public BoxCollider2D							thisCollisionCollider {
		get { return _thisCollisionCollider; }
		private set { _thisCollisionCollider = value; }
	}

	public BoxCollider2D							thisInteractionCollider {
		get { return _thisInteractionCollider; }
		private set { _thisInteractionCollider = value; }
	}


	public bool IsBrainInteractor(ScriptableActorBrain brain) {
        return IsBrainOnSet(interactorBrains, brain);
    }

	private bool OnInteracted(Direction direction, GameObject interactor) {
		if(brain) {
			brain.DoInteracted(this, direction, interactor);
		}

		return false;
	}

	private void OnEnable() {
		if(brain) {
			brain.DoOnEnable(this);
		}

		thisInteractable.OnInteract += OnInteracted;
	}

	private void OnDisable() {
		if(brain) {
			brain.DoOnDisable(this);
		}

		thisInteractable.OnInteract -= OnInteracted;
	}

    protected override void Reset() {
        base.Reset();

		// Set animator
        thisAnimator = gameObject.GetComponentInChildren<Animator>(true);

		// Set sprite renderer
        thisSpriteRenderer = _thisAnimator.GetComponentInChildren<SpriteRenderer>(true);

		// Set collision collider
		thisCollisionCollider = Utilities.CreateOrGetObject("Collision", thisTransform).AddOrGetComponent<BoxCollider2D>();
		thisCollisionCollider.size = thisSpriteRenderer ? thisSpriteRenderer.size * thisSpriteRenderer.GetComponent<Transform>().localScale :
															thisCollisionCollider.size;

		// Set interaction collider
		thisInteractionCollider = Utilities.CreateOrGetObject("Interaction", thisTransform).AddOrGetComponent<BoxCollider2D>();
        thisInteractionCollider.isTrigger = true;
    }


    public class BlockBrain : ActorBrain<BlockActor> {
		public virtual bool DoInteracted(BlockActor blockActor, Direction direction, GameObject interactor) { return false; }
	}
}
