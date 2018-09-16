using UnityEngine;
using UnityEngine.UI;


public class PlayerStatsUiController : MonoBehaviour {
    [SerializeField] private Text scoreValueText;
    [SerializeField] private Text coinsValueText;
    [SerializeField] private Text currentLevelInfoText;
    [SerializeField] private Text timeValueText;

    private PlayerStats thisPlayerStats;


    private void Awake() {
        thisPlayerStats = SingletonController.Get<PlayerStats>();
    }

    private void Update() {
        scoreValueText.text = thisPlayerStats.Score.ToString("000000");
        coinsValueText.text = thisPlayerStats.Coins.ToString("00");
        currentLevelInfoText.text = thisPlayerStats.CurrentLevelInfo;
        timeValueText.text = thisPlayerStats.LevelTimeLeft.ToString("000");
    }
}
