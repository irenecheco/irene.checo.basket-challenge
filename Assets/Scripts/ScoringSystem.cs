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
                        break;
                    case BackboardBonusType.Common:
                        sessionScore += 4;
                        backboardBonus.DisableBonus();
                        break;
                    case BackboardBonusType.Rare:
                        sessionScore += 6;
                        backboardBonus.DisableBonus();
                        break;
                    case BackboardBonusType.VeryRare:
                        sessionScore += 8;
                        backboardBonus.DisableBonus();
                        break;
                    default:
                        sessionScore += 2;
                        break;
                }
            } else
            {
                sessionScore += 2;
            }            
            
        } else
        {
            sessionScore += 3;
        }
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

        rimTouched = false;
        backboardTouched = false;
        scored = false;
        inputBar.SetForce(0f);
    }

}
