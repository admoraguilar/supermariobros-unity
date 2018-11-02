using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace WishfulDroplet.Components {
	/// <summary>
	/// Binds MonoBehaviour to singleton. Attach this to GameObjects to make components of it singletons.
	/// </summary>
	public class SingletonBinder : MonoBehaviour {
		public bool isDontDestroyOnLoad = true;

		private List<MonoBehaviour> _monoBehaviours = new List<MonoBehaviour>();


		private void Awake() {
			if(isDontDestroyOnLoad) {
				DontDestroyOnLoad(gameObject);
			}

			GetComponents(_monoBehaviours);

			// We don't want SingletonBinders to bind themselves
			_monoBehaviours.RemoveAll(mono => mono.GetType() == typeof(SingletonBinder));

			foreach(var mono in _monoBehaviours) {
				Singleton.Add(mono.GetType(), mono);
			}
		}

		private void OnDestroy() {
			foreach(var mono in _monoBehaviours) {
				Singleton.Remove(mono.GetType());
			}
		}
	}


#if UNITY_EDITOR
	internal class SingletonBinderEditor {
		[InitializeOnLoadMethod]
		private static void RunFirstOnScriptExecutionOrder() {
			Utilities.SetExecutionOrder(typeof(SingletonBinder), -10000);
		}
	}
#endif
}
