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
    [SerializeField] private FireballBonus fireballBonus;

    [SerializeField] private Color perfectShotColor;
    [SerializeField] private Color normalShotColor;
    [SerializeField] private Color backboardBonusColor;
    [SerializeField] private TextMeshProUGUI pointsText;
    [SerializeField] private TextMeshProUGUI bonusText;
    [SerializeField] private GameObject feedbackCanvas;
    [SerializeField] private ParticleSystem shotParticle;
    [SerializeField] private ParticleSystem perfectShotParticle;
    [SerializeField] private ParticleSystem backboardBonusParticle;
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
        int _scoreIncrease = 0;
        bool _perfectShot = false;
        if (rimTouched || backboardTouched)
        {
            if (backboardTouched)
            {
                switch (backboardBonus.activeBonus)
                {
                    case BackboardBonusType.None:
                        _scoreIncrease += 2;
                        bonusText.text = "";
                        pointsText.color = normalShotColor;
                        bonusText.color = normalShotColor;
                        shotParticle.Play();
                        break;
                    case BackboardBonusType.Common:
                        _scoreIncrease += 4;
                        _perfectShot = true;
                        bonusText.text = "Backboard bonus!";
                        pointsText.color = backboardBonusColor;
                        bonusText.color = backboardBonusColor;
                        shotParticle.Play();
                        backboardBonusParticle.Play();
                        StartCoroutine(WaitAndDisableBonus());
                        break;
                    case BackboardBonusType.Rare:
                        _scoreIncrease += 6;
                        _perfectShot = true;
                        bonusText.text = "Backboard bonus!";
                        pointsText.color = backboardBonusColor;
                        bonusText.color = backboardBonusColor;
                        shotParticle.Play();
                        backboardBonusParticle.Play();
                        StartCoroutine(WaitAndDisableBonus());
                        break;
                    case BackboardBonusType.VeryRare:
                        _scoreIncrease += 8;
                        _perfectShot = true;
                        bonusText.text = "Backboard bonus!";
                        pointsText.color = backboardBonusColor;
                        bonusText.color = backboardBonusColor;
                        shotParticle.Play();
                        backboardBonusParticle.Play();
                        StartCoroutine(WaitAndDisableBonus());
                        break;
                    default:
                        _scoreIncrease += 2;
                        bonusText.text = "";
                        pointsText.color = normalShotColor;
                        bonusText.color = normalShotColor;
                        shotParticle.Play();
                        break;
                }
            } else
            {
                _scoreIncrease += 2;
                bonusText.text = "";
                pointsText.color = normalShotColor;
                bonusText.color = normalShotColor;
                shotParticle.Play();
            }            
            
        } else
        {
            _scoreIncrease += 3;
            _perfectShot = true;
            bonusText.text = "Perfect shot!";
            pointsText.color = perfectShotColor;
            bonusText.color = perfectShotColor;
            perfectShotParticle.Play();
        }
        if (fireballBonus.FireballActive) _scoreIncrease *= 2;
        sessionScore += _scoreIncrease;
        pointsText.text = $"+{_scoreIncrease} points";
        fireballBonus.UpdateFireballBar(_perfectShot);
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

    private IEnumerator WaitAndDisableBonus()
    {
        yield return new WaitForSeconds(backboardBonusParticle.main.duration);

        backboardBonus.DisableBonus();
    }

}
