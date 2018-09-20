using UnityEngine;
using System;


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


    public override Collider2D IsColliding(Direction direction) {
        switch(direction) {
            case Direction.Up: return upRay.Collider;
            case Direction.Down: return downRay.Collider;
            case Direction.Left: return leftRay.Collider;
            case Direction.Right: return rightRay.Collider;
            case Direction.Any: return anyRay.Collider;
            default: return null;
        }
    }

    protected override void UpdateDetector() {
        for(int i = 0; i < Enum.GetNames(typeof(Direction)).Length; i++) {
            Direction direction = (Direction)i;
            RayInfo rayInfo = MakeRay(thisReferenceCollider, direction);
            if(!rayInfo.IsEnable) continue;

            Collider2D oldCollider = rayInfo.Collider;

            RaycastHit2D hit = Physics2D.Raycast(rayInfo.Origin, rayInfo.Direction, rayInfo.Distance);
            rayInfo.Collider = hit.collider;

            if(oldCollider != rayInfo.Collider) {
                if(oldCollider) _OnColliderExit(direction, oldCollider);
                if(rayInfo.Collider) _OnColliderEnter(direction, rayInfo.Collider);
            } else {
                if(rayInfo.Collider) _OnColliderStay(direction, rayInfo.Collider);
            }
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
        public Vector2 Offset;
        public Collider2D Collider;
        public float Distance = 1f;
        public bool IsEnable = true;
    }
}
