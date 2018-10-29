using UnityEngine;


namespace WishfulDroplet.Extensions {
    public static class ComponentExtensions {
        public static bool HasComponent<T>(this Component component) {
            return component.GetComponentsInChildren<T>(true) != null;
        }

		public static T GetComponentInRoot<T>(this Component component) where T : Component {
			return component.transform.root.GetComponent<T>();
		}
	}
}

