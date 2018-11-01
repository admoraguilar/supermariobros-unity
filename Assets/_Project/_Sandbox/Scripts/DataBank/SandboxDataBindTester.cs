using UnityEngine;
using System.Collections.Generic;
using WishfulDroplet;
using WishfulDroplet.Extensions;


public class SandboxDataBindTester : MonoBehaviour {
	public List<GameObject> gameObjectWatcher = new List<GameObject>();


	private void Start() {
		DataWatcher.AddWatcher(ref gameObjectWatcher);
		DataWatcher.AddData(gameObject);
	}
}
