using UnityEngine;
using System;


public class PlayerStatsController : MonoBehaviour {
    public event Action OnCoinsReachedMax = delegate { };
    public event Action OnLevelTimeLeftZero = delegate { };

    public int Score { get { return score; } set { score = value; } }

    public int Coins {
        get { return coins; }
        set {
            coins = value;
            if(coins >= maxCoins) {
                OnCoinsReachedMax();
                coins = 0;
            }
        }
    }

    public int Lives {
        get { return lives; }
        set {
            lives = value;
            if(lives >= maxLives) {
                lives = maxLives;
            }
        }
    }

    public float LevelTimeLeft {
        get { return levelTimeLeft; }
        set {
            levelTimeLeft = value;
            if(levelTimeLeft <= 0) {
                OnLevelTimeLeftZero();
                levelTimeLeft = 0;
            }
        }
    }

    public string CurrentLevelInfo { get { return currentLevelInfo; } set { currentLevelInfo = value; } }

    [SerializeField] private int maxCoins;
    [SerializeField] private int maxLives;

    [SerializeField] private int score;
    [SerializeField] private int coins;
    [SerializeField] private int lives;
    [SerializeField] private float levelTimeLeft;
    [SerializeField] private string currentLevelInfo;
}
