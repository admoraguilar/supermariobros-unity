using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace WishfulDroplet {
	using Extensions;


	[Flags]
	public enum Direction {
		Up			= 0,
		Down		= 1,
		Left		= 2,
		Right		= 3,
	}


	public enum CollisionFilter {
		OnlyNonTrigger,
		OnlyTrigger,
		Both
	}


	/// <summary>
	/// Useful for detecting any colliders in a 4-directional way.
	/// </summary>
	[Serializable]
	public class DirectionalBoxCast2D {
		public event Action<Direction, RaycastHit2D, Collider2D>		OnHit = delegate { };

		public List<BoxCastInfo>										boxCastInfos = new List<BoxCastInfo>();
		public List<Collider2D>											boxCastMask = new List<Collider2D>();
		public BoxCollider2D											referenceCollider;
		public LayerMask												layerMask;
		
		private RaycastHit2D[]											_hitBuffer;


		public DirectionalBoxCast2D(int hitBufferSize = 20) {
			SetHitBufferSize(hitBufferSize);
		}

		public bool IsHitAtAnyDirection(CollisionFilter filter) {
			return IsEvaluateHitsCollisionType(boxCastInfos, filter);
		}

		public bool IsHitAtAnyDirection(Collider2D collider) {
			return IsHitsContainingCollider(boxCastInfos, collider);
		}

		public bool IsHitAt(Direction direction, CollisionFilter filter) {
			return IsEvaluateHitsCollisionType(GetBoxCastInfo(direction), filter);
		}

		public bool IsHitAt(Direction direction, Collider2D collider) {
			return IsHitsContainingCollider(GetBoxCastInfo(direction), collider);
		}

		public BoxCastInfo GetBoxCastInfo(Direction direction) {
			for(int i = 0; i < boxCastInfos.Count; i++) {
				if(boxCastInfos[i].direction == direction) {
					return boxCastInfos[i];
				}
			}

			return null;
		}

		/// <summary>
		/// Set how many hits each box cast could detect.
		/// </summary>
		/// <param name="size"></param>
		public void SetHitBufferSize(int size) {
			_hitBuffer = new RaycastHit2D[size];
		}

		/// <summary>
		/// Cast the boxes, it is recommended to only update the hits when Unity events like OnCollisionEnter2D happens.
		/// </summary>
		public void UpdateHits() {
			for(int i = 0; i < boxCastInfos.Count; i++) {
				BoxCastInfo info = boxCastInfos[i];
				//info.hits.Clear();

				switch(info.direction) {
					case Direction.Up:
						info._origin			= new Vector2(referenceCollider.bounds.center.x, referenceCollider.bounds.max.y) + info.offset;
						info._size				= new Vector2(referenceCollider.size.x * info.referenceSizeMultiplier, 1f * info.directionSizeMultiplier);
						info._castDirection		= Vector2.up;
						break;
					case Direction.Down:
						info._origin			= new Vector2(referenceCollider.bounds.center.x, referenceCollider.bounds.min.y) + info.offset;
						info._size				= new Vector2(referenceCollider.size.x * info.referenceSizeMultiplier, 1f * info.directionSizeMultiplier);
						info._castDirection		= Vector2.down;
						break;
					case Direction.Left:
						info._origin			= new Vector2(referenceCollider.bounds.min.x, referenceCollider.bounds.center.y) + info.offset;
						info._size				= new Vector2(1f * info.directionSizeMultiplier * info.referenceSizeMultiplier, referenceCollider.size.y);
						info._castDirection		= Vector2.left;
						break;
					case Direction.Right:
						info._origin			= new Vector2(referenceCollider.bounds.max.x, referenceCollider.bounds.center.y) + info.offset;
						info._size				= new Vector2(1f * info.directionSizeMultiplier * info.referenceSizeMultiplier, referenceCollider.size.y);
						info._castDirection		= Vector2.right;
						break;
				}

				int hitCount = Physics2D.BoxCastNonAlloc(info.origin, info.size, 0f,
														 info.castDirection, _hitBuffer, info.distance,
														 layerMask);

				for(int a = 0; a < hitCount; a++) {
					// Filter the hits here
					if(boxCastMask.Contains(_hitBuffer[a].collider)) {
						continue;
					}

					// Call hit callback
					if(!info.hits.Contains(_hitBuffer[a].collider)) {
						OnHit(info.direction, _hitBuffer[a], _hitBuffer[a].collider);
					}
				}

				info.hits.Clear();

				for(int a = 0; a < hitCount; a++) {
					// Filter the hits here
					if(boxCastMask.Contains(_hitBuffer[a].collider)) {
						continue;
					}

					// Add the hit to the buffer
					info.hits.Add(_hitBuffer[a].collider);
				}
			}
		}

		private bool IsEvaluateHitsCollisionType(List<BoxCastInfo> infos, CollisionFilter filter) {
			for(int i = 0; i < infos.Count; i++) {
				BoxCastInfo info = infos[i];
				if(IsEvaluateHitsCollisionType(info, filter)) return true;
			}

			return false;
		}

		private bool IsEvaluateHitsCollisionType(BoxCastInfo info, CollisionFilter filter) {
			for(int i = 0; i < info.hits.Count; i++) {
				Collider2D hit = info.hits[i];
				if(!hit.isTrigger && filter == CollisionFilter.OnlyNonTrigger) return true;
				else if(hit.isTrigger && filter == CollisionFilter.OnlyTrigger) return true;
				else if(filter == CollisionFilter.Both) return info.hits.Count != 0;
			}

			return false;
		}

		private bool IsHitsContainingCollider(List<BoxCastInfo> infos, Collider2D collider) {
			for(int i = 0; i < infos.Count; i++) {
				BoxCastInfo info = infos[i];
				if(IsHitsContainingCollider(info, collider)) return true;
			}

			return false;
		}

		private bool IsHitsContainingCollider(BoxCastInfo info, Collider2D collider) {
			for(int i = 0; i < info.hits.Count; i++) {
				if(info.hits.Contains(collider)) return true;
			}

			return false;
		}


		[Serializable]
		public class BoxCastInfo {
			public Direction									direction;
			public Vector2										offset = Vector2.zero;
			public float										referenceSizeMultiplier = 1f;
			public float										directionSizeMultiplier = 1f;
			public float										distance = 1f;

			[SerializeField] internal List<Collider2D>			_hits = new List<Collider2D>();
			internal Vector2									_origin;
			internal Vector2									_size;
			internal Vector2									_castDirection;

			public List<Collider2D>								hits { get { return _hits; } }
			public Vector2										origin { get { return _origin; } }
			public Vector2										size { get { return _size; } }
			public Vector2										castDirection { get { return _castDirection; } }
		}
	}


	public class Utilities {
		public static GameObject CreateOrGetObject(string path, Transform parent = null) {
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


		// Runtime editor methods
#if UNITY_EDITOR
		public static void SetExecutionOrder(Type type, int order) {
			string scriptName = type.Name;

			foreach(MonoScript monoScript in MonoImporter.GetAllRuntimeMonoScripts()) {
				if(monoScript.name == scriptName) {
					if(MonoImporter.GetExecutionOrder(monoScript) != order) {
						MonoImporter.SetExecutionOrder(monoScript, order);
					}
					break;
				}
			}
		}
#endif
	}
}