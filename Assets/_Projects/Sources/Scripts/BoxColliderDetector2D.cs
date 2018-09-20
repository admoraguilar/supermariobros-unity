using UnityEngine;
using System;


public class BoxColliderDetector2D : ColliderDetector2D {
    public BoxInfo UpBox { get { return upBox; } }
    public BoxInfo DownBox { get { return downBox; } }
    public BoxInfo LeftBox { get { return leftBox; } }
    public BoxInfo RightBox { get { return rightBox; } }
    public BoxInfo AnyBox { get { return anyBox; } }

    [SerializeField] private BoxInfo upBox;
    [SerializeField] private BoxInfo downBox;
    [SerializeField] private BoxInfo leftBox;
    [SerializeField] private BoxInfo rightBox;
    [SerializeField] private BoxInfo anyBox;


    public override Collider2D IsColliding(Direction direction) {
        switch(direction) {
            case Direction.Up: return upBox.Collider;
            case Direction.Down: return downBox.Collider;
            case Direction.Left: return leftBox.Collider;
            case Direction.Right: return rightBox.Collider;
            case Direction.Any: return anyBox.Collider;
            default: return null;
        }
    }

    protected override void UpdateDetector() {
        for(int i = 0; i < Enum.GetNames(typeof(Direction)).Length; i++) {
            Direction direction = (Direction)i;
            BoxInfo boxInfo = MakeBox(thisReferenceCollider, direction);
            if(!boxInfo.IsEnable) continue;

            Collider2D oldCollider = boxInfo.Collider;

            if(direction == Direction.Any) {
                Collider2D hit = Physics2D.OverlapBox(boxInfo.Center, boxInfo.Size, 0);
                boxInfo.Collider = hit;
            } else {
                RaycastHit2D hit = Physics2D.BoxCast(boxInfo.Center, boxInfo.Size, 0f, boxInfo.Direction, boxInfo.Distance);
                boxInfo.Collider = hit.collider;
            }

            if(oldCollider != boxInfo.Collider) {
                if(oldCollider) _OnColliderExit(direction, oldCollider);
                if(boxInfo.Collider) _OnColliderEnter(direction, boxInfo.Collider);
            } else {
                if(boxInfo.Collider) _OnColliderStay(direction, boxInfo.Collider);
            }
        }
    }

    private BoxInfo MakeBox(Collider2D collider, Direction direction) {
        switch(direction) {
            case Direction.Up:
                upBox.Center = new Vector2(collider.bounds.center.x, collider.bounds.max.y);
                upBox.Size = new Vector2(collider.bounds.size.x * upBox.SizeMultiplier, collider.bounds.size.y * .2f);
                upBox.Direction = Vector2.up;
                return upBox;
            case Direction.Down:
                downBox.Center = new Vector2(collider.bounds.center.x, collider.bounds.min.y);
                downBox.Size = new Vector2(collider.bounds.size.x * downBox.SizeMultiplier, collider.bounds.size.y * .2f);
                downBox.Direction = Vector2.down;
                return downBox;
            case Direction.Left:
                leftBox.Center = new Vector2(collider.bounds.min.x, collider.bounds.center.y);
                leftBox.Size = new Vector2(collider.bounds.size.x * .2f, collider.bounds.size.y * leftBox.SizeMultiplier);
                leftBox.Direction = Vector2.left;
                return leftBox;
            case Direction.Right:
                rightBox.Center = new Vector2(collider.bounds.max.x, collider.bounds.center.y);
                rightBox.Size = new Vector2(collider.bounds.size.x * .2f, collider.bounds.size.y * rightBox.SizeMultiplier);
                rightBox.Direction = Vector2.right;
                return rightBox;
            case Direction.Any:
                anyBox.Center = collider.bounds.center;
                anyBox.Size = collider.bounds.size * anyBox.SizeMultiplier;
                anyBox.Direction = Vector2.zero;
                return anyBox;
            default:
                return new BoxInfo();
        }
    }

    private void OnDrawGizmos() {
        Collider2D boxCol = thisReferenceCollider;
        BoxInfo boxInfo = new BoxInfo();

        if(boxCol) {
            Gizmos.color = Color.red;

            for(int i = 0; i < Enum.GetNames(typeof(Direction)).Length; i++) {
                Direction direction = (Direction)i;
                boxInfo = MakeBox(boxCol, direction);
                if(!boxInfo.IsEnable) continue;

                Gizmos.DrawWireCube(boxInfo.Center, boxInfo.Size);
            }
        }
    }

    [Serializable]
    public class BoxInfo {
        [HideInInspector] public Vector2 Center;
        [HideInInspector] public Vector2 Size;
        [HideInInspector] public Vector2 Direction;
        public Collider2D Collider;
        public float SizeMultiplier = 1f;
        public float Distance = 1f;
        public bool IsEnable = true;
    }
}
