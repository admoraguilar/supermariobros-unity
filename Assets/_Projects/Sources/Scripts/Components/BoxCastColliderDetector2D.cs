using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;


[ExecuteInEditMode()]
public class BoxCastColliderDetector2D : ColliderDetector2D {
    public IList<BoxCastDetectorInfo> BoxCastDetectors { get { return boxCastDetectors; } }

    [SerializeField] private List<BoxCastDetectorInfo> boxCastDetectors = new List<BoxCastDetectorInfo>();


    /// <summary>
    /// Use to query if a non-trigger or trigger collider is within bounds.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="collider"></param>
    /// <returns></returns>
    public override bool IsColliderWithinBounds(Direction direction, Collider2D collider = null) {
        if(direction == Direction.Any) {
            for(int i = 0; i < boxCastDetectors.Count; i++) {
                if(collider) {
                    if(boxCastDetectors[i].HitColliders.Contains(collider)) {
                        return true;
                    }
                } else {
                    if(boxCastDetectors[i].HitColliders.Count != 0) {
                        return true;
                    }
                }
            }

            return false;
        } else {
            BoxCastDetectorInfo detectorInfo = GetDetector(direction);
            return collider ? detectorInfo.HitColliders.Contains(collider) : detectorInfo.HitColliders.Count != 0;
        }
    }

    /// <summary>
    /// Use to query if a non-trigger collider is within bounds.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="collider"></param>
    /// <returns></returns>
    public override bool IsColliding(Direction direction, Collider2D collider = null) {
        if(direction == Direction.Any) {
            for(int i = 0; i < boxCastDetectors.Count; i++) {
                IEnumerable<Collider2D> nonTriggerColliders = boxCastDetectors[i].HitColliders.Where(hit => hit.isTrigger == false);
                if(collider) {
                    if(nonTriggerColliders.Contains(collider)) {
                        return true;
                    }
                } else {
                    if(nonTriggerColliders.Count() != 0) {
                        return true;
                    }
                }
            }

            return false;
        } else {
            BoxCastDetectorInfo detectorInfo = GetDetector(direction);
            IEnumerable<Collider2D> nonTriggerColliders = detectorInfo.HitColliders.Where(hit => hit.isTrigger == false);
            return collider ? nonTriggerColliders.Contains(collider) : nonTriggerColliders.Count() != 0;
        }
    }

    public override Collider2D[] GetDetectedColliders(Direction direction) {
        return GetDetector(direction).HitColliders.ToArray();
    }

