using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoringSystem : MonoBehaviour
{
    [SerializeField] private ShotPositionManager shotPositionManager;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private UiInputBar inputBar;
    [SerializeField] private BackboardBonus backboardBonus;

    [SerializeField] private Color perfectShotColor;
    [SerializeField] private Color normalShotColor;
    [SerializeField] private Color backboardBonusColor;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TextMeshProUGUI bonusText;
    [SerializeField] private GameObject feedbackCanvas;
    private bool rimTouched = false;
    private bool backboardTouched = false;
    private bool scored = false;
    private int sessionScore = 0;

    private void Start()
    {
        scoreText.text = sessionScore.ToString();
    }


    public void UpdateTouchRim()
    {
        if (!rimTouched)
        {
            rimTouched = true;
        }
    }

    public void UpdateTouchBackboard()
    {
        if (!backboardTouched)
        {
            backboardTouched = true;
        }
    }

    public void ComputeScoreType()
    {
        scored = true;
        if (rimTouched || backboardTouched)
        {
            if (backboardTouched)
            {
                switch (backboardBonus.activeBonus)
                {
                    case BackboardBonusType.None:
                        sessionScore += 2;
                        pointsText.text = "+2 points";
                        bonusText.text = "";
                        pointsText.color = normalShotColor;
                        bonusText.color = normalShotColor;
                        break;
                    case BackboardBonusType.Common:
                        sessionScore += 4;
                        pointsText.text = "+4 points";
                        bonusText.text = "Backboard bonus!";
                        pointsText.color = backboardBonusColor;
                        bonusText.color = backboardBonusColor;
                        backboardBonus.DisableBonus();
                        break;
                    case BackboardBonusType.Rare:
                        sessionScore += 6;
                        pointsText.text = "+6 points";
                        bonusText.text = "Backboard bonus!";
                        pointsText.color = backboardBonusColor;
                        bonusText.color = backboardBonusColor;
                        backboardBonus.DisableBonus();
                        break;
                    case BackboardBonusType.VeryRare:
                        sessionScore += 8;
                        pointsText.text = "+8 points";
                        bonusText.text = "Backboard bonus!";
                        pointsText.color = backboardBonusColor;
                        bonusText.color = backboardBonusColor;
                        backboardBonus.DisableBonus();
                        break;
                    default:
                        sessionScore += 2;
                        pointsText.text = "+2 points";
                        bonusText.text = "";
                        pointsText.color = normalShotColor;
                        bonusText.color = normalShotColor;
                        break;
                }
            } else
            {
                sessionScore += 2;
                pointsText.text = "+2 points";
                bonusText.text = "";
                pointsText.color = normalShotColor;
                bonusText.color = normalShotColor;
            }            
            
        } else
        {
            sessionScore += 3;
            pointsText.text = "+3 points";
            bonusText.text = "Perfect shot!";
            pointsText.color = perfectShotColor;
            bonusText.color = perfectShotColor;
        }
        feedbackCanvas.SetActive(true);
        GameManager.Instance.FinalScore = sessionScore;
        scoreText.text = sessionScore.ToString();
    }

    public void ResetBall()
    {
        if (scored)
        {
            shotPositionManager.ChangePos();
        } else
        {
            shotPositionManager.Spawn();
        }

        feedbackCanvas.SetActive(false);
        rimTouched = false;
        backboardTouched = false;
        scored = false;
        inputBar.SetForce(0f);
    }

}
