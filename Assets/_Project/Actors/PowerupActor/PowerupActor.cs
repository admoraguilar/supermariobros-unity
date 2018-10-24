using UnityEngine;
using System;
using WishfulDroplet;
using WishfulDroplet.Extensions;


public class PowerupActor : Actor<PowerupActor, PowerupActor.PowerupBrain> {
    public Rigidbody2D thisRigidbody2D {
        get { return _thisRigidbody2D; }
        private set { _thisRigidbody2D = value; }
    }

    public BoxCollider2D thisCollisionCollider2D {
        get { return _thisCollisionCollider2D; }
        private set { _thisCollisionCollider2D = value; }
    }

    public BoxCollider2D thisInteractionCollider2D {
        get { return _thisInteractionCollider2D; }
        private set { _thisInteractionCollider2D = value; }
    }

    [InspectorNote("Powerup Actor")]
    [Header("Data")]
    public _InternalActorBrain[] buffableBrains;

    [Header("References")]
    [SerializeField] private Rigidbody2D _thisRigidbody2D;
    [SerializeField] private BoxCollider2D _thisCollisionCollider2D;
    [SerializeField] private BoxCollider2D _thisInteractionCollider2D;


    public bool IsBrainBuffable(_InternalActorBrain brain) {
        return IsBrainOnSet(buffableBrains, brain);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(brain) {
            brain.DoTriggerEnter2D(this, collision);
        }
    }

    protected override void Reset() {
        base.Reset();

        _thisRigidbody2D = gameObject.AddOrGetComponent<Rigidbody2D>();

        _thisCollisionCollider2D = gameObject.AddOrGetComponent<BoxCollider2D>();

        _thisInteractionCollider2D = Utilities.CreateObject("Interaction", thisTransform).AddOrGetComponent<BoxCollider2D>();
        _thisInteractionCollider2D.isTrigger = true;
    }


    public abstract class PowerupBrain : ActorBrain<PowerupActor> {

    }
}