using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WishfulDroplet;
using WishfulDroplet.Extensions;


public class SandboxDataBindTester : MonoBehaviour {
	public List<Camera> cameraWatcher = new List<Camera>();


	private void Start() {
		DataWatcher.AddWatcher(ref cameraWatcher);
		//DataWatcher.AddData(gameObject);
	}

	private void Update() {
		
	}

	private void OnDestroy() {
		DataWatcher.RemoveWatcher(cameraWatcher);
		Debug.Log(string.Format("Removing watcher, count now: {0}", DataWatcher.GetWatchers<Camera>().Count));
	}
}
