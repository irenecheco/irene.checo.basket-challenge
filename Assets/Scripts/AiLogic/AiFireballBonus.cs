using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Handle fireball bonus for ai opponent
public class AiFireballBonus : MonoBehaviour
{
    #region Serialized Fields
    [Header("Fireball Charge Settings")]
    [SerializeField, Range(0f, 1f)] private float perfectShotIncrease = .4f;
    [SerializeField, Range(0f, 1f)] private float normalShotIncrease = .25f;

    [Header("Fireball Settings")]
    [SerializeField] private float fireballDuration = 10f;
    [SerializeField] private AiShotManager shotManager;
    #endregion

    #region Variables
    public bool FireballActive { get; private set; } = false;

    private float fireballCharge = 0f;
    private float chargeDecayRate = 0.015f;
    #endregion

    private void Update()
    {
        if (!FireballActive && fireballCharge > 0f)
            fireballCharge = Mathf.Max(0f, fireballCharge - chargeDecayRate * Time.deltaTime);
    }

    #region Fireball Logic
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
            if (fireballCharge >= 1)
            {
                FireballActive = true;
                ActivateFireball();
            }
        }
    }

    public void ActivateFireball()
    {
        shotManager.StartFire();
        StartCoroutine(StartTimer());
    }

    public void ResetFireballBar()
    {
        FireballActive = false;
        fireballCharge = 0;
        shotManager.EndFire();
    }
    #endregion

    #region Timer Logic
    private IEnumerator StartTimer()
    {
        float _fireballTimer = fireballDuration;

        while (_fireballTimer > 0)
        {
            _fireballTimer -= Time.deltaTime;

            yield return null;
        }

        ResetFireballBar();
    }
    #endregion
}
