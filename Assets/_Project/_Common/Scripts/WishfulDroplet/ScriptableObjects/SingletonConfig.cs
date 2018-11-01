using UnityEngine;
using System;
using System.Linq;


namespace WishfulDroplet {
	[CreateAssetMenu(menuName = "WishfulDroplet/ScriptableObjects/SingletonConfig")]
	public class SingletonConfig : ScriptableObject {
		public SingletonSet[] SingletonSets;


		[Serializable]
		public class SingletonSet {
			public string label;
			public GameObject[] gameObjectSingletons = new GameObject[0];
			public ScriptableObject[] scriptableObjectSingletons = new ScriptableObject[0];
			public bool isEnabled = true;
		}


		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void LoadSingletonConfigOnStartup() {
			SingletonConfig[] configs = Resources.LoadAll<SingletonConfig>("");
			SingletonConfig config = configs.FirstOrDefault();

			if(!config) {
				Debug.Log("No singleton sets loaded.");
				return;
			}

			foreach(var singletonSet in config.SingletonSets) {
				if(!singletonSet.isEnabled) continue;

				foreach(var singleton in singletonSet.gameObjectSingletons) {
					if(!singleton) continue;

					GameObject go = singleton as GameObject;
					if(go) {
						go = UnityEngine.Object.Instantiate(go);
						go.name = singleton.name;
						UnityEngine.Object.DontDestroyOnLoad(go);

						foreach(var mono in go.GetComponents<MonoBehaviour>()) {
							Singleton.Add(mono.GetType(), mono);
						}
					}
				}

				foreach(var singleton in singletonSet.scriptableObjectSingletons) {
					if(!singleton) continue;

					ScriptableObject so = singleton as ScriptableObject;
					if(so) {
						Singleton.Add(so.GetType(), so);
					}
				}
			}
		}
	}
}
