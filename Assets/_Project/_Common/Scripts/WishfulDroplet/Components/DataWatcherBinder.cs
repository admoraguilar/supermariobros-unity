using UnityEngine;
using WishfulDroplet;


/// <summary>
/// Binds data to the DataWatcher on Awake.
/// </summary>
public class DataWatcherBinder : MonoBehaviour {
	[SerializeField] private Object[] toBinds;


	private void Awake() {
		foreach(var toBind in toBinds) {
			DataWatcher.AddData(toBind.GetType(), toBind);
		}
	}
}
