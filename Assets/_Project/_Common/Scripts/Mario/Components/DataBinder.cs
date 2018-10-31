using UnityEngine;
using WishfulDroplet;


public class DataBinder : MonoBehaviour {
	[SerializeField] private Object[] toBinds;

	private IDataBankController _thisDataBankController;


	private void Awake() {
		_thisDataBankController = Singleton.Get<IDataBankController>();

		foreach(var toBind in toBinds) {
			_thisDataBankController.AddData(toBind.GetType(), toBind);
		}
	}
}
