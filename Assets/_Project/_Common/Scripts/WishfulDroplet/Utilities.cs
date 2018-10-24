using UnityEngine;
using System;
using System.Collections.Generic;


namespace WishfulDroplet {
    using Extensions;


    [Flags]
    public enum Direction {
        Up = 0,
        Down = 1, 
        Left = 2,
        Right = 3,
    }


    [Serializable]
    public class DirectionalBoxCast2D {
        public DirectionalBoxCast2D(int maxHitBufferSize = 20) {
            hitBuffer = new RaycastHit2D[maxHitBufferSize];
        }

        [SerializeField] public BoxCastInfo[]              BoxInfos;
        [SerializeField] public BoxCollider2D              ReferenceCollider;

        private RaycastHit2D[]                             hitBuffer;


        public bool IsHit(Collider2D collider = null) {
            bool isHit = false;

            for(int i = 0; i < BoxInfos.Length; i++) {
                if(!collider) {
                    if(BoxInfos[i].Hits.Count != 0) {
                        isHit = true;
                        break;
                    }
                } else {
                    if(BoxInfos[i].Hits.Contains(collider)) {
                        isHit = true;
                        break;
                    }
                }
            }

            return isHit;
        }

        public bool IsHit(Direction direction, Collider2D collider = null) {
            for(int i = 0; i < BoxInfos.Length; i++) {
                if(BoxInfos[i].Direction == direction) {
                    if(!collider) {
                        return BoxInfos[i].Hits.Count != 0;
                    } else {
                        return BoxInfos[i].Hits.Contains(collider);
                    }
                }
            }

            return false;
        }

        public void SetHitBufferSize(int size) {
            hitBuffer = new RaycastHit2D[size];
        }

        public void GetHits(IList<Collider2D> touchColliderMask = null, IList<Collider2D> boxCastMask = null, Action<Direction, RaycastHit2D, Collider2D> onHit = null) {
            for(int i = 0; i < BoxInfos.Length; i++) {
                switch(BoxInfos[i].Direction) {
                    case Direction.Up:
                        BoxInfos[i].origin = new Vector2(ReferenceCollider.bounds.center.x, ReferenceCollider.bounds.max.y);
                        BoxInfos[i].size = new Vector2(ReferenceCollider.size.x, 1f * BoxInfos[i].SizeMultiplier);
                        BoxInfos[i].castDirection = Vector2.up;
                        break;
                    case Direction.Down:
                        BoxInfos[i].origin = new Vector2(ReferenceCollider.bounds.center.x, ReferenceCollider.bounds.min.y);
                        BoxInfos[i].size = new Vector2(ReferenceCollider.size.x, 1f * BoxInfos[i].SizeMultiplier);
                        BoxInfos[i].castDirection = Vector2.down;
                        break;
                    case Direction.Left:
                        BoxInfos[i].origin = new Vector2(ReferenceCollider.bounds.min.x, ReferenceCollider.bounds.center.y);
                        BoxInfos[i].size = new Vector2(1f * BoxInfos[i].SizeMultiplier, ReferenceCollider.size.y);
                        BoxInfos[i].castDirection = Vector2.left;
                        break;
                    case Direction.Right:
                        BoxInfos[i].origin = new Vector2(ReferenceCollider.bounds.max.x, ReferenceCollider.bounds.center.y);
                        BoxInfos[i].size = new Vector2(1f * BoxInfos[i].SizeMultiplier, ReferenceCollider.size.y);
                        BoxInfos[i].castDirection = Vector2.right;
                        break;
                }

                int hitCount = Physics2D.BoxCastNonAlloc(BoxInfos[i].Origin,
                                                         BoxInfos[i].Size,
                                                         0f,
                                                         BoxInfos[i].CastDirection,
                                                         hitBuffer,
                                                         BoxInfos[i].Distance);

                for(int a = 0; a < hitCount; a++) {
                    if(touchColliderMask != null) {
                        if(!touchColliderMask.Contains(hitBuffer[a].collider)) {
                            continue;
                        }
                    }

                    if(boxCastMask != null) {
                        if(boxCastMask.Contains(hitBuffer[a].collider)) {
                            continue;
                        }
                    }

                    if(!BoxInfos[i].Hits.Contains(hitBuffer[a].collider)) {
                        BoxInfos[i].Hits.Add(hitBuffer[a].collider);
                        if(onHit != null) {
                            onHit(BoxInfos[i].Direction, hitBuffer[a], hitBuffer[a].collider);
                        }
                    }
                }
            }
        }
    
