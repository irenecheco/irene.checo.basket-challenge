using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    StartScreen,
    Countdown,
    Playing,
    GameOver
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private float totalGameTime = 60f;
    public static GameManager Instance;

    public GameState CurrentState { get; private set; }

    private float gameTimer;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        SetState(GameState.StartScreen);
        gameTimer = totalGameTime;
    }

    private void Update()
    {
        if(CurrentState == GameState.Playing)
        {
            HandleTimer();
        }
    }

    private void HandleTimer()
    {
        gameTimer -= Time.deltaTime;

        if(gameTimer <= 0f)
        {
            EndGame();
        }
    }

    public void StartGame()
    {
        gameTimer = totalGameTime;
        SetState(GameState.Playing);
        //in game UI
    }

    public void EndGame()
    {
        SetState(GameState.GameOver);
        //end screen UI
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
    }

    public bool IsGameActive()
    {
        return CurrentState == GameState.Playing;
    }
}
