using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPlayer : MonoBehaviour
{
    [SerializeField] private AiShotManager aiShotManager;
    [SerializeField] private float timeBetweenShots = 2.5f;
    [SerializeField, Range(0f, .7f)] private float accuracy = .7f;// 70% clean
    [SerializeField] private float backboardProb = 0.15f;    // +15%
    [SerializeField] private float rimProb = 0.05f;          // +5%

    [SerializeField] private float missProb = 0.05f;       // 5%
    [SerializeField] private float backboardMissProb = 0.03f;        // 3%
    [SerializeField] private float longMissProb = 0.02f;   // 2%

    void Start()
    {
        StartCoroutine(AIPlayLoop());
    }

    private IEnumerator AIPlayLoop()
    {
        while(GameManager.Instance.CurrentState != GameState.Playing)
        {
            yield return null;
        }

        while (GameManager.Instance.CurrentState == GameState.Playing)
        {
            yield return new WaitForSeconds(timeBetweenShots);

            ShotType shotType = DecideShotType();

            aiShotManager.Shoot(shotType);
            Debug.Log($"shot type è {shotType}");
        }
    }

    private ShotType DecideShotType()
    {
        float roll = Random.value; // 0..1
        Debug.Log($"calcola shot type e roll è {roll}");

        if (roll <= accuracy) return ShotType.Clean;
        else if (roll <= accuracy + backboardProb) return ShotType.Backboard;
        else if (roll <= accuracy + backboardProb + rimProb) return ShotType.Rim;
        else if (roll <= accuracy + backboardProb + rimProb + missProb) return ShotType.Miss;
        else if (roll <= accuracy + backboardProb + rimProb + missProb + backboardMissProb) return ShotType.BackboardMiss;
        else return ShotType.LongMiss;
    }
}
