using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoringSystem : MonoBehaviour
{
    [SerializeField] private ShotPositionManager shotPositionManager;
    private bool rimTouched = false;
    private bool backboardTouched = false;
    private bool scored = false;
    private int sessionScore = 0;


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
