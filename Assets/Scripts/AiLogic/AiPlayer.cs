using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPlayer : MonoBehaviour
{
    [SerializeField] private AiShotManager aiShotManager;
    [SerializeField] private float timeBetweenShots = 2.5f;
    [SerializeField] private float backboardProb = 0.15f;
    [SerializeField] private float rimProb = 0.05f;

    [SerializeField] private float missProb = 0.05f;
    [SerializeField] private float backboardMissProb = 0.03f;
    //[SerializeField] private float longMissProb = 0.02f;

    public float accuracy = .3f;

    void Start()
    {
        accuracy = GameManager.Instance.AiAccuracy;
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
            //Debug.Log($"shot type è {shotType}");
        }
    }

    private ShotType DecideShotType()
    {
        float roll = Random.value;

        if (roll <= accuracy) return ShotType.Clean;
        else if (roll <= accuracy + backboardProb) return ShotType.Backboard;
        else if (roll <= accuracy + backboardProb + rimProb) return ShotType.Rim;
        else if (roll <= accuracy + backboardProb + rimProb + missProb) return ShotType.Miss;
        else if (roll <= accuracy + backboardProb + rimProb + missProb + backboardMissProb) return ShotType.BackboardMiss;
        else return ShotType.LongMiss;
    }
}
