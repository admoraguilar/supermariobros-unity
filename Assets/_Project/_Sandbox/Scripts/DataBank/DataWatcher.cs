using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


namespace WishfulDroplet {
	public static class DataWatcher {
		private static Dictionary<Type, IList<IList>> dataWatchers = new Dictionary<Type, IList<IList>>();


		public static IList<IList> GetWatchers(Type type) {
			IList<IList> value = null;
			dataWatchers.TryGetValue(type, out value);

			IList<IList> watchers = value as IList<IList>;
			return watchers;
		}

		public static void AddWatcher<T>(ref List<T> watcher) {
			Type type = typeof(T);
			if(!dataWatchers.ContainsKey(type)) {
				dataWatchers.Add(type, new List<IList>());
				//Debug.Log(string.Format("Creating new data watcher set of type: {0}", type.Name));
			}

			IList<IList> watchers = GetWatchers(typeof(T));
			if(watchers != null) {
				watchers.Add(watcher);
				//Debug.Log(string.Format("Watcher added successfully, watcher data count: {0}", watchers.Count));

				IList firstWatcher = watchers.FirstOrDefault();
				if(firstWatcher != null) {
					watcher.Clear();
					watcher.AddRange((List<T>)firstWatcher);
				}
			}
		}

		public static void RemoveWatcher<T>(List<T> watcher) {
			IList<IList> watchers = GetWatchers(watcher.GetType());
			if(watchers != null) {
				watchers.Remove(watcher);
				//Debug.Log(string.Format("Watcher removed successfully, watcher data count: {0}", watchers.Count));
			}
		}

		public static void AddData<T>(T data) {
			AddData(typeof(T), data);
		}

		public static void AddData(Type type, object data) {
			IList<IList> watchers = GetWatchers(type);
			foreach(var watcher in watchers) {
				watcher.Add(data);
				//Debug.Log(string.Format("Data added successfully, watcher data count: {0}", watcher.Count));
			}
		}

		public static void RemoveData<T>(T data) {
			RemoveData(typeof(T), data);
		}

		public static void RemoveData(Type type, object data) {
			IList<IList> watchers = GetWatchers(type);
			foreach(var watcher in watchers) {
				watcher.Remove(data);
				//Debug.Log(string.Format("Data removed successfully, watcher data count: {0}", watcher.Count));
			}
		}
	}
}