    protected override void UpdateDetector() {
        for(int i = 0; i < boxCastDetectors.Count; i++) {
            if(!boxCastDetectors[i].IsEnable) continue;

            // Create box cast
            switch(boxCastDetectors[i].Direction) {
                case Direction.Up:
                    boxCastDetectors[i].Center          = new Vector2(boxCastDetectors[i].ReferenceCollider.bounds.center.x, 
                                                                      boxCastDetectors[i].ReferenceCollider.bounds.max.y);
                    boxCastDetectors[i].Size            = new Vector2(boxCastDetectors[i].ReferenceCollider.bounds.size.x * boxCastDetectors[i].SizeMultiplier, 
                                                                      boxCastDetectors[i].ReferenceCollider.bounds.size.y * .2f);
                    boxCastDetectors[i].CastDirection   = Vector2.up;
                    break;
                case Direction.Down:
                    boxCastDetectors[i].Center          = new Vector2(boxCastDetectors[i].ReferenceCollider.bounds.center.x, 
                                                                      boxCastDetectors[i].ReferenceCollider.bounds.min.y);
                    boxCastDetectors[i].Size            = new Vector2(boxCastDetectors[i].ReferenceCollider.bounds.size.x * boxCastDetectors[i].SizeMultiplier, 
                                                                      boxCastDetectors[i].ReferenceCollider.bounds.size.y * .2f);
                    boxCastDetectors[i].CastDirection   = Vector2.down;
                    break;
                case Direction.Left:
                    boxCastDetectors[i].Center          = new Vector2(boxCastDetectors[i].ReferenceCollider.bounds.min.x, 
                                                                      boxCastDetectors[i].ReferenceCollider.bounds.center.y);
                    boxCastDetectors[i].Size            = new Vector2(boxCastDetectors[i].ReferenceCollider.bounds.size.x * .2f, 
                                                                      boxCastDetectors[i].ReferenceCollider.bounds.size.y * boxCastDetectors[i].SizeMultiplier);
                    boxCastDetectors[i].CastDirection   = Vector2.left;
                    break;
                case Direction.Right:
                    boxCastDetectors[i].Center          = new Vector2(boxCastDetectors[i].ReferenceCollider.bounds.max.x, 
                                                                      boxCastDetectors[i].ReferenceCollider.bounds.center.y);
                    boxCastDetectors[i].Size            = new Vector2(boxCastDetectors[i].ReferenceCollider.bounds.size.x * .2f, 
                                                                      boxCastDetectors[i].ReferenceCollider.bounds.size.y * boxCastDetectors[i].SizeMultiplier);
                    boxCastDetectors[i].CastDirection   = Vector2.right;
                    break;
            }

            boxCastDetectors[i].OldColliders.Clear();
            boxCastDetectors[i].OldColliders.AddRange(boxCastDetectors[i].HitColliders);

            boxCastDetectors[i].HitColliders.Clear();
            RaycastHit2D[] rHits = Physics2D.BoxCastAll(boxCastDetectors[i].Center, 
                                                        boxCastDetectors[i].Size, 
                                                        0f, 
                                                        boxCastDetectors[i].CastDirection, 
                                                        boxCastDetectors[i].Distance);
            
            // Filter cast hits
            foreach(var rHit in rHits) {
                if(toIgnores.Contains(rHit.collider)) continue;
                boxCastDetectors[i].HitColliders.Add(rHit.collider);
            }


            // Call events
            foreach(var collider in boxCastDetectors[i].OldColliders) {
                if(!boxCastDetectors[i].HitColliders.Contains(collider)) _OnColliderExit(boxCastDetectors[i].Direction, collider);
            }

            foreach(var collider in boxCastDetectors[i].HitColliders) {
                if(boxCastDetectors[i].OldColliders.Contains(collider)) _OnColliderStay(boxCastDetectors[i].Direction, collider);
                else _OnColliderEnter(boxCastDetectors[i].Direction, collider);
            }
        }
    }

    public BoxCastDetectorInfo GetDetector(Direction direction) {
        return boxCastDetectors.FirstOrDefault(d => d.Direction == direction);
    }

    public void Init(BoxCastDetectorInfo[] boxCastDetectorInfos) {
        boxCastDetectors.Clear();
        boxCastDetectors.AddRange(boxCastDetectorInfos);
    }

    private void OnValidate() {
        UpdateDetector();
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;

        for(int i = 0; i < boxCastDetectors.Count; i++) {
            if(!boxCastDetectors[i].IsEnable) continue;

            Gizmos.DrawWireCube(boxCastDetectors[i].Center, boxCastDetectors[i].Size);
        }
    }


    [Serializable]
    public class BoxCastDetectorInfo {
        [HideInInspector] public Vector2            Center;
        [HideInInspector] public Vector2            Size;
        [HideInInspector] public Vector2            CastDirection;

        public Direction                            Direction;
        public bool                                 IsEnable = true;
        public Collider2D                           ReferenceCollider;
        public float                                SizeMultiplier = 1f;
        public float                                Distance = 1f;

        [Header("Runtime")]
        [HideInInspector] public List<Collider2D>   OldColliders = new List<Collider2D>();
        public List<Collider2D>                     HitColliders = new List<Collider2D>();
    }
}
