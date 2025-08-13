using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum BackboardBonusType
{
    None,
    Common,
    Rare,
    VeryRare 
}

public class BackboardBonus : MonoBehaviour
{
    #region Serialized Fields
    [Header("Timing Settings")]
    [SerializeField] float minTimeBetweenBonuses = 20f;
    [SerializeField] float maxTimeBetweenBonuses = 45f;
    [SerializeField] float bonusDuration = 10f;

    [Header("Bonus Settings")]
    [SerializeField] private int commonPoints = 4;
    [SerializeField] private Color commonColor;
    [SerializeField] private int rarePoints = 6;
    [SerializeField] private Color rareColor;
    [SerializeField] private int veryRarePoints = 8;
    [SerializeField] private Color veryRareColor;

    [Header("UI References")]
    [SerializeField] private Image backboardBorder;
    [SerializeField] private TextMeshProUGUI backboardPointsText;
    [SerializeField] private GameObject backboardCanvas;

    [Header("Game References")]
    [SerializeField] private ShotManager shotManager;
    #endregion

    public BackboardBonusType activeBonus = BackboardBonusType.None;

    void Start()
    {
        StartCoroutine(BonusRoutine());
    }

    #region Bonus Logic
    public void DisableBonus()
    {
        if (activeBonus == BackboardBonusType.None) return;

        backboardCanvas.SetActive(false);
        activeBonus = BackboardBonusType.None;

        StartCoroutine(BonusRoutine());
    }

    private IEnumerator BonusRoutine()
    {
        while (GameManager.Instance.CurrentState != GameState.Playing) yield return null;

        float waitTime = Random.Range(minTimeBetweenBonuses, maxTimeBetweenBonuses);
        yield return new WaitForSeconds(waitTime);

        while (shotManager.shotInProgress) yield return null;

        activeBonus = GetRandomBonusType();

        switch(activeBonus)
        {
            case BackboardBonusType.Common:
                backboardBorder.color = commonColor;
                backboardPointsText.color = commonColor;
                backboardPointsText.text = "+" + commonPoints.ToString();
                backboardCanvas.SetActive(true);
                break;
            case BackboardBonusType.Rare:
                backboardBorder.color = rareColor;
                backboardPointsText.color = rareColor;
                backboardPointsText.text = "+" + rarePoints.ToString();
                backboardCanvas.SetActive(true);
                break;
            case BackboardBonusType.VeryRare:
                backboardBorder.color = veryRareColor;
                backboardPointsText.color = veryRareColor;
                backboardPointsText.text = "+" + veryRarePoints.ToString();
                backboardCanvas.SetActive(true);
                break;
            default:
                break;
        }

        yield return new WaitForSeconds(bonusDuration);

        if(activeBonus != BackboardBonusType.None)
        {
            DisableBonus();
        }        
    }

    BackboardBonusType GetRandomBonusType()
    {
        float r = Random.value;
        if (r < 0.5f) return BackboardBonusType.Common;      // 50%
        if (r < 0.8f) return BackboardBonusType.Rare;        // 30%
        return BackboardBonusType.VeryRare;                  // 20%
    }
    #endregion
}
