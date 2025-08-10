using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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

    public float GameTimer;
    public float FinalScore;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SetState(GameState.StartScreen);
        GameTimer = totalGameTime;
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
        GameTimer -= Time.deltaTime;

        if (GameTimer <= 0f)
        {
            EndGame();
        }
    }

    public void StartCountdown()
    {
        SceneManager.LoadScene("Gameplay");
        GameTimer = totalGameTime;
        SetState(GameState.Countdown);
        //in game UI
    }

    public void StartGame()
    {
        SetState(GameState.Playing);
    }

    public void EndGame()
    {
        SceneManager.LoadScene("FinalMenu");
        SetState(GameState.GameOver);
        //end screen UI
    }

    public void Restart()
    {
        SceneManager.LoadScene("StartMenu");
        SetState(GameState.StartScreen);
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
    }

    public void SetGameTime(int _index)
    {
        totalGameTime = 60 * (_index+1);
    }

    public bool IsGameActive()
    {
        return CurrentState == GameState.Playing;
    }
}
