using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FinalMenuManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI FinalScore;

    private void Start()
    {
        FinalScore.text = GameManager.Instance.FinalScore.ToString();
    }

    public void BackToStart()
    {
        GameManager.Instance.Restart();
    }
}
