using UnityEngine;
using System.Collections.Generic;


namespace WishfulDroplet.Extensions {
    public static class GameObjectExtensions {
        public static bool HasComponent<T>(this GameObject gameObject) {
            return gameObject.GetComponentsInChildren<T>(true) != null;
        }

        public static T AddOrGetComponent<T>(this GameObject gameObject, int index = 0) where T : Component {
            List<T> components = new List<T>(gameObject.GetComponents<T>());

            while(index >= components.Count) {
                components.Add(gameObject.AddComponent<T>());
            }

            return components[index];
        }

		public static T GetComponentInRoot<T>(this GameObject gameObject) where T : Component {
			return gameObject.transform.root.GetComponent<T>();
		}
    }
}

