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
    [SerializeField] private ShotPositionManager shotPositionManager;
    [SerializeField] private InputHandler inputHandler;

    [Header("Force Settings")]
    public float minForce = 0f;
    public float maxForce = 1f;

    private float currentForce;
    private Vector3 cleanRangeInnerPos;
    private Vector3 cleanRangeOuterPos;
    private Vector2 cleanRangeInnerSize;
    private Vector2 cleanRangeOuterSize;
    private Vector3 backboardRangeInnerPos;
    private Vector3 backboardRangeOuterPos;
    private Vector2 backboardRangeInnerSize;
    private Vector2 backboardRangeOuterSize;

    private void Start()
    {
        Vector3 cleanRangeInitialPos = CleanRangeImg.rectTransform.position;
        Vector3 backboardRangeInitialPos = BackboardRangeImg.rectTransform.position;
        float fillImageHeight = fillImage.rectTransform.rect.height;
        cleanRangeInnerPos = new Vector3(cleanRangeInitialPos.x, cleanRangeInitialPos.y + (fillImageHeight * inputHandler.cleanMinThreshold), cleanRangeInitialPos.z);
        cleanRangeOuterPos = new Vector3(cleanRangeInitialPos.x, cleanRangeInitialPos.y + (fillImageHeight * inputHandler.cleanMinThresholdOut), cleanRangeInitialPos.z);
        backboardRangeInnerPos = new Vector3(backboardRangeInitialPos.x, backboardRangeInitialPos.y + (fillImageHeight * inputHandler.backboardMinThreshold), backboardRangeInitialPos.z);
        backboardRangeOuterPos = new Vector3(backboardRangeInitialPos.x, backboardRangeInitialPos.y + (fillImageHeight * inputHandler.backboardMinThresholdOut), backboardRangeInitialPos.z);
        cleanRangeInnerSize = new Vector2(50f, (inputHandler.cleanMaxThreshold - inputHandler.cleanMinThreshold) * fillImageHeight);
        cleanRangeOuterSize = new Vector2(50f, (inputHandler.cleanMaxThresholdOut - inputHandler.cleanMinThresholdOut) * fillImageHeight);
        backboardRangeInnerSize = new Vector2(50f, (inputHandler.backboardMaxThreshold - inputHandler.backboardMinThreshold) * fillImageHeight);
        backboardRangeOuterSize = new Vector2(50f, (inputHandler.backboardMaxThresholdOut - inputHandler.backboardMinThresholdOut) * fillImageHeight);
        SetBarRanges();
    }

    void Update()
    {
        UpdateBar(currentForce);
    }

    public void SetBarRanges()
    {
        if(shotPositionManager._previousPos <= 4)
        {
            CleanRangeImg.rectTransform.position = cleanRangeInnerPos;
            CleanRangeImg.rectTransform.sizeDelta = cleanRangeInnerSize;
            BackboardRangeImg.rectTransform.position = backboardRangeInnerPos;
            BackboardRangeImg.rectTransform.sizeDelta = backboardRangeInnerSize;
        } else
        {
            CleanRangeImg.rectTransform.position = cleanRangeOuterPos;
            CleanRangeImg.rectTransform.sizeDelta = cleanRangeOuterSize;
            BackboardRangeImg.rectTransform.position = backboardRangeOuterPos;
            BackboardRangeImg.rectTransform.sizeDelta = backboardRangeOuterSize;
        }
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
