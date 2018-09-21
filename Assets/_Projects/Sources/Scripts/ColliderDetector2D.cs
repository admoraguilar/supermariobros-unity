using UnityEngine;
using System;
using System.Collections.Generic;


public abstract class ColliderDetector2D : MonoBehaviour {
    public event Action<Direction, Collider2D> OnColliderEnter = delegate { };
    public event Action<Direction, Collider2D> OnColliderStay = delegate { };
    public event Action<Direction, Collider2D> OnColliderExit = delegate { };

    protected void _OnColliderEnter(Direction direction, Collider2D collider) {
        if(OnColliderEnter != null) OnColliderEnter(direction, collider);
    }
    protected void _OnColliderStay(Direction direction, Collider2D collider) {
        if(OnColliderEnter != null) OnColliderStay(direction, collider);
    }
    protected void _OnColliderExit(Direction direction, Collider2D collider) {
        if(OnColliderEnter != null) OnColliderExit(direction, collider);
    }


    public Collider2D ReferenceCollider { get { return thisReferenceCollider; } }

    [SerializeField] protected List<Collider2D> toIgnore = new List<Collider2D>();

    [SerializeField] protected Collider2D thisReferenceCollider;


    public abstract bool IsColliding(Direction direction, Collider2D collider = null);
    protected abstract void UpdateDetector();

    public void SetReferenceCollider(Collider2D collider) {
        thisReferenceCollider = collider;
    }

    protected virtual void Awake() {
        if(thisReferenceCollider == null) thisReferenceCollider = GetComponent<Collider2D>();
    }

    protected virtual void Update() {
        UpdateDetector();
    }

    protected virtual void OnValidate() {
        if(thisReferenceCollider == null) thisReferenceCollider = GetComponent<Collider2D>();
    }
}
