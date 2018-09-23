using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;


[ExecuteInEditMode()]
public class BoxColliderDetector2D : ColliderDetector2D {
    public IList<BoxDetectorInfo>                       BoxDetectors { get { return boxDetectors; } }

    [SerializeField] private List<BoxDetectorInfo>      boxDetectors = new List<BoxDetectorInfo>();


    public override bool IsColliding(Direction direction, Collider2D collider = null) {
        if(direction == Direction.Any) {
            for(int i = 0; i < boxDetectors.Count; i++) {
                if(collider) {
                    if(boxDetectors[i].HitColliders.Contains(collider)) {
                        return true;
                    }
                } else {
                    if(boxDetectors[i].HitColliders.Count != 0) {
                        return true;
                    }
                }
            }

            return false;
        } else {
            BoxDetectorInfo detectorInfo = GetDetector(direction);
            return collider ? detectorInfo.HitColliders.Contains(collider) : detectorInfo.HitColliders.Count != 0;
        }
    }

    public override Collider2D[] GetDetectedColliders(Direction direction) {
        return GetDetector(direction).HitColliders.ToArray();
    }

    protected override void UpdateDetector() {
        for(int i = 0; i < boxDetectors.Count; i++) {
            boxDetectors[i].Collider.gameObject.SetActive(boxDetectors[i].IsEnable);
            boxDetectors[i].Collider.isTrigger = boxDetectors[i].IsTrigger;
        }
    }

    public bool IsUsingCollider(Collider2D collider) {
        return boxDetectors.FirstOrDefault(d => d.Collider == collider) != null;
    }

    public void Init(BoxDetectorInfo[] boxDetectorInfos) {
        boxDetectors.Clear();
        boxDetectors.AddRange(boxDetectorInfos);
    }

    private BoxDetectorInfo GetDetector(Direction direction) {
        return boxDetectors.FirstOrDefault(d => d.Direction == direction);
    }

    private void HookCollider(List<BoxDetectorInfo> boxDetectorInfo, bool isHook) {
        for(int i = 0; i < boxDetectorInfo.Count; i++) {
            int capture = i;
            if(boxDetectorInfo[capture].OnCollisionEnter2D == null) {
                boxDetectorInfo[capture].OnCollisionEnter2D = (collision) => {
                    if(toIgnores.Contains(collision.collider)) return;

                    boxDetectorInfo[capture].HitColliders.Add(collision.collider);
                    _OnColliderEnter(boxDetectorInfo[capture].Direction, collision.collider);
                };
            }

            if(boxDetectorInfo[capture].OnCollisionStay2D == null) {
                boxDetectorInfo[capture].OnCollisionStay2D = (collision) => {
                    if(boxDetectorInfo[capture].HitColliders.Contains(collision.collider)) {
                        _OnColliderStay(boxDetectorInfo[capture].Direction, collision.collider);
                    }
                };
            }

            if(boxDetectorInfo[capture].OnCollisionExit2D == null) {
                boxDetectorInfo[capture].OnCollisionExit2D = (collision) => {
                    boxDetectorInfo[capture].HitColliders.Remove(collision.collider);
                    _OnColliderExit(boxDetectorInfo[capture].Direction, collision.collider);
                };
            }

            if(boxDetectorInfo[capture].OnTriggerEnter2D == null) {
                boxDetectorInfo[capture].OnTriggerEnter2D = (collider) => {
                    if(toIgnores.Contains(collider)) return;

                    boxDetectorInfo[capture].HitColliders.Add(collider);
                    _OnColliderEnter(boxDetectorInfo[capture].Direction, collider);
                };
            }

            if(boxDetectorInfo[capture].OnTriggerStay2D == null) {
                boxDetectorInfo[capture].OnTriggerStay2D = (collider) => {
                    if(boxDetectorInfo[capture].HitColliders.Contains(collider)) {
                        _OnColliderStay(boxDetectorInfo[capture].Direction, collider);
                    }
                };
            }

            if(boxDetectorInfo[capture].OnTriggerExit2D == null) {
                boxDetectorInfo[capture].OnTriggerExit2D = (collider) => {
                    boxDetectorInfo[capture].HitColliders.Remove(collider);
                    _OnColliderExit(boxDetectorInfo[capture].Direction, collider);
                };
            }

            if(isHook) {
                boxDetectorInfo[capture].ColliderEventHook.AOnCollisionEnter2D += boxDetectorInfo[capture].OnCollisionEnter2D;
                boxDetectorInfo[capture].ColliderEventHook.AOnCollisionStay2D += boxDetectorInfo[capture].OnCollisionStay2D;
                boxDetectorInfo[capture].ColliderEventHook.AOnCollisionExit2D += boxDetectorInfo[capture].OnCollisionExit2D;
                boxDetectorInfo[capture].ColliderEventHook.AOnTriggerEnter2D += boxDetectorInfo[capture].OnTriggerEnter2D;
                boxDetectorInfo[capture].ColliderEventHook.AOnTriggerStay2D += boxDetectorInfo[capture].OnTriggerStay2D;
                boxDetectorInfo[capture].ColliderEventHook.AOnTriggerExit2D += boxDetectorInfo[capture].OnTriggerExit2D;
            } else {
                boxDetectorInfo[capture].ColliderEventHook.AOnCollisionEnter2D -= boxDetectorInfo[capture].OnCollisionEnter2D;
                boxDetectorInfo[capture].ColliderEventHook.AOnCollisionStay2D -= boxDetectorInfo[capture].OnCollisionStay2D;
                boxDetectorInfo[capture].ColliderEventHook.AOnCollisionExit2D -= boxDetectorInfo[capture].OnCollisionExit2D;
                boxDetectorInfo[capture].ColliderEventHook.AOnTriggerEnter2D -= boxDetectorInfo[capture].OnTriggerEnter2D;
                boxDetectorInfo[capture].ColliderEventHook.AOnTriggerStay2D -= boxDetectorInfo[capture].OnTriggerStay2D;
                boxDetectorInfo[capture].ColliderEventHook.AOnTriggerExit2D -= boxDetectorInfo[capture].OnTriggerExit2D;
            }
        }
    }

    private void Start() {
        HookCollider(boxDetectors, true);
    }

    private void OnValidate() {
        UpdateDetector();
    }


    [Serializable]
    public class BoxDetectorInfo {
        public Direction                    Direction;
        public bool                         IsEnable;
        public bool                         IsTrigger = false;
        public Collider2D                   Collider;
        public Collider2DEventHook          ColliderEventHook;

        [Header("Runtime")]
        public List<Collider2D>             HitColliders = new List<Collider2D>();

        internal Action<Collision2D>        OnCollisionEnter2D;
        internal Action<Collision2D>        OnCollisionStay2D;
        internal Action<Collision2D>        OnCollisionExit2D;
        internal Action<Collider2D>         OnTriggerEnter2D;
        internal Action<Collider2D>         OnTriggerStay2D;
        internal Action<Collider2D>         OnTriggerExit2D;
    }
}
