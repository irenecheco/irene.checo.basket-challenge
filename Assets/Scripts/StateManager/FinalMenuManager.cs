using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FinalMenuManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI FinalScore;
    [SerializeField] private TextMeshProUGUI GuestFinalScore;
    [SerializeField] private TextMeshProUGUI Result;

    private void Start()
    {
        FinalScore.text = GameManager.Instance.FinalScore.ToString();
        GuestFinalScore.text = GameManager.Instance.AiFinalScore.ToString();
        if (GameManager.Instance.FinalScore > GameManager.Instance.AiFinalScore) Result.text = "YOU WIN!";
        else Result.text = "MATCH LOST!";
    }

    public void BackToStart()
    {
        GameManager.Instance.Restart();
    }
}
