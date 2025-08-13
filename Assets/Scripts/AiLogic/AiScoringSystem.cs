using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AiScoringSystem : MonoBehaviour
{
    #region Serialized Fields
    [Header("References")]
    [SerializeField] private AiShotPositionManager aiShotPositionManager;
    [SerializeField] private TextMeshProUGUI aiScoreText;
    [SerializeField] private AiFireballBonus fireballBonus;
    #endregion

    #region Variables
    private bool rimTouched = false;
    private bool backboardTouched = false;
    private bool scored = false;
    private int sessionScore = 0;
    #endregion

    private void Start()
    {
        aiScoreText.text = sessionScore.ToString();
    }

    #region Scoring Logic for ai
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
        int _scoreIncrease = 0;
        bool _perfectShot = false;
        if (rimTouched || backboardTouched)
        {
            _scoreIncrease += 2;    
            
        } else
        {
            _scoreIncrease += 3;
            _perfectShot = true;
        }
        if (fireballBonus.FireballActive) _scoreIncrease *= 2;
        sessionScore += _scoreIncrease;
        fireballBonus.UpdateFireballBar(_perfectShot);
        GameManager.Instance.AiFinalScore = sessionScore;
        aiScoreText.text = sessionScore.ToString();
    }

    public void ResetBall()
    {
        if (scored)
        {
            aiShotPositionManager.ChangePos();
        } else
        {
            aiShotPositionManager.Spawn();
        }

        rimTouched = false;
        backboardTouched = false;
        scored = false;
    }
    #endregion
}
