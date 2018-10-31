using UnityEngine;
using System;
using WishfulDroplet;
using WishfulDroplet.Extensions;


[CreateAssetMenu(menuName = "Mario/Singletons/DataBankController")]
public class DataBankController : ScriptableObject, IDataBankController {
	[SerializeField] private DataBank dataBank;


	public T GetData<T>() {
		return dataBank.GetData<T>();
	}

	public void AddData<T>(object data) {
		dataBank.AddData<T>(data);
	}

	public void AddData(Type type, object data) {
		dataBank.AddData(type, data);
	}

	public void RemoveData<T>(object data) {
		dataBank.RemoveData<T>(data);
	}

	public void RemoveData(Type type, object data) {
		dataBank.RemoveData(type, data);
	}
}


public interface IDataBankController {
	T GetData<T>();
	void AddData<T>(object data);
	void AddData(Type type, object data);
	void RemoveData<T>(object data);
	void RemoveData(Type type, object data);
}
