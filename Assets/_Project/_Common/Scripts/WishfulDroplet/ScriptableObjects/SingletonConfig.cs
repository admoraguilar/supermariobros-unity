using UnityEngine;
using System;
using System.Linq;


namespace WishfulDroplet {
	/// <summary>
	/// A scriptable object for setting singletons on the editor.
	/// </summary>
	[CreateAssetMenu(menuName = "WishfulDroplet/ScriptableObjects/SingletonConfig")]
	public class SingletonConfig : ScriptableObject {
		public SingletonSet[] singletonSets;


		[Serializable]
		public class SingletonSet {
			public string					label;
			public GameObject[]				gameObjectSingletons = new GameObject[0];
			public ScriptableObject[]		scriptableObjectSingletons = new ScriptableObject[0];
			public bool						isEnabled = true;
		}


		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void LoadSingletonConfigOnStartup() {
			SingletonConfig[] configs = Resources.LoadAll<SingletonConfig>("");
			SingletonConfig config = configs.FirstOrDefault();

			if(!config) {
				Debug.Log("No singleton sets loaded.");
				return;
			}

			foreach(var singletonSet in config.singletonSets) {
				if(!singletonSet.isEnabled) continue;

				// Process GameObject singletons
				foreach(var singleton in singletonSet.gameObjectSingletons) {
					if(!singleton) continue;

					GameObject go = singleton as GameObject;
					if(go) {
						go = Instantiate(go);
						go.name = singleton.name;
						DontDestroyOnLoad(go);

						foreach(var mono in go.GetComponents<MonoBehaviour>()) {
							Singleton.Add(mono.GetType(), mono);
						}
					}
				}

				// Process ScriptableObjects singletons
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
