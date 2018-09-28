using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;


[ExecuteInEditMode()]
public class DirectionalCollider2DDetector : Collider2DDetector {
    public new event Action<Direction, Collider2D> OnColliderEnter = delegate { };
    public new event Action<Direction, Collider2D> OnColliderStay = delegate { };
    public new event Action<Direction, Collider2D> OnColliderExit = delegate { };

    [SerializeField] private List<DetectorInfo> detectors = new List<DetectorInfo>();
    [SerializeField] private Collider2D referenceCollider;


    public bool IsColliding(Direction direction, bool noTriggers = false, Collider2D collider = null) {
        if(direction == Direction.Any) {
            bool isColliderDetected = false;

            for(int i = 0; i < detectors.Count; i++) {
                if(!noTriggers) {
                    if(detectors[i].Detector.GetDetectedColliders().Count() != 0) {
                        isColliderDetected = true;
                    }
                } else {
                    if(detectors[i].Detector.GetDetectedColliders().FirstOrDefault(c => c.isTrigger == false)) {
                        isColliderDetected = true;
                    }
                }
            }

            return isColliderDetected;
        } else {
            DetectorInfo detector = GetDetector(direction);
            return detector != null ? detector.Detector.IsColliding(noTriggers, collider) : false;
        }
    }

    public IEnumerable<Collider2D> GetDetectedColliders(Direction direction) {
        if(direction == Direction.Any) {
            List<Collider2D> colliders = new List<Collider2D>();
            for(int i = 0; i < detectors.Count; i++) {
                colliders.AddRange(detectors[i].Detector.GetDetectedColliders());
            }
            return colliders;
        } else {
            DetectorInfo detector = GetDetector(direction);
            return detector != null ? detector.Detector.GetDetectedColliders() : null;
        }
    }

    public DetectorInfo GetDetector(Direction direction) {
        return detectors.FirstOrDefault(d => d.Direction == direction);
    }

    public void Init(DetectorInfo[] detectorInfo,
                     Collider2D referenceCollider) {
        detectors.Clear();
        detectors.AddRange(detectorInfo);
        this.referenceCollider = referenceCollider;
    }

    private void Start() {
        // Hook detection events
        for(int i = 0; i < detectors.Count; i++) {
            int catchIndex = i;

            if(detectors[catchIndex].Detector == null) continue;

            detectors[catchIndex].Detector.OnColliderEnter += (Collider2D collider2D) => {
                // Abstract class compatibility
                hitColliders.Add(collider2D);
                RaiseOnColliderEnter(collider2D);

                OnColliderEnter(detectors[catchIndex].Direction, collider2D);
            };

            detectors[catchIndex].Detector.OnColliderStay += (Collider2D collider2D) => {
                // Abstract class compatibility
                RaiseOnColliderStay(collider2D);

                OnColliderStay(detectors[catchIndex].Direction, collider2D);
            };

            detectors[catchIndex].Detector.OnColliderExit += (Collider2D collider2D) => {
                // Abstract class compatibility
                hitColliders.Remove(collider2D);
                RaiseOnColliderExit(collider2D);

                OnColliderExit(detectors[catchIndex].Direction, collider2D);  
            };
        }
    }

    protected override void UpdateDetector() {
        // Update collider origins
        for(int i = 0; i < detectors.Count; i++) {
            if(detectors[i].Detector.gameObject.activeSelf != detectors[i].IsEnable) {
                detectors[i].Detector.gameObject.SetActive(detectors[i].IsEnable);
            }

            if(!detectors[i].IsEnable) {
                continue;
            }

            BoxCastCollider2DDetector boxCast = detectors[i].Detector as BoxCastCollider2DDetector;
            if(boxCast) {
                if(referenceCollider) {
                    boxCast.CenterPlacement = BoxCastCollider2DDetector.Placement.Free;

                    switch(detectors[i].Direction) {
                        case Direction.Up:
                            boxCast.Center = new Vector2(referenceCollider.bounds.center.x,
                                                         referenceCollider.bounds.max.y);
                            boxCast.Size = new Vector2(referenceCollider.bounds.size.x * boxCast.SizeMultiplier,
                                                       referenceCollider.bounds.size.y * .2f);
                            boxCast.CastDirection = Vector2.up;
                            break;
                        case Direction.Down:
                            boxCast.Center = new Vector2(referenceCollider.bounds.center.x,
                                                         referenceCollider.bounds.min.y);
                            boxCast.Size = new Vector2(referenceCollider.bounds.size.x * boxCast.SizeMultiplier,
                                                       referenceCollider.bounds.size.y * .2f);
                            boxCast.CastDirection = Vector2.down;
                            break;
                        case Direction.Left:
                            boxCast.Center = new Vector2(referenceCollider.bounds.min.x,
                                                         referenceCollider.bounds.center.y);
                            boxCast.Size = new Vector2(referenceCollider.bounds.size.x * .2f,
                                                       referenceCollider.bounds.size.y * boxCast.SizeMultiplier);
                            boxCast.CastDirection = Vector2.left;
                            break;
                        case Direction.Right:
                            boxCast.Center = new Vector2(referenceCollider.bounds.max.x,
                                                         referenceCollider.bounds.center.y);
                            boxCast.Size = new Vector2(referenceCollider.bounds.size.x * .2f,
                                                       referenceCollider.bounds.size.y * boxCast.SizeMultiplier);
                            boxCast.CastDirection = Vector2.right;
                            break;
                    }
                } else {
                    boxCast.CenterPlacement = BoxCastCollider2DDetector.Placement.FromPosition;
                }
                continue;
            }

            RayCastCollider2DDetector rayCast = detectors[i].Detector as RayCastCollider2DDetector;
            if(rayCast) {
                if(referenceCollider) {
                    rayCast.OriginPlacement = RayCastCollider2DDetector.Placement.Free;

                    switch(detectors[i].Direction) {
                        case Direction.Up:
                            rayCast.Origin = new Vector2(referenceCollider.bounds.center.x, referenceCollider.bounds.max.y) + rayCast.Offset;
                            rayCast.CastDirection = Vector2.up;
                            break;
                        case Direction.Down:
                            rayCast.Origin = new Vector2(referenceCollider.bounds.center.x, referenceCollider.bounds.min.y) + rayCast.Offset;
                            rayCast.CastDirection = Vector2.down;
                            break;
                        case Direction.Left:
                            rayCast.Origin = new Vector2(referenceCollider.bounds.min.x, referenceCollider.bounds.center.y) + rayCast.Offset;
                            rayCast.CastDirection = Vector2.left;
                            break;
                        case Direction.Right:
                            rayCast.Origin = new Vector2(referenceCollider.bounds.max.x, referenceCollider.bounds.center.y) + rayCast.Offset;
                            rayCast.CastDirection = Vector2.right;
                            break;
                    }
                } else {
                    rayCast.OriginPlacement = RayCastCollider2DDetector.Placement.FromPosition;
                }
                continue;
            }
        }
    }


    [Serializable]
    public class DetectorInfo {
        public Collider2DDetector Detector;
        public Direction Direction;
        public bool IsEnable = true;
    }
}
