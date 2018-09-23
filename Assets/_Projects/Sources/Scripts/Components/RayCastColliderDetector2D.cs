using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;


[ExecuteInEditMode()]
public class RayCastColliderDetector2D : ColliderDetector2D {
    public IList<RayCastDetectorInfo> RayCastDetectors { get { return raycastDetectors; } }

    [SerializeField] private List<RayCastDetectorInfo> raycastDetectors = new List<RayCastDetectorInfo>();


    public override bool IsColliding(Direction direction, Collider2D collider = null) {
        if(direction == Direction.Any) {
            for(int i = 0; i < raycastDetectors.Count; i++) {
                if(collider) {
                    if(raycastDetectors[i].HitColliders.Contains(collider)) {
                        return true;
                    }
                } else {
                    if(raycastDetectors[i].HitColliders.Count != 0) {
                        return true;
                    }
                }
            }

            return false;
        } else {
            RayCastDetectorInfo detectorInfo = GetDetector(direction);
            return collider ? detectorInfo.HitColliders.Contains(collider) : detectorInfo.HitColliders.Count != 0;
        }
    }

    public override Collider2D[] GetDetectedColliders(Direction direction) {
        return GetDetector(direction).HitColliders.ToArray();
    }

    protected override void UpdateDetector() {
        for(int i = 0; i < raycastDetectors.Count; i++) {
            if(!raycastDetectors[i].IsEnable) continue;

            // Create box cast
            switch(raycastDetectors[i].Direction) {
                case Direction.Up:
                    raycastDetectors[i].Origin = new Vector2(raycastDetectors[i].ReferenceCollider.bounds.center.x * Mathf.Sign(raycastDetectors[i].ReferenceCollider.transform.localScale.x),
                                                             raycastDetectors[i].ReferenceCollider.bounds.max.y) + raycastDetectors[i].Offset;
                    raycastDetectors[i].CastDirection = Vector2.up;
                    break;
                case Direction.Down:
                    raycastDetectors[i].Origin = new Vector2(raycastDetectors[i].ReferenceCollider.bounds.center.x * Mathf.Sign(raycastDetectors[i].ReferenceCollider.transform.localScale.x),
                                                             raycastDetectors[i].ReferenceCollider.bounds.min.y) + raycastDetectors[i].Offset;
                    raycastDetectors[i].CastDirection = Vector2.down;
                    break;
                case Direction.Left:
                    raycastDetectors[i].Origin = new Vector2(raycastDetectors[i].ReferenceCollider.bounds.min.x,
                                                             raycastDetectors[i].ReferenceCollider.bounds.center.y * Mathf.Sign(raycastDetectors[i].ReferenceCollider.transform.localScale.y)) + raycastDetectors[i].Offset;
                    raycastDetectors[i].CastDirection = Vector2.left;
                    break;
                case Direction.Right:
                    raycastDetectors[i].Origin = new Vector2(raycastDetectors[i].ReferenceCollider.bounds.max.x,
                                                             raycastDetectors[i].ReferenceCollider.bounds.center.y * Mathf.Sign(raycastDetectors[i].ReferenceCollider.transform.localScale.y)) + raycastDetectors[i].Offset;
                    raycastDetectors[i].CastDirection = Vector2.right;
                    break;
            }

            raycastDetectors[i].OldColliders.Clear();
            raycastDetectors[i].OldColliders.AddRange(raycastDetectors[i].HitColliders);

            raycastDetectors[i].HitColliders.Clear();
            RaycastHit2D[] rHits = Physics2D.RaycastAll(raycastDetectors[i].Origin,
                                                        raycastDetectors[i].CastDirection,
                                                        raycastDetectors[i].Distance);

            // Filter cast hits
            foreach(var rHit in rHits) {
                if(toIgnores.Contains(rHit.collider)) continue;
                raycastDetectors[i].HitColliders.Add(rHit.collider);
            }


            // Call events
            foreach(var collider in raycastDetectors[i].OldColliders) {
                if(!raycastDetectors[i].HitColliders.Contains(collider)) _OnColliderExit(raycastDetectors[i].Direction, collider);
            }

            foreach(var collider in raycastDetectors[i].HitColliders) {
                if(raycastDetectors[i].OldColliders.Contains(collider)) _OnColliderStay(raycastDetectors[i].Direction, collider);
                else _OnColliderEnter(raycastDetectors[i].Direction, collider);
            }
        }
    }

    public RayCastDetectorInfo GetDetector(Direction direction) {
        return raycastDetectors.FirstOrDefault(d => d.Direction == direction);
    }

    public void Init(RayCastDetectorInfo[] boxCastDetectorInfos) {
        raycastDetectors.Clear();
        raycastDetectors.AddRange(boxCastDetectorInfos);
    }

    private void OnValidate() {
        UpdateDetector();
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;

        for(int i = 0; i < raycastDetectors.Count; i++) {
            if(!raycastDetectors[i].IsEnable) continue;

            Gizmos.DrawRay(raycastDetectors[i].Origin, raycastDetectors[i].CastDirection * raycastDetectors[i].Distance);
        }
    }

    [Serializable]
    public class RayCastDetectorInfo {
        [HideInInspector] public Vector2 Origin;
        [HideInInspector] public Vector2 CastDirection;

        public Direction Direction;
        public bool IsEnable = true;
        public Collider2D ReferenceCollider;
        public Vector2 Offset = Vector2.zero;
        public float Distance = 1f;

        [Header("Runtime")]
        [HideInInspector] public List<Collider2D> OldColliders = new List<Collider2D>();
        public List<Collider2D> HitColliders = new List<Collider2D>();
    }
}
