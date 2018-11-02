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
		Up = 0,
		Down = 1,
		Left = 2,
		Right = 3,
	}


	public enum CollisionFilter {
		OnlyNonTrigger,
		OnlyTrigger,
		Both
	}


	[Serializable]
	public class DirectionalBoxCast2D {
		public event Action<Direction, RaycastHit2D, Collider2D> OnHit = delegate { };

		public List<BoxCastInfo> boxCastInfos = new List<BoxCastInfo>();
		public LayerMask layerMask;
		public BoxCollider2D referenceCollider;
		public List<Collider2D> castMask = new List<Collider2D>();

		private RaycastHit2D[] _hitBuffer;


		public DirectionalBoxCast2D(int hitBufferSize = 20) {
			SetHitBufferSize(hitBufferSize);
		}

		public bool IsHittingAtAnyDirection(CollisionFilter filter) {
			for(int i = 0; i < boxCastInfos.Count; i++) {
				if(IsEvaluateHits(filter, boxCastInfos[i].hits)) {
					return true;
				}
			}

			return false;
		}

		public bool IsHittingAtAnyDirection(Collider2D collider) {
			for(int i = 0; i < boxCastInfos.Count; i++) {
				if(boxCastInfos[i].hits.Contains(collider)) {
					return true;
				}
			}

			return false;
		}

		public bool IsHittingAt(Direction direction, CollisionFilter filter) {
			BoxCastInfo info = GetBoxCastInfo(direction);
			if(info != null) {
				return IsEvaluateHits(filter, info.hits);
			}

			return false;
		}

		public bool IsHittingAt(Direction direction, Collider2D collider) {
			BoxCastInfo info = GetBoxCastInfo(direction);
			if(info != null) {
				return info.hits.Contains(collider);
			}

			return false;
		}

		public BoxCastInfo GetBoxCastInfo(Direction direction) {
			for(int i = 0; i < boxCastInfos.Count; i++) {
				if(boxCastInfos[i].direction == direction) {
					return boxCastInfos[i];
				}
			}

			return null;
		}

		public void SetHitBufferSize(int size) {
			_hitBuffer = new RaycastHit2D[size];
		}

		public void UpdateHits() {
			for(int i = 0; i < boxCastInfos.Count; i++) {
				boxCastInfos[i].hits.Clear();

				switch(boxCastInfos[i].direction) {
					case Direction.Up:
						boxCastInfos[i]._origin = new Vector2(referenceCollider.bounds.center.x, referenceCollider.bounds.max.y) + boxCastInfos[i]._offset;
						boxCastInfos[i]._size = new Vector2(referenceCollider.size.x * boxCastInfos[i].referenceSizeMultiplier, 1f * boxCastInfos[i].directionSizeMultiplier);
						boxCastInfos[i]._castDirection = Vector2.up;
						break;
					case Direction.Down:
						boxCastInfos[i]._origin = new Vector2(referenceCollider.bounds.center.x, referenceCollider.bounds.min.y) + boxCastInfos[i]._offset;
						boxCastInfos[i]._size = new Vector2(referenceCollider.size.x * boxCastInfos[i].referenceSizeMultiplier, 1f * boxCastInfos[i].directionSizeMultiplier);
						boxCastInfos[i]._castDirection = Vector2.down;
						break;
					case Direction.Left:
						boxCastInfos[i]._origin = new Vector2(referenceCollider.bounds.min.x, referenceCollider.bounds.center.y) + boxCastInfos[i]._offset;
						boxCastInfos[i]._size = new Vector2(1f * boxCastInfos[i].directionSizeMultiplier * boxCastInfos[i].referenceSizeMultiplier, referenceCollider.size.y);
						boxCastInfos[i]._castDirection = Vector2.left;
						break;
					case Direction.Right:
						boxCastInfos[i]._origin = new Vector2(referenceCollider.bounds.max.x, referenceCollider.bounds.center.y) + boxCastInfos[i]._offset;
						boxCastInfos[i]._size = new Vector2(1f * boxCastInfos[i].directionSizeMultiplier * boxCastInfos[i].referenceSizeMultiplier, referenceCollider.size.y);
						boxCastInfos[i]._castDirection = Vector2.right;
						break;
				}

				int hitCount = Physics2D.BoxCastNonAlloc(boxCastInfos[i].origin,
														 boxCastInfos[i].size,
														 0f,
														 boxCastInfos[i].castDirection,
														 _hitBuffer,
														 boxCastInfos[i].distance,
														 layerMask);

				for(int a = 0; a < hitCount; a++) {
					if(castMask.Contains(_hitBuffer[a].collider)) {
						continue;
					}

					if(!boxCastInfos[i].hits.Contains(_hitBuffer[a].collider)) {
						boxCastInfos[i].hits.Add(_hitBuffer[a].collider);
						OnHit(boxCastInfos[i].direction, _hitBuffer[a], _hitBuffer[a].collider);
					}
				}
			}
		}
		
		private bool IsEvaluateHits(CollisionFilter filter, List<Collider2D> hits) {
			switch(filter) {
				case CollisionFilter.OnlyNonTrigger:
					for(int i = 0; i < hits.Count; i++) {
						if(!hits[i].isTrigger) {
							return true;
						}
					}
					break;
				case CollisionFilter.OnlyTrigger:
					for(int i = 0; i < hits.Count; i++) {
						if(hits[i].isTrigger) {
							return true;
						}
					}
					break;
				default:
					return hits.Count != 0;
			}

			return false;
		}


		[Serializable]
		public class BoxCastInfo {
			public Vector2 origin { get { return _origin; } }
			public Vector2 size { get { return _size; } }
			public Vector2 castDirection { get { return _castDirection; } }
			public List<Collider2D> hits { get { return _hits; } }

			public Direction direction;
			public Vector2 _offset = Vector2.zero;
			public float referenceSizeMultiplier = 1f;
			public float directionSizeMultiplier = 1f;
			public float distance = 1f;

			[SerializeField] internal List<Collider2D> _hits = new List<Collider2D>();
			internal Vector2 _origin;
			internal Vector2 _size;
			internal Vector2 _castDirection;
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