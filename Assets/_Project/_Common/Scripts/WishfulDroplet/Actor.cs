using UnityEngine;


namespace WishfulDroplet {
    public abstract class Actor<TActor, TBrain> : _InternalActor
        where TActor : Actor<TActor, TBrain>
        where TBrain : ActorBrain<TActor> {
        public GameObject thisGameObject {
            get { return _thisGameObject; }
            private set { _thisGameObject = value; }
        }

        public Transform thisTransform {
            get { return _thisTransform; }
            private set { _thisTransform = value; }
        }

        [InspectorNote("Actor")]
        [Header("Base Data")]
        public TBrain brain;
        

        [Header("Base Refereces")]
        [SerializeField] protected TActor _thisActor;
        [SerializeField] protected GameObject _thisGameObject;
        [SerializeField] protected Transform _thisTransform;

        [Header("Base Editor Internal")]
        [SerializeField] protected TBrain oldBrain;


        protected TBrain CheckBrainIfOnSet(TBrain[] brainSet, TBrain brain)  {
            if(brain == null) return default(TBrain);

            for(int i = 0; i < brainSet.Length; i++) {
                if(brainSet[i].Equals(brain)) {
                    return brainSet[i];
                }
            }

            return default(TBrain);
        }

        protected virtual void OnValidate() {
            if(brain) {
                if(brain != oldBrain) {
                    brain.DoReset(_thisActor);
                    oldBrain = brain;
                }
            }
        }

        protected virtual void Reset() {
            _thisActor = (TActor)this;

            _thisGameObject = gameObject;
            _thisTransform = GetComponent<Transform>();
        }
    }

    public abstract class ActorBrain<TActor> : _InternalActorBrain
        where TActor : _InternalActor {
        public virtual void DoAwake(TActor actor) { }
        public virtual void DoOnEnable(TActor actor) { }
        public virtual void DoOnDisable(TActor actor) { }
        public virtual void DoStart(TActor actor) { }
        public virtual void DoUpdate(TActor actor) { }
        public virtual void DoFixedUpdate(TActor actor) { }
        public virtual void DoCollisionEnter2D(TActor actor, Collision2D collision) { }
        public virtual void DoCollisionStay2D(TActor actor, Collision2D collision) { }
        public virtual void DoCollisionExit2D(TActor actor, Collision2D collision) { }
        public virtual void DoTriggerEnter2D(TActor actor, Collider2D collision) { }
        public virtual void DoTriggerStay2D(TActor actor, Collider2D collision) { }
        public virtual void DoTriggerExit2D(TActor actor, Collider2D collision) { }
        public virtual void DoDrawGizmos(TActor actor) { }
        public virtual void DoReset(TActor actor) { }
    }


    public abstract class _InternalActor : MonoBehaviour { }
    public abstract class _InternalActorBrain : ScriptableObject { }
}