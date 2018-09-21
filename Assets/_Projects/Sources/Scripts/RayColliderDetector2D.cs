using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;


public class RayColliderDetector2D : ColliderDetector2D {
    public RayInfo UpRay { get { return upRay; } }
    public RayInfo DownRay { get { return downRay; } }
    public RayInfo LeftRay { get { return leftRay; } }
    public RayInfo RightRay { get { return rightRay; } }
    public RayInfo AnyRay { get { return anyRay; } }

    [SerializeField] private RayInfo upRay;
    [SerializeField] private RayInfo downRay;
    [SerializeField] private RayInfo leftRay;
    [SerializeField] private RayInfo rightRay;
    [SerializeField] private RayInfo anyRay;

    private int directionCount;


    public override bool IsColliding(Direction direction, Collider2D collider = null) {
        RayInfo rayInfo = GetRay(direction);
        if(collider) return rayInfo.Colliders.FirstOrDefault(c => c == collider);
        else return rayInfo.Colliders.Count != 0;
    }

    protected override void UpdateDetector() {
        for(int i = 0; i < directionCount; i++) {
            Direction direction = (Direction)i;
            RayInfo rayInfo = MakeRay(thisReferenceCollider, direction);
            if(!rayInfo.IsEnable) continue;

            // Raycast
            RaycastHit2D[] rHits = Physics2D.RaycastAll(rayInfo.Origin, rayInfo.Direction, rayInfo.Distance);

            // Assign and filter colliders
            rayInfo.OldColliders.Clear();
            rayInfo.OldColliders.AddRange(rayInfo.Colliders);

            rayInfo.Colliders.Clear();
            foreach(var rHit in rHits) {
                if(toIgnore.Contains(rHit.collider)) continue;
                rayInfo.Colliders.Add(rHit.collider);
            }

            // Call events
            foreach(var collider in rayInfo.OldColliders) {
                if(!rayInfo.Colliders.Contains(collider)) _OnColliderExit(direction, collider);
            }

            foreach(var collider in rayInfo.Colliders) {
                if(rayInfo.OldColliders.Contains(collider)) _OnColliderStay(direction, collider);
                else _OnColliderEnter(direction, collider);
            }
        }
    }

    public RayInfo GetRay(Direction direction) {
        switch(direction) {
            case Direction.Up: return upRay;
            case Direction.Down: return downRay;
            case Direction.Left: return leftRay;
            case Direction.Right: return rightRay;
            case Direction.Any: return anyRay;
            default: return null;
        }
    }

    private RayInfo MakeRay(Collider2D collider, Direction direction) {
        switch(direction) {
            case Direction.Up:
                upRay.Origin = new Vector2(collider.bounds.center.x * Mathf.Sign(collider.transform.localScale.x), collider.bounds.max.y) + upRay.Offset;
                upRay.Direction = Vector2.up;
                return upRay;
            case Direction.Down:
                downRay.Origin = new Vector2(collider.bounds.center.x * Mathf.Sign(collider.transform.localScale.x), collider.bounds.min.y) + downRay.Offset;
                downRay.Direction = Vector2.down;
                return downRay;
            case Direction.Left:
                leftRay.Origin = new Vector2(collider.bounds.min.x, collider.bounds.center.y * Mathf.Sign(collider.transform.localScale.y)) + leftRay.Offset;
                leftRay.Direction = Vector2.left;
                return leftRay;
            case Direction.Right:
                rightRay.Origin = new Vector2(collider.bounds.max.x, collider.bounds.center.y * Mathf.Sign(collider.transform.localScale.y)) + rightRay.Offset;
                rightRay.Direction = Vector2.right;
                return rightRay;
            case Direction.Any:
                anyRay.Origin = collider.bounds.center + new Vector3(anyRay.Offset.x, anyRay.Offset.y, 0f);
                anyRay.Direction = Vector2.zero;
                return anyRay;
            default:
                return new RayInfo();
        }
    }

    private void Start() {
        directionCount = Enum.GetNames(typeof(Direction)).Length;
    }

    private void OnDrawGizmos() {
        Collider2D rayCol = thisReferenceCollider;
        RayInfo rayInfo = new RayInfo();

        if(rayCol) {
            Gizmos.color = Color.red;

            for(int i = 0; i < Enum.GetNames(typeof(Direction)).Length; i++) {
                Direction direction = (Direction)i;
                rayInfo = MakeRay(rayCol, direction);
                if(!rayInfo.IsEnable) continue;

                Gizmos.DrawRay(rayInfo.Origin, rayInfo.Direction * rayInfo.Distance);
            }
        }
    }

    [Serializable]
    public class RayInfo {
        [HideInInspector] public Vector2 Origin;
        [HideInInspector] public Vector2 Direction;
        [HideInInspector] public List<Collider2D> OldColliders = new List<Collider2D>();
        public List<Collider2D> Colliders = new List<Collider2D>();
        public Vector2 Offset;
        public float Distance = 1f;
        public bool IsEnable = true;
    }
}
