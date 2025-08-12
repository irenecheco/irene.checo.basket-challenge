using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackboardCollisionDetection : MonoBehaviour
{
    [SerializeField] private ScoringSystem scoringSystem;
    [SerializeField] private AiScoringSystem aiScoringSystem;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Ball")
        {
            scoringSystem.UpdateTouchBackboard();
        } 
        else if(collision.collider.tag == "AiBall")
        {
            aiScoringSystem.UpdateTouchBackboard();
        }
    }
}
