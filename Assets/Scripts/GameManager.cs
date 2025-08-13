using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public enum GameState
{
    StartScreen,
    Countdown,
    Playing,
    Ending,
    GameOver
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private float totalGameTime = 60f;
    [SerializeField, Range(0f, .7f)] private float aiEasyAccuracy = .3f;
    [SerializeField, Range(0f, .7f)] private float aiMediumAccuracy = .5f;
    [SerializeField, Range(0f, .7f)] private float aiHardAccuracy = .7f;
    public static GameManager Instance;

    public GameState CurrentState { get; private set; }
    public float AiAccuracy { get; private set; } = .3f;

    public float GameTimer;
    public float FinalScore;
    public float AiFinalScore;

    public event Action EndOfTimer;

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

        if(GameTimer > 6.2f && GameTimer <6.5f)
        {
            GetComponent<AudioSource>().Play();
        }

        if (GameTimer <= 0f)
        {
            GetComponent<AudioSource>().Stop();
            EndOfTimer?.Invoke();
        }
    }

    public void StartCountdown()
    {
        SceneManager.LoadScene("Gameplay");
        GameTimer = totalGameTime;
        SetState(GameState.Countdown);
    }

    public void StartGame()
    {
        SetState(GameState.Playing);
    }

    public void EndGame()
    {
        SceneManager.LoadScene("FinalMenu");
        SetState(GameState.GameOver);
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

    public void SetAiDifficulty(int _index)
    {
        switch (_index)
        {
            case 0:
                AiAccuracy = aiEasyAccuracy;
                break;
            case 1:
                AiAccuracy = aiMediumAccuracy;
                break;
            case 2:
                AiAccuracy = aiHardAccuracy;
                break;
            default:
                AiAccuracy = aiEasyAccuracy;
                break;
        }
    }

    public bool IsGameActive()
    {
        return CurrentState == GameState.Playing;
    }
}
