using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Linq;
using System.Collections.Generic;


namespace WishfulDroplet {
    public static class Singleton {
        private static Dictionary<Type, object> singletons = new Dictionary<Type, object>();


        public static T Get<T>() where T : class {
			Type type = typeof(T);
			object value = null;

			if(!singletons.TryGetValue(type, out value)) {
				foreach(var singleton in singletons.Values) {
					if(type.IsAssignableFrom(singleton.GetType())) {
						value = singleton;
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
			singletons[type] = singleton;
		}

		public static void Remove<T>() where T : class {
			Remove(typeof(T));
		}

		public static void Remove(Type type) {
			singletons.Remove(type);
		}
    }
}