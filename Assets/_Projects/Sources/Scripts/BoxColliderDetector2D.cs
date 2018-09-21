using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;


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

    private int directionCount;


    public override bool IsColliding(Direction direction, Collider2D collider = null) {
        BoxInfo boxInfo = GetBox(direction);
        if(collider) return boxInfo.Colliders.FirstOrDefault(c => c == collider);
        else return boxInfo.Colliders.Count != 0;
    }

    protected override void UpdateDetector() {
        for(int i = 0; i < directionCount; i++) {
            Direction direction = (Direction)i;
            BoxInfo boxInfo = MakeBox(thisReferenceCollider, direction);
            if(!boxInfo.IsEnable) continue;

            // Assign and filter list
            boxInfo.OldColliders.Clear();
            boxInfo.OldColliders.AddRange(boxInfo.Colliders);

            boxInfo.Colliders.Clear();
            if(direction == Direction.Any) {
                foreach(var hit in Physics2D.OverlapBoxAll(boxInfo.Center, boxInfo.Size, 0f)) {
                    if(toIgnore.Contains(hit)) continue;
                    boxInfo.Colliders.Add(hit);
                }
            } else {
                RaycastHit2D[] rHits = Physics2D.BoxCastAll(boxInfo.Center, boxInfo.Size, 0f, boxInfo.Direction, boxInfo.Distance);
                foreach(var rHit in rHits) {
                    if(toIgnore.Contains(rHit.collider)) continue;
                    boxInfo.Colliders.Add(rHit.collider);
                }
            }
            

            // Call events
            foreach(var collider in boxInfo.OldColliders) {
                if(!boxInfo.Colliders.Contains(collider)) _OnColliderExit(direction, collider);
            }

            foreach(var collider in boxInfo.Colliders) {
                if(boxInfo.OldColliders.Contains(collider)) _OnColliderStay(direction, collider);
                else _OnColliderEnter(direction, collider);
            }
        }
    }

    public BoxInfo GetBox(Direction direction) {
        switch(direction) {
            case Direction.Up: return upBox;
            case Direction.Down: return downBox;
            case Direction.Left: return leftBox;
            case Direction.Right: return rightBox;
            case Direction.Any: return anyBox;
            default: return null;
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

    private void Start() {
        directionCount = Enum.GetNames(typeof(Direction)).Length;
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
        [HideInInspector] public List<Collider2D> OldColliders = new List<Collider2D>();
        public List<Collider2D> Colliders = new List<Collider2D>();
        public float SizeMultiplier = 1f;
        public float Distance = 1f;
        public bool IsEnable = true;
    }
}
