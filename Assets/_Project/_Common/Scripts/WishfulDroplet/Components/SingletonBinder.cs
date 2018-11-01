using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace WishfulDroplet.Components {
	public class SingletonBinder : MonoBehaviour {
		public bool isDontDestroyOnLoad = true;

		private List<MonoBehaviour> monoBehaviours = new List<MonoBehaviour>();


		private void Awake() {
			if(isDontDestroyOnLoad) {
				DontDestroyOnLoad(gameObject);
			}

			GetComponents(monoBehaviours);

			foreach(var mono in monoBehaviours) {
				Singleton.Add(mono.GetType(), mono);
			}
		}

		private void OnDestroy() {
			foreach(var mono in monoBehaviours) {
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
