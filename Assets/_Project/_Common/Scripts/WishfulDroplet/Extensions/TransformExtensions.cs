using UnityEngine;


namespace WishfulDroplet.Extensions {
    public static class TransformExtensions {
		public enum SearchType {
			BreadthFirst,
			DepthFirst
		}

		/// <summary>
		/// Traverse the transform 'til the end to find a child given a path.
		/// </summary>
		/// <param name="transform"></param>
		/// <param name="path">The path to find the child</param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Transform FindDeepChild(this Transform transform, string path, SearchType type) {
            switch(type) {
                case SearchType.BreadthFirst: {
                        Transform result = transform.Find(path);
                        if(result != null) return result;

                        foreach(Transform child in transform) {
                            result = child.FindDeepChild(path, SearchType.BreadthFirst);
                            if(result != null) return result;
                        }

                        return null;
                    }
                case SearchType.DepthFirst: {
                        foreach(Transform child in transform) {
                            if(child.name == path) return child;

							Transform result = child.FindDeepChild(path, SearchType.DepthFirst);
                            if(result != null) return result;
                        }

                        return null;
                    }
                default:
					return null;
            }
        } 
    }
}

