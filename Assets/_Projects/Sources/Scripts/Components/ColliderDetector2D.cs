using UnityEngine;
using System;
using System.Collections.Generic;


[ExecuteInEditMode()]
public abstract class ColliderDetector2D : MonoBehaviour {
    public event Action<Direction, Collider2D> OnColliderEnter = delegate { };
    public event Action<Direction, Collider2D> OnColliderStay = delegate { };
    public event Action<Direction, Collider2D> OnColliderExit = delegate { };

    protected void _OnColliderEnter(Direction direction, Collider2D collider) {
        if(OnColliderEnter != null) {
            OnColliderEnter(direction, collider);
        }
    }
    protected void _OnColliderStay(Direction direction, Collider2D collider) {
        if(OnColliderEnter != null) {
            OnColliderStay(direction, collider);
        }
    }
    protected void _OnColliderExit(Direction direction, Collider2D collider) {
        if(OnColliderEnter != null) {
            OnColliderExit(direction, collider);
        }
    }

    public IList<Collider2D>                                    ToIgnores { get { return toIgnores; } }

    [SerializeField]  protected List<Collider2D>                toIgnores = new List<Collider2D>();


    /// <summary>
    /// Use to query if a non-trigger or trigger collider is within bounds.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="collider"></param>
    /// <returns></returns>
    public abstract bool                        IsColliderWithinBounds(Direction direction, Collider2D collider = null);

    /// <summary>
    /// Use to query if a non-trigger collider is within bounds.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="collider"></param>
    /// <returns></returns>
    public abstract bool                        IsColliding(Direction direction, Collider2D collider = null);
    public      abstract Collider2D[]           GetDetectedColliders(Direction direction);
    protected   abstract void                   UpdateDetector();

    public void Init(Collider2D[] toIgnores) {
        this.toIgnores.Clear();
        this.toIgnores.AddRange(toIgnores);
    }

    protected virtual void Update() {
        UpdateDetector();
    }

    public enum Direction {
        Up,
        Down,
        Left,
        Right,
        Any
    }
}
