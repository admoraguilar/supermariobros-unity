using UnityEngine;
using System.Collections.Generic;
using WishfulDroplet;
using WishfulDroplet.Extensions;


public class SandboxDataBindTester : MonoBehaviour {
	public List<GameObject> gameObjects = new List<GameObject>();

	private IDataBankController dataBankController;


	private void Awake() {
		dataBankController = Singleton.Get<IDataBankController>();
	}

	private void Start() {
		GameObject go = dataBankController.GetData<GameObject>();
		Debug.Log(go.name);
	}
}