        public void RemoveHit(Collider2D collider) {
            for(int i = 0; i < BoxInfos.Length; i++) {
                BoxInfos[i].Hits.Remove(collider);
            }
        }

        [Serializable]
        public class BoxCastInfo {
            public Vector2 Origin { get { return origin; } }
            public Vector2 Size { get { return size; } }
            public Vector2 CastDirection { get { return castDirection; } }

            public Direction                    Direction;
            public float                        SizeMultiplier = 1f;
            public float                        Distance = 1f;
            public List<Collider2D>             Hits = new List<Collider2D>();

            internal Vector2                    origin;
            internal Vector2                    size;
            internal Vector2                    castDirection;
        }
    }


    public class Utilities {
        public static GameObject CreateObject(string path, Transform parent = null) {
            string[] splitPath = path.Split('/');

            for(int i = 0; i < splitPath.Length; i++) {
                string curPath = splitPath[i];

                Transform child = parent ? parent.FindDeepChild(curPath, TransformExtensions.SearchType.BreadthFirst) : null;

                if(!child) {
                    child = new GameObject(curPath).GetComponent<Transform>();
                    child.SetParent(parent, false);
                    child.localPosition = Vector3.zero;
                }

                parent = child;
            }

            return parent.gameObject;
        }


        public static bool CheckOtherColliderDirection2D(Direction direction, Collider2D collider, Collider2D otherCollider, float maxDistanceDelta = 1f) {
            switch(direction) {
                case Direction.Up:
                    return collider.bounds.max.y < otherCollider.bounds.max.y &&
                           Mathf.Abs(collider.bounds.max.y - otherCollider.bounds.max.y) <= maxDistanceDelta;
                case Direction.Down:
                    return collider.bounds.min.y > otherCollider.bounds.min.y &&
                           Mathf.Abs(collider.bounds.min.y - otherCollider.bounds.min.y) <= maxDistanceDelta;
                case Direction.Left:
                    return collider.bounds.min.x > otherCollider.bounds.min.x &&
                           Mathf.Abs(collider.bounds.min.x - otherCollider.bounds.min.x) <= maxDistanceDelta;
                case Direction.Right:
                    return collider.bounds.max.x < otherCollider.bounds.max.x &&
                           Mathf.Abs(collider.bounds.max.x - otherCollider.bounds.max.x) <= maxDistanceDelta;
                default:
                    return false;
            }

            //switch(direction) {
            //    case Direction.Up:
            //        return collider.bounds.max.y < otherCollider.bounds.min.y &&
            //               Mathf.Abs(collider.bounds.max.y - otherCollider.bounds.min.y) <= maxDistanceDelta;
            //    case Direction.Down:
            //        return collider.bounds.min.y > otherCollider.bounds.max.y &&
            //               Mathf.Abs(collider.bounds.min.y - otherCollider.bounds.max.y) <= maxDistanceDelta;
            //    case Direction.Left:
            //        return collider.bounds.min.x > otherCollider.bounds.max.x &&
            //               Mathf.Abs(collider.bounds.min.x - otherCollider.bounds.max.x) <= maxDistanceDelta;
            //    case Direction.Right:
            //        return collider.bounds.max.x < otherCollider.bounds.min.x &&
            //               Mathf.Abs(collider.bounds.max.x - otherCollider.bounds.min.x) <= maxDistanceDelta;
            //    default:
            //        return false;
            //}
        }
    }
}