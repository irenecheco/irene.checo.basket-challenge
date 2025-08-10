using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiInputBar : MonoBehaviour
{
    [Header("References")]
    public Image fillImage;
    [SerializeField] private Image CleanRangeImg;
    [SerializeField] private Image BackboardRangeImg;

    [Header("Force Settings")]
    public float minForce = 0f;
    public float maxForce = 1f;

    [Header("Scoring Range")]
    public float cleanScoreMin = 0.45f;
    public float cleanScoreMax = 0.60f;
    public float backboardScoreMin = 0.80f;
    public float backboardScoreMax = 0.95f;

    private float currentForce;

    private void Start()
    {
        Debug.Log($"{CleanRangeImg.rectTransform.position} è posizione, {fillImage.rectTransform.rect.height * ((cleanScoreMin + cleanScoreMax) / 2)} è somma");
        CleanRangeImg.rectTransform.position = new Vector3(CleanRangeImg.rectTransform.position.x, CleanRangeImg.rectTransform.position.y +(fillImage.rectTransform.rect.height * cleanScoreMin), CleanRangeImg.rectTransform.position.z);
        CleanRangeImg.rectTransform.sizeDelta = new Vector2(50f, (cleanScoreMax - cleanScoreMin) * fillImage.rectTransform.rect.height);
        BackboardRangeImg.rectTransform.position = new Vector3(BackboardRangeImg.rectTransform.position.x, BackboardRangeImg.rectTransform.position.y + (fillImage.rectTransform.rect.height * backboardScoreMin), BackboardRangeImg.rectTransform.position.z);
        BackboardRangeImg.rectTransform.sizeDelta = new Vector2(50f, (backboardScoreMax - backboardScoreMin) * fillImage.rectTransform.rect.height);
    }

    void Update()
    {
        UpdateBar(currentForce);
    }

    public void SetForce(float force)
    {
        currentForce = Mathf.Clamp(force, minForce, maxForce);
    }

    private void UpdateBar(float force)
    {
        fillImage.fillAmount = currentForce;
    }
}
