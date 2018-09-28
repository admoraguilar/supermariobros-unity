using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;


[ExecuteInEditMode()]
public abstract class Collider2DDetector : MonoBehaviour {
    public event Action<Collider2D> OnColliderEnter = delegate { };
    public event Action<Collider2D> OnColliderStay = delegate { };
    public event Action<Collider2D> OnColliderExit = delegate { };

    protected void RaiseOnColliderEnter(Collider2D collider2D) { OnColliderEnter(collider2D); }
    protected void RaiseOnColliderStay(Collider2D collider2D) { OnColliderStay(collider2D); }
    protected void RaiseOnColliderExit(Collider2D collider2D) { OnColliderExit(collider2D); }

    public IList<Collider2D> ToIgnores { get { return toIgnores; } }

    [SerializeField] protected List<Collider2D> toIgnores = new List<Collider2D>();
    [SerializeField] protected List<Collider2D> hitColliders = new List<Collider2D>();

    [Header("References")]
    [SerializeField] protected Transform thisTransform;


    protected abstract void UpdateDetector();

    public bool IsColliding(bool noTriggers = false, Collider2D collider = null) {
        IEnumerable<Collider2D> colliders = noTriggers ? hitColliders.Where(c => c.isTrigger == false)
                                                       : hitColliders;
        return collider ? colliders.Contains(collider) : colliders.Count() != 0;
    }

    public IEnumerable<Collider2D> GetDetectedColliders() {
        return hitColliders;
    }

    public void Init(Collider2D[] toIgnores) {
        this.toIgnores.Clear();
        this.toIgnores.AddRange(toIgnores);
    }

    private void Update() {
        for(int i = 0; i < hitColliders.Count; i++) {
            OnColliderStay(hitColliders[i]);
        }

        UpdateDetector();
    }

    private void Reset() {
        thisTransform = GetComponent<Transform>();
    }
}
