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

    [SerializeField] private bool isDebugMode;

    [Header("Debug")]
#pragma warning disable 0414
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject player;
    [SerializeField] private Canvas masterOverlayCanvas;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject loadingScreenUi;
    [SerializeField] private GameObject mainMenuScreenUi;
    [SerializeField] private GameObject playerStatsUi;
#pragma warning restore 0414

    [SerializeField] private Level currentLevel;
    [SerializeField] private Scene currentLoadedLevelScene;
    [SerializeField] private int currentLevelIndex;

    private PlayerStatsController thisPlayerStats;


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

                // Disable debug objects
                GameObject goDebug = SceneManager.GetActiveScene().GetRootGameObjects().FirstOrDefault(go => {
                    return go.name == "Debug";
                });
                goDebug.SetActive(false);

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
                    player = this.Spawn(playerPrefab);
                    player.GetComponent<Transform>().position = currentLevel.PlayerStart.position;

                    // Setup camera
                    mainCamera = this.Spawn(mainCameraPrefab);
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

    private void Awake() {
        thisPlayerStats = SingletonController.Get<PlayerStatsController>();
    }

    private void Start() {
        // Spawn systems
        masterOverlayCanvas = this.Spawn(masterOverlayCanvasPrefab, null, true);
        eventSystem = this.Spawn(eventSystemPrefab, null, true);

        // Spawn uis
        loadingScreenUi = this.Spawn(loadingScreenUiPrefab, masterOverlayCanvas.GetComponent<Transform>(), true);
        playerStatsUi = this.Spawn(playerStatsUiPrefab, masterOverlayCanvas.GetComponent<Transform>(), true);

        if(isDebugMode) {
            loadingScreenUi.SetActive(false);
            return;
        }

        // Start level changer behaviour
        StartCoroutine(LevelChanger());
    }
}
