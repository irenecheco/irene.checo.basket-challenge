using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing)
            return;

        float timeLeft = GameManager.Instance.GameTimer;
        int minutes = Mathf.FloorToInt(timeLeft / 60f);
        int seconds = Mathf.FloorToInt(timeLeft % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
