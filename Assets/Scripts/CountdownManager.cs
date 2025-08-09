using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountdownManager : MonoBehaviour
{
    [SerializeField] private GameObject timer;
    [SerializeField] private GameObject score;
    [SerializeField] private GameObject countdownObj;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private InputHandler inputHandler;

    private float countdownTime = 3;

    private void Start()
    {
        if (GameManager.Instance.CurrentState != GameState.Countdown) return;

        StartCoroutine(StartCountdown());
    }

    private void ResetAndStart()
    {
        timer.SetActive(true);
        score.SetActive(true);
        countdownObj.SetActive(false);
        inputHandler.ResetInputs();
        GameManager.Instance.StartGame();
    }

    private IEnumerator StartCountdown()
    {
        float _countdown = countdownTime;

        while(_countdown >= 0)
        {
            _countdown -= Time.deltaTime;
            int seconds = Mathf.FloorToInt(_countdown % 60f);

            countdownText.text = string.Format("{0:0}", seconds+1);
            yield return null;
        }

        ResetAndStart();
    }
}
