using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireballBonus : MonoBehaviour
{
    #region Serialized Fields
    [Header("Charge Settings")]
    [SerializeField, Range(0f, 1f)] private float perfectShotIncrease = .4f;
    [SerializeField, Range(0f, 1f)] private float normalShotIncrease = .25f;

    [Header("UI References")]
    [SerializeField] private Color InactiveColor;
    [SerializeField] private Color ActiveColor;
    [SerializeField] private Image BarBg;
    [SerializeField] private Image FireballImg;
    [SerializeField] private Image FillImg;
    [SerializeField] private GameObject FireballColor;

    [Header("Fireball Settings")]
    [SerializeField] private float fireballDuration = 10f;
    [SerializeField] private ShotManager shotManager;
    #endregion

    public bool FireballActive { get; private set; } = false;

    private float fireballCharge = 0f;
    private float chargeDecayRate = 0.015f;

    private void Update()
    {
        if (!FireballActive && fireballCharge > 0f)
            fireballCharge = Mathf.Max(0f, fireballCharge - chargeDecayRate * Time.deltaTime);
        FillImg.fillAmount = fireballCharge;
    }

    public void UpdateFireballBar(bool _perfectShot)
    {
        if (!FireballActive)
        {
            if (_perfectShot)
            {
                fireballCharge += perfectShotIncrease;
            }
            else
            {
                fireballCharge += normalShotIncrease;
            }
            fireballCharge = Mathf.Clamp01(fireballCharge);
            FillImg.fillAmount = fireballCharge;
            if (fireballCharge >= 1)
            {
                FireballActive = true;
                ActivateFireball();
            }
        }
    }

    public void ActivateFireball()
    {
        BarBg.color = ActiveColor;
        FireballImg.color = ActiveColor;
        FireballColor.SetActive(true);
        shotManager.StartFire();
        GetComponent<AudioSource>().Play();
        StartCoroutine(StartTimer());
    }

    public void ResetFireballBar()
    {
        FireballActive = false;
        fireballCharge = 0;
        FillImg.fillAmount = fireballCharge;
        BarBg.color = InactiveColor;
        FireballImg.color = InactiveColor;
        FireballColor.SetActive(false);
        shotManager.EndFire();
    }

    private IEnumerator StartTimer()
    {
        float _fireballTimer = fireballDuration;

        while (_fireballTimer > 0)
        {
            _fireballTimer -= Time.deltaTime;
            FillImg.fillAmount = _fireballTimer / fireballDuration;

            yield return null;
        }

        ResetFireballBar();
    }
}
