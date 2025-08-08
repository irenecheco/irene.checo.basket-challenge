using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreDetection : MonoBehaviour
{
    [SerializeField] private ScoringSystem scoringSystem;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Ball")
        {
            scoringSystem.ComputeScoreType();
        }
    }
}
