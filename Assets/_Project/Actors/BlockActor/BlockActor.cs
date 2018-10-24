using UnityEngine;
using WishfulDroplet;
using WishfulDroplet.Extensions;


public class BlockActor : Actor<BlockActor, BlockActor.BlockBrain> {
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

    [InspectorNote("Block Actor")]
    [Header("Data")]
    public _InternalActorBrain[] interactorBrains;
    public AudioClip hitSound;
    public AudioClip breakSound;

    [Header("References")]
    [SerializeField] private Animator _thisAnimator;
    [SerializeField] private BoxCollider2D _thisCollisionCollider2D;
    [SerializeField] private BoxCollider2D _thisInteractionCollider2D;


    public bool IsBrainInteractor(_InternalActorBrain brain) {
        return IsBrainOnSet(interactorBrains, brain);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(brain) {
            brain.DoTriggerEnter2D(this, collision);
        }
    }

    protected override void Reset() {
        base.Reset();

        _thisAnimator = gameObject.GetComponentInChildren<Animator>(true);

        _thisCollisionCollider2D = gameObject.AddOrGetComponent<BoxCollider2D>();

        _thisInteractionCollider2D = Utilities.CreateObject("Interaction", thisTransform).AddOrGetComponent<BoxCollider2D>();
        _thisInteractionCollider2D.isTrigger = true;
    }


    public class BlockBrain : ActorBrain<BlockActor> {

    }
}
