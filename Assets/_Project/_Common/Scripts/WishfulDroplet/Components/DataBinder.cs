using UnityEngine;
using WishfulDroplet;


public class DataBinder : MonoBehaviour {
	[SerializeField] private Object[] toBinds;


	private void Awake() {
		foreach(var toBind in toBinds) {
			DataWatcher.AddData(toBind.GetType(), toBind);
		}
	}
}
