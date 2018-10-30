using UnityEngine;


namespace WishfulDroplet {
    public abstract class Actor<TActor, TBrain> : _InternalActor
        where TActor : Actor<TActor, TBrain>
        where TBrain : ActorBrain<TActor> {
		public new TBrain brain {
			get {
				// We cache it so we don't cast everytime
				// #MICRO-OPTIMIZATIIIIION
				if(cachedBrain != base.brain) {
					cachedBrain = (TBrain)base.brain;
					Debug.Log("Caching brain");
				}
				return cachedBrain;
			}
			private set {
				base.brain = value;
			}
		}

		public GameObject thisGameObject {
            get { return _thisGameObject; }
            private set { _thisGameObject = value; }
        }

        public Transform thisTransform {
            get { return _thisTransform; }
            private set { _thisTransform = value; }
        }

		[InspectorNote("Actor")]
		[Header("Base Refereces")]
        [SerializeField] protected TActor _thisActor;
        [SerializeField] protected GameObject _thisGameObject;
        [SerializeField] protected Transform _thisTransform;

        [Header("Base Editor Internal")]
        [SerializeField] protected TBrain oldBrain;
		[SerializeField] protected TBrain cachedBrain; 


        protected bool IsBrainOnSet<T>(T[] brainSet, T brain) {
            if(brain == null) return false;

            for(int i = 0; i < brainSet.Length; i++) {
                if(brainSet[i].Equals(brain)) {
                    return true;
                }
            }

            return false;
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


    public abstract class _InternalActor : MonoBehaviour {
		[InspectorNote("_Internal Actor")]
		[Header("Base Data")]
		public _InternalActorBrain brain;
	}

    public abstract class _InternalActorBrain : ScriptableObject { }

	//public class LevelItemActor : LevelItemActor<LevelItemActor, LevelItemActor.LevelItemBrain> {
	//    public abstract class LevelItemBrain : ActorBrain<LevelItemActor> {

	//    }
	//}


	//public class LevelItemActor<T, U> : Actor<T, U>
	//    where T : Actor<T, U>
	//    where U : ActorBrain<T> {
	//    [InspectorNote("Level Item Actor")]
	//    public int test = 0;
	//}
}