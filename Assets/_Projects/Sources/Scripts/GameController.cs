using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class GameController : MonoBehaviour {
    [SerializeField] private float loadingScreenTime = 2f;

    [SerializeField] private Camera mainCameraPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private string[] levels;
    [SerializeField] private Canvas masterOverlayCanvasPrefab;
    [SerializeField] private EventSystem eventSystemPrefab;

    [SerializeField] private GameObject loadingScreenUiPrefab;
    [SerializeField] private GameObject mainMenuScreenUiPrefab;
    [SerializeField] private GameObject playerStatsUiPrefab;

    [Header("Debug")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject player;
    [SerializeField] private Canvas masterOverlayCanvas;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject loadingScreenUi;
    [SerializeField] private GameObject mainMenuScreenUi;
    [SerializeField] private GameObject playerStatsUi;

    [SerializeField] private Level currentLevel;
    [SerializeField] private Scene currentLoadedLevelScene;
    [SerializeField] private int currentLevelIndex;

    private PlayerStats thisPlayerStats;


    private IEnumerator LevelChanger() {
        while(true) {
            if(currentLoadedLevelScene.name != levels[currentLevelIndex]) {
                loadingScreenUi.SetActive(true);

                // Load the desired level
                yield return SceneManager.LoadSceneAsync(levels[currentLevelIndex]);
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(levels[currentLevelIndex]));
                currentLoadedLevelScene = SceneManager.GetActiveScene();

                // Unload the existing level
                for(int i = 0; i < SceneManager.sceneCount; i++) {
                    if(SceneManager.GetSceneAt(i).name != levels[currentLevelIndex] &&
                       SceneManager.GetSceneAt(i).isLoaded) {
                        yield return SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i));
                    }
                }

                // Get level object
                GameObject goLevel = SceneManager.GetActiveScene().GetRootGameObjects().FirstOrDefault(go => {
                    return go.GetComponent<Level>();
                });
                if(goLevel) {
                    currentLevel = goLevel.GetComponent<Level>();

                    // Setup some stats
                    thisPlayerStats.CurrentLevelInfo = currentLoadedLevelScene.name;
                    thisPlayerStats.LevelTimeLeft = currentLevel.Time;

                    // Setup player
                    player = Spawn(playerPrefab, false);
                    player.GetComponent<Transform>().position = currentLevel.PlayerStart.position;

                    // Setup camera
                    mainCamera = Spawn(mainCameraPrefab, false);
                    mainCamera.GetComponent<Camera2DFollow>().SetTarget(player.GetComponent<Transform>());
                    mainCamera.backgroundColor = currentLevel.BackgroundColor;

                    // Setup audio
                    currentLevel.AudioSource.clip = currentLevel.BGM;
                    currentLevel.AudioSource.loop = true;
                    currentLevel.AudioSource.PlayDelayed(loadingScreenTime);
                } else {
                    Debug.LogError("Level has no level object!");
                }

                yield return new WaitForSeconds(loadingScreenTime);
                loadingScreenUi.SetActive(false);
            }

            yield return null;
        }
    }

    private T Spawn<T>(T toSpawn, bool dontDestroy, Transform parent = null) where T : UnityEngine.Object {
        T obj = Instantiate(toSpawn);
        obj.name = toSpawn.name;
        if(dontDestroy) DontDestroyOnLoad(obj);

        if(parent) {
            Transform objT = null;

            GameObject go = obj as GameObject;
            if(go) objT = go.GetComponent<Transform>();

            Behaviour bh = obj as Behaviour;
            if(bh) objT = bh.GetComponent<Transform>();

            objT.SetParent(parent, false);
        }

        return obj;
    }

    private void Awake() {
        thisPlayerStats = SingletonController.Get<PlayerStats>();
    }

    private void Start() {
        // Spawn systems
        masterOverlayCanvas = Spawn(masterOverlayCanvasPrefab, true);
        eventSystem = Spawn(eventSystemPrefab, true);

        // Spawn uis
        loadingScreenUi = Spawn(loadingScreenUiPrefab, true, masterOverlayCanvas.GetComponent<Transform>());
        playerStatsUi = Spawn(playerStatsUiPrefab, true, masterOverlayCanvas.GetComponent<Transform>());

        // Start level changer behaviour
        StartCoroutine(LevelChanger());
    }
}
