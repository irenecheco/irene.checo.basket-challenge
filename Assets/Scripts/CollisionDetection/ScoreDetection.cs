using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreDetection : MonoBehaviour
{
    [SerializeField] private ScoringSystem scoringSystem;
    [SerializeField] private AiScoringSystem aiScoringSystem;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Ball")
        {
            scoringSystem.ComputeScoreType();
        }
        else if(other.gameObject.tag == "AiBall")
        {
            aiScoringSystem.ComputeScoreType();
        }
    }
}
