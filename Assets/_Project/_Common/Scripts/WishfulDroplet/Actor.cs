using UnityEngine;
using System;


namespace WishfulDroplet {
    public abstract class Actor<TActor, TBrain> : MonoActor
        where TActor : Actor<TActor, TBrain>
        where TBrain : ActorBrain<TActor> {
		public new TBrain brain {
			get {
				// We cache it so we don't cast everytime
				// #MICRO-OPTIMIZATIIIIION
				if(_cachedBrain != _brain) {
					_cachedBrain = (TBrain)_brain;
					Debug.Log("Caching brain");
				}
				return _cachedBrain;
			}
			private set {
				_brain = value;
			}
		}

		public TActor thisActor {
			get { return _thisActor; }
			private set { _thisActor = value; }
		}

		[InspectorNote("Actor")]
		[Header("Base Refereces")]
        [SerializeField] protected TActor _thisActor;

		[Header("Base Editor Internal")]
		[SerializeField] protected TBrain _cachedBrain;


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
			if(brain != null) {
				if(!Equals(brain, _cachedBrain)) {
					brain.DoReset(_thisActor);
					_cachedBrain = brain;
				}
			}
		}

		protected override void Reset() {
			base.Reset();

            thisActor = (TActor)this;
        }
	}

    public abstract class ActorBrain<TActor> : ScriptableActorBrain
        where TActor : IActor {
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


	[Serializable]
	public abstract class MonoActor : MonoBehaviour, IActor {
		public ScriptableActorBrain brain {
			get { return _brain; }
			set { _brain = value; }
		}

		public GameObject thisGameObject {
			get { return _thisGameObject; }
			private set { _thisGameObject = value; }
		}

		public Transform thisTransform {
			get { return _thisTransform; }
			private set { _thisTransform = value; }
		}

		[InspectorNote("Mono Actor")]
		[Header("Base Refereces")]
		[SerializeField] protected ScriptableActorBrain _brain;
		[SerializeField] protected GameObject _thisGameObject;
		[SerializeField] protected Transform _thisTransform;


		protected virtual void Reset() {
			thisGameObject = gameObject;
			thisTransform = GetComponent<Transform>();
		}


		IActorBrain IActor.brain {
			get { return _brain; }
			set { _brain = (ScriptableActorBrain)value; }
		}
	}


	[Serializable]
	public abstract class ScriptableActorBrain : ScriptableObject, IActorBrain {

	}


	public interface IActor {
		IActorBrain brain { get; set; }
	}


	public interface IActorBrain {

	}

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