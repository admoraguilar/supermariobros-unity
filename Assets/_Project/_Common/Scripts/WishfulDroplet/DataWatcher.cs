using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


namespace WishfulDroplet {
	/// <summary>
	/// A powerful tool to decouple UI code from game code.
	/// </summary>
	public static class DataWatcher {
		private static Dictionary<Type, IList<IList>> dataWatchers = new Dictionary<Type, IList<IList>>();


		public static void GetWatchers<T>(List<List<T>> results) {
			IList<IList> watchers = GetWatchers(typeof(T));

			foreach(var watcher in watchers) {
				List<T> castWatcher = watcher as List<T>;
				if(castWatcher != null) {
					results.Add(castWatcher);
				}
			}
		} 

		public static List<List<T>> GetWatchers<T>() {
			// Plz optimize this code this is the headache of the bunch
			IList<IList> watchers = GetWatchers(typeof(T));

			List<List<T>> castWatchers = new List<List<T>>();
			foreach(var watcher in watchers) {
				List<T> castWatcher = watcher as List<T>;
				if(castWatcher != null) {
					castWatchers.Add(castWatcher);
				}
			}

			return castWatchers;
		}

		public static IList<IList> GetWatchers(Type type) {
			IList<IList> value = null;
			if(!dataWatchers.TryGetValue(type, out value)) {
				value = new List<IList>();
				dataWatchers.Add(type, value);
				value.Add(new ArrayList());
				//Debug.Log(string.Format("Creating new data watcher set of type: {0}", type.Name));
			}

			IList<IList> watchers = value as IList<IList>;
			return watchers;
		}

		public static void AddWatcher<T>(List<T> watcher) {
			IList cast = watcher;
			AddWatcher(typeof(T), cast);
		}

		public static void AddWatcher(Type type, IList watcher) {
			IList<IList> watchers = GetWatchers(type);
			if(watchers != null) {
				watchers.Add(watcher);
				//Debug.Log(string.Format("Watcher added successfully, watcher data count: {0}", watchers.Count));

				IList firstWatcher = watchers.FirstOrDefault();
				if(firstWatcher != null) {
					watcher.Clear();
					foreach(var data in firstWatcher) {
						watcher.Add(data);
					}
				}
			}
		}

		public static void RemoveWatcher<T>(List<T> watcher) {
			IList cast = watcher;
			RemoveWatcher(typeof(T), watcher);
		}

		public static void RemoveWatcher(Type type, IList watcher) {
			IList<IList> watchers = GetWatchers(type);
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
