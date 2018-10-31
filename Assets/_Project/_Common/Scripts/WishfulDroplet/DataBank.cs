using UnityEngine;
using System;
using System.Collections.Generic;


namespace WishfulDroplet {
	[Serializable]
	public class DataBank {
		private Dictionary<Type, List<object>> dataBank = new Dictionary<Type, List<object>>();


		public T GetData<T>() {
			Type type = typeof(T);
			List<object> bank = null;
			object data = null;

			if(dataBank.TryGetValue(type, out bank)) {
				data = bank[0];
			}

			return data != null ? (T)data : default(T);
		}

		public void AddData<T>(object data) {
			AddData(typeof(T), data);
		}

		public void AddData(Type type, object data) {
			if(!dataBank.ContainsKey(type)) {
				dataBank.Add(type, new List<object>());
			}

			dataBank[type].Add(data);
			Debug.Log(string.Format("Data: {0} binded to {1}.", data.ToString(), type.Name));
		}

		public void RemoveData<T>(object data) {
			RemoveData(typeof(T), data);
			
		}

		public void RemoveData(Type type, object data) {
			if(!dataBank.ContainsKey(type)) {
				return;
			}

			dataBank[type].Remove(data);
			Debug.Log(string.Format("Data: {0} remove binding from {1}.", data.ToString(), type.Name));
		}
	}
}
