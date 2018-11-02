using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Linq;
using System.Collections.Generic;


namespace WishfulDroplet {
	/// <summary>
	/// A Singleton manager.
	/// </summary>
    public static class Singleton {
        private static Dictionary<Type, object> _singletons = new Dictionary<Type, object>();


        public static T Get<T>() where T : class {
			Type type = typeof(T);
			object value = null;

			if(!_singletons.TryGetValue(type, out value)) {
				// If we don't find any singleton of that type then
				// we try to iterate the values if there are singletons 
				// which are assignable to our type
				foreach(var singleton in _singletons) {
					if(type.IsAssignableFrom(singleton.Value.GetType())) {
						value = singleton.Value;
						Add(type, value);
						break;
					}
				}
			}

			return (T)value;
        }

		public static void Add<T>(T singleton) where T : class {
			Add(typeof(T), singleton);
		}

		public static void Add(Type type, object singleton) {
			Assert.IsTrue(type.IsAssignableFrom(singleton.GetType()));
			_singletons[type] = singleton;
		}

		public static void Remove<T>() where T : class {
			Remove(typeof(T));
		}

		public static void Remove(Type type) {
			_singletons.Remove(type);
		}
    }
}