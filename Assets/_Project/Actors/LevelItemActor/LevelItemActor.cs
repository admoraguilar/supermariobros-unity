using UnityEngine;
using System;
using WishfulDroplet;
using WishfulDroplet.Extensions;


public class LevelItemActor : Actor<LevelItemActor, LevelItemActor.LevelItemBrain> {
    [InspectorNote("Level Item Actor")]
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


    public abstract class LevelItemBrain : ActorBrain<LevelItemActor> {

    }
}