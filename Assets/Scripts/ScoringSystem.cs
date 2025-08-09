using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoringSystem : MonoBehaviour
{
    [SerializeField] private ShotPositionManager shotPositionManager;
    [SerializeField] private TextMeshProUGUI scoreText;
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
            if(rimTouched) Debug.Log("rim score");
            if(backboardTouched) Debug.Log("backboard score");
            sessionScore += 2;
        } else
        {
            Debug.Log("clean score");
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
    }

}
